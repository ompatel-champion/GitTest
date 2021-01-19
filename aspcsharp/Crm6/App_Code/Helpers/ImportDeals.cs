using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crm6.App_Code.Login;
using ClosedXML.Excel;
using GlobalUser = Crm6.App_Code.Login.GlobalUser;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;

namespace Helpers
{
    public class ImportDeals
    {
        private DbLoginDataContext _loginContext;
        private readonly Crm6.App_Code.Shared.DbSharedDataContext _sharedContext;
        private readonly DbFirstFreightDataContext _crmContext;

        // note: if we get a DI framework running, this constructor won't be necessary
        public ImportDeals() : this(LoginUser.GetWritableSharedConnectionForDataCenter(""), LoginUser.GetConnection())
        {
        }

        public ImportDeals(string sharedConnectionString, string crmConnectionString)
        {
            _sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnectionString);
            _crmContext = new DbFirstFreightDataContext(crmConnectionString);
        }


        public bool PerformDealsImport(int subscriberId, int userId, string blobReference, string containerReference)
        {
            var connection = LoginUser.GetConnection();
            using (_loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection()))
            using (var workbook = GetWorkbook(containerReference, blobReference))
            {
                var user = _loginContext.GlobalUsers.FirstOrDefault(i => i.UserId == userId && i.SubscriberId == subscriberId);
                var dataCenter = user?.DataCenter ?? HttpContext.Current.Session["UserDataCenter"].ToString();
                //  get deals from the excel
                var dealImports = GetDealImports(workbook, subscriberId, user).ToList();

                // add deals
                foreach (var dealImportItem in dealImports)
                {
                    AddDeal(subscriberId, user, dealImportItem, connection);
                }

            }
            return true;
        }


        public bool AddDeal(int subscriberId, GlobalUser user, DealsImportModel dealImportItem, string coneection)
        {
            var context = new DbFirstFreightDataContext(coneection);

            var objDeal = new Deal();
            objDeal.DealName = (!string.IsNullOrWhiteSpace(dealImportItem.Shipper) ? dealImportItem.Shipper : dealImportItem.Consignee)
                                + (!string.IsNullOrWhiteSpace(dealImportItem.Commodity) ? (" - " + dealImportItem.Commodity) : "");

            // sales stage
            switch (dealImportItem.DealStatus)
            {
                case "Expired":
                    objDeal.SalesStageName = "Lost";
                    break;
                case "SecuredHouse":
                    objDeal.SalesStageName = "Won";
                    break;
                case "Open":
                    objDeal.SalesStageName = "Qualifying";
                    break;
                case "Lost":
                    objDeal.SalesStageName = "Lost";
                    break;
                case "Secured":
                    objDeal.SalesStageName = "Won";
                    break;
                case "Abandoned":
                    objDeal.SalesStageName = "Lost";
                    break;
                default:
                    objDeal.SalesStageName = "Qualifying";
                    break;
            }
            var stage = context.SalesStages.FirstOrDefault(t => t.SalesStageName.ToLower() == objDeal.SalesStageName.ToLower() && t.SubscriberId == subscriberId && !t.Deleted);
            if (stage != null)
            {
                objDeal.SalesStageId = stage.SalesStageId;
            }

            // company
            var company = context.Companies.FirstOrDefault(t => t.CompanyName.ToLower() == (dealImportItem.Shipper + "").Trim().ToLower() && t.SubscriberId == subscriberId && !t.Deleted);
            if (company == null)
            {
                company = context.Companies.FirstOrDefault(t => t.CompanyName.ToLower() == (dealImportItem.Consignee + "").Trim().ToLower() && t.SubscriberId == subscriberId && !t.Deleted);
            }
            if (company == null)
            {
                return false;
            }

            objDeal.CompanyId = company.CompanyId;
            // get company 
            objDeal.CompanyIdGlobal = company.CompanyIdGlobal;
            objDeal.CompanyName = company.CompanyName;
            objDeal.Commodities = dealImportItem.Commodity;
            objDeal.Comments = dealImportItem.Comments;
            objDeal.DealOwnerId = user.UserId;
            objDeal.PrimaryContactName = "";
            // dates 
            objDeal.DecisionDate = dealImportItem.DecisionDate;
            objDeal.LastUpdate = dealImportItem.LastUpdatedDate;

            // last update
            objDeal.LastUpdate = DateTime.UtcNow;
            objDeal.UpdateUserId = user.UserId;
            objDeal.UpdateUserName = user.FullName;

            objDeal.Services = dealImportItem.Service;
            objDeal.Revenue = dealImportItem.DealValue;
            objDeal.RevenueUSD = dealImportItem.DealValue;

            // won reason
            if (objDeal.SalesStageName == "Won")
            {
                objDeal.SalesStageName = "Won";
                objDeal.Won = true;
                objDeal.Lost = false;
                if (objDeal.DateWon == null)
                    objDeal.DateWon = dealImportItem.DecisionDate;

            }
            // lost reason
            else if (objDeal.SalesStageName == "Lost")
            {
                objDeal.SalesStageName = "Lost";
                objDeal.Lost = true;
                objDeal.Won = false;
                if (objDeal.DateLost == null)
                    objDeal.DateLost = dealImportItem.DecisionDate;
            }
            else
            {
                objDeal.Lost = false;
                objDeal.Won = false;
                objDeal.DateLost = null;
                objDeal.DateWon = null;
                objDeal.ReasonWonLost = "";
            }

            // insert new deal
            if (objDeal.DealId < 1)
            {
                objDeal.SubscriberId = subscriberId;
                objDeal.CreatedUserId = user.UserId;
                objDeal.CreatedDate = DateTime.UtcNow;
                objDeal.CreatedUserName = objDeal.UpdateUserName;
                objDeal.DealOwnerId = user.UserId;
                context.Deals.InsertOnSubmit(objDeal);
            }

            context.SubmitChanges();

            // add deal owner | Only add for the new deals
            if (objDeal.DealId > 0 && objDeal.DealOwnerId > 0)
            {
                var updatedUserName = context.Users.Where(u => u.UserId == objDeal.DealOwnerId)
                                        .Select(u => u.FullName).FirstOrDefault() ?? "";

                var dealUser = new LinkUserToDeal();
                var found = context.LinkUserToDeals.FirstOrDefault(u => u.DealId == objDeal.DealId && u.UserId == objDeal.DealOwnerId
                                                    && !u.Deleted);
                if (found == null)
                {
                    // add deal user
                    dealUser.UserName = context.Users.Where(u => u.UserId == objDeal.DealOwnerId).Select(u => u.FullName).FirstOrDefault() ?? "";
                    dealUser.CreatedUserId = objDeal.DealOwnerId;
                    dealUser.CreatedUserName = updatedUserName;
                    dealUser.CreatedDate = DateTime.UtcNow;
                    dealUser.UpdateUserId = objDeal.DealOwnerId;
                    dealUser.UpdateUserName = updatedUserName;
                    dealUser.LastUpdate = DateTime.UtcNow;
                    dealUser.DealId = objDeal.DealId;
                    dealUser.LinkType = "";
                    dealUser.DealName = objDeal.DealName;
                    context.LinkUserToDeals.InsertOnSubmit(dealUser);
                }
                context.SubmitChanges();

                // sales team 
                var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == dealUser.DealId && !t.Deleted)
                                 .Select(t => t.UserName).ToList();
                objDeal.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();


                // add lane

                if (dealImportItem.DealValue > 0)
                {
                    var objLane = new Lane();

                    // populate basic lane fields
                    objLane.DealId = objDeal.DealId;
                    objLane.Service = dealImportItem.Service;
                    objLane.Revenue = dealImportItem.DealValue;

                    // convert revenue to USD
                    objLane.RevenueUSD = dealImportItem.DealValue;

                    objLane.CurrencyCode = "USD";

                    objLane.LastUpdate = DateTime.UtcNow;
                    objLane.UpdateUserId = objDeal.UpdateUserId;
                    objLane.UpdateUserName = objDeal.UpdateUserName;


                    // insert new lane
                    if (objLane.LaneId < 1)
                    {
                        objLane.SubscriberId = objDeal.SubscriberId;
                        objLane.CreatedUserId = objDeal.UpdateUserId;
                        objLane.CreatedDate = DateTime.UtcNow;
                        objLane.CreatedUserName = objLane.UpdateUserName;
                        context.Lanes.InsertOnSubmit(objLane);
                    }
                    context.SubmitChanges();

                }

            }

            return false;
        }


        /// <summary>
        /// Get the excel workbook from Azure storage. Defined as public field to make it easier to mock in test.
        ///  This decision can be revisited.  Discussion is welcome.
        /// </summary>
        public Func<string, string, XLWorkbook> GetWorkbook = (cref, bref) =>
        {
            var docStream = new BlobStorageHelper().DownloadBlobStream(cref, bref);
            return new XLWorkbook(docStream);
        };


        public IEnumerable<DealsImportModel> GetDealImports(XLWorkbook workbook, int subscriberId, GlobalUser user)
        {
            return workbook.Worksheet(1)
                .Rows()
                .Where(i => !DealsImportModel.IsHeaderRow(i))
                .Select(i => DealsImportModel.FromXlRow(i, subscriberId, user));
        }


    }


    public class DealsImportModel
    {
        public static bool IsHeaderRow(IXLRow row)
        {
            var val = row.Cell(1).Value.ToString().ToLower().Trim();
            return val == "id";
        }


        public static DealsImportModel FromXlRow(IXLRow row, int subscriberId, GlobalUser user)
        {
            var xxx = row?.Cell("A").Value.ToString() ?? "Bob Dylan";

            var dealImport = new DealsImportModel
            {
                SubscriberId = subscriberId,
                Shipper = row?.Cell("D")?.Value?.ToString(),
                Consignee = row?.Cell("E")?.Value?.ToString(),
                DealOwnerName = row?.Cell("B")?.Value?.ToString(),
                Commodity = row?.Cell("J")?.Value?.ToString(),
                Origin = row?.Cell("H")?.Value?.ToString(),
                Destination = row?.Cell("I")?.Value?.ToString(),
                DealStatus = row?.Cell("O")?.Value?.ToString()
            };
            var dealValue = 0.0f;
            if (!string.IsNullOrWhiteSpace(row?.Cell("N")?.Value.ToString()))
            {
                dealValue = float.Parse(row?.Cell("N")?.Value?.ToString().Replace("$", "").Trim());
            }
            dealImport.DealValue = dealValue;
            if (row?.Cell("K")?.Value?.ToString() != "")
            {
                dealImport.DecisionDate = Convert.ToDateTime(row?.Cell("K")?.Value?.ToString());
            }
            if (row?.Cell("P")?.Value?.ToString() != "")
            {
                dealImport.LastUpdatedDate = Convert.ToDateTime(row?.Cell("P")?.Value?.ToString());
            }

            // servcice
            switch (row?.Cell("F")?.Value?.ToString())
            {
                case "Air":
                    dealImport.Service = "Air"; break;
                case "Ocean":
                    dealImport.Service = "Ocean"; break;
                case "Truck":
                    dealImport.Service = "Road"; break;
                case "Brokerage":
                    dealImport.Service = "Brokerage"; break;
                case "Warehouse-Distribution":
                    dealImport.Service = "Logistics"; break;
                case "Triangle Service":
                    dealImport.Service = "Logistics"; break;
                case "Break Bulk - Ocean":
                    dealImport.Service = "Ocean"; break;
                default:
                    break;
            }

            // comments
            dealImport.Comments += "Id : " + row?.Cell("A")?.Value?.ToString() + " | ";
            dealImport.Comments += "Account Exec : " + row?.Cell("B")?.Value?.ToString() + " | ";
            dealImport.Comments += "Routing By : " + row?.Cell("C")?.Value?.ToString() + " | ";
            dealImport.Comments += "Shipper : " + row?.Cell("D")?.Value?.ToString() + " | ";
            dealImport.Comments += "Consignee : " + row?.Cell("E")?.Value?.ToString() + " | ";
            dealImport.Comments += "Mode : " + row?.Cell("F")?.Value?.ToString() + " | ";
            dealImport.Comments += "Lane : " + row?.Cell("G")?.Value?.ToString() + " | ";
            dealImport.Comments += "Origin : " + row?.Cell("H")?.Value?.ToString() + " | ";
            dealImport.Comments += "Destination : " + row?.Cell("I")?.Value?.ToString() + " | ";
            dealImport.Comments += "Commodity : " + row?.Cell("J")?.Value?.ToString() + " | ";
            dealImport.Comments += "Anticipated Closing:  " + row?.Cell("K")?.Value?.ToString() + " | ";
            dealImport.Comments += "Closing Probability(%) : " + row?.Cell("L")?.Value?.ToString() + " | ";
            dealImport.Comments += "Date Closed : " + row?.Cell("M")?.Value?.ToString() + " | ";
            dealImport.Comments += "Value: " + row?.Cell("N")?.Value?.ToString() + " | ";
            dealImport.Comments += "Status : " + row?.Cell("O")?.Value?.ToString() + " | ";
            dealImport.Comments += "Last Modified : " + row?.Cell("P")?.Value?.ToString() + " | ";
            dealImport.Comments += "Expiry Date : " + row?.Cell("Q")?.Value?.ToString() + " | ";
            dealImport.Comments += "Has Mode Commission : " + row?.Cell("R")?.Value?.ToString() + " | ";



            return dealImport;

        }


        public int DealId { get; set; }
        public int SubscriberId { get; set; }
        public string Shipper { get; set; }
        public string Consignee { get; set; }
        public string Service { get; set; }
        public string DealOwnerName { get; set; }
        public string Commodity { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public float DealValue { get; set; }
        public DateTime? DecisionDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string Comments { get; set; }
        public string DealStatus { get; set; }

    }
}