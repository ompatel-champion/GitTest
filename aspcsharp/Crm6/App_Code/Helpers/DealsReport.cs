using ClosedXML.Excel;
using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Crm6.App_Code.Helpers;
using Microsoft.WindowsAzure.Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Helpers
{
    public class DealsReport
    {

        /// <summary>
        /// get report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public DealsReportResponse GetDealsForReport(DealsReportFilters filters)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            if (filters.Campaigns != null && filters.Campaigns.Count > 0)
            {
                var campaignStr = filters.Campaigns[0];
                var campaign = context.Campaigns.FirstOrDefault(t => t.CampaignName == campaignStr && t.SubscriberId == filters.SubscriberId);
                if (campaign != null && campaign.CampaignType == "Global")
                {
                    return GetReportInAllSubscribers(filters);
                }
            }

            return GetDealsForReportForSubscriber(filters);
        }



        public DealsReportResponse GetReportInAllSubscribers(DealsReportFilters filters)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
            var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                        .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                        .Select(s => new { s.LinkedSubscriberId, s.DataCenter })
                        .ToList();

            var response = new DealsReportResponse
            {
                CurrentPage = filters.CurrentPage,
                Deals = new List<DealsReportItem>()
            };
            // set currency code
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var user = context.Users.Where(u => u.UserId == filters.UserId).FirstOrDefault();
            if (string.IsNullOrEmpty(filters.CurrencyCode))
            {
                filters.CurrencyCode = user.CurrencyCode;
            }
            var currencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

            foreach (var linkedSubscriber in linkedSubscribers)
            {
                var connection = LoginUser.GetConnectionForDataCenter(linkedSubscriber.DataCenter);
                context = new DbFirstFreightDataContext(connection);

                try
                {
                    // apply filters
                    var locationCodes = new List<string>();
                    var dealIds = new List<int>();


                    // construct the SQL statement
                    var sqlStr = "SELECT * From Deals INNER JOIN Users ON Deals.DealOwnerId = Users.UserId ";
                    sqlStr += "WHERE Deals.Deleted = 0 AND Users.Deleted = 0 ";


                    if (filters.SubscriberId > 0)
                    {
                        sqlStr += " AND Deals.SubscriberId = " + linkedSubscriber.LinkedSubscriberId;
                    }

                    if (filters.SalesStages != null && filters.SalesStages.Count > 0)
                    {
                        if (filters.SalesStages[0] == "All")
                        {
                            // do nothing
                        }
                        else
                        {
                            sqlStr += " AND Deals.SalesStageName IN ('" + string.Join("','", filters.SalesStages) + "')";
                        }
                    }


                    // filter by district codes id passed
                    if (filters.DistrictCodes != null && filters.DistrictCodes.Count > 0)
                    {
                        var locCodes = context.Locations.Where(t => filters.DistrictCodes.Contains(t.DistrictCode) && t.LocationCode != "").Select(t => t.LocationCode).ToList();
                        if (locCodes.Count > 0)
                            sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", locCodes) + "')";
                    }

                    // filter by country codes id passed
                    if (filters.CountryNames != null && filters.CountryNames.Count > 0)
                    {
                        var locCodes = context.Locations.Where(t => filters.CountryNames.Contains(t.CountryName) && t.LocationCode != "")
                                                                   .Select(t => t.LocationCode).ToList();
                        if (locCodes.Count > 0)
                            sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", locCodes) + "')";
                    }

                    // filter location code
                    if (filters.LocationCodes != null && filters.LocationCodes.Count > 0)
                    {
                        sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", filters.LocationCodes) + "')";
                    }


                    // Date Type and related date fields criteria
                    if (!string.IsNullOrEmpty(filters.DateFrom))
                    {
                        var dateFrom = Convert.ToDateTime(filters.DateFrom);
                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        switch (filters.DateType)
                        {
                            case "DecisionDate":
                                sqlStr += " AND Deals.DecisionDate > '" + dateFrom.ToString("s") + "'";
                                break;
                            case "FirstShipment":
                                sqlStr += " AND Deals.EstimatedStartDate >= '" + dateFrom.ToString("s") + "'";
                                break;
                            case "ContractEnd":
                                sqlStr += " AND Deals.ContractEndDate >= '" + dateFrom.ToString("s") + "'";
                                break;
                            case "CreatedDate":
                                sqlStr += " AND Deals.CreatedDate >= '" + dateFrom.ToString("s") + "'";
                                break;
                            case "UpdatedDate":
                                sqlStr += " AND Deals.LastUpdate >= '" + dateFrom.ToString("s") + "'";
                                break;
                            case "DateLost":
                                sqlStr += " AND Deals.DateLost >= '" + dateFrom.ToString("s") + "'";
                                break;
                            case "DateWon":
                                sqlStr += " AND Deals.DateWon >= '" + dateFrom.ToString("s") + "'";
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(filters.DateTo))
                    {
                        var dateTo = Convert.ToDateTime(filters.DateTo);
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        switch (filters.DateType)
                        {
                            case "DecisionDate":
                                sqlStr += " AND Deals.DecisionDate <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "FirstShipment":
                                sqlStr += " AND Deals.EstimatedStartDate <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "ContractEnd":
                                sqlStr += " AND Deals.ContractEndDate <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "CreatedDate":
                                sqlStr += " AND Deals.CreatedDate <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "UpdatedDate":
                                sqlStr += " AND Deals.LastUpdate <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "DateLost":
                                sqlStr += " AND Deals.DateLost <= '" + dateTo.ToString("s") + "'";
                                break;
                            case "DateWon":
                                sqlStr += " AND Deals.DateWon <= '" + dateTo.ToString("s") + "'";
                                break;
                        }
                    }

                    // Service Types
                    if (filters.ServiceTypes != null && filters.ServiceTypes.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var serviceType in filters.ServiceTypes)
                        {
                            sqlStr += " Deals.Services LIKE '%" + serviceType + "%'";
                            if (count < filters.ServiceTypes.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // Deal Types
                    if (filters.DealTypes != null && filters.DealTypes.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var dealType in filters.DealTypes)
                        {
                            sqlStr += " Deals.DealType LIKE '%" + dealType + "%'";
                            if (count < filters.DealTypes.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // Industries
                    if (filters.Industries != null && filters.Industries.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var industry in filters.Industries)
                        {
                            sqlStr += " Deals.Industry LIKE '%" + industry + "%'";
                            if (count < filters.Industries.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // Campaigns
                    if (filters.Campaigns != null && filters.Campaigns.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var campaign in filters.Campaigns)
                        {
                            sqlStr += " Deals.Campaign LIKE '%" + campaign + "%'";
                            if (count < filters.Campaigns.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // competitors
                    if (filters.Competitors != null && filters.Competitors.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var competitor in filters.Competitors)
                        {
                            sqlStr += " Deals.Competitors LIKE '%" + competitor + "%'";
                            if (count < filters.Competitors.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }



                    // Origin Countries
                    if (filters.OriginCountries != null && filters.OriginCountries.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var dcc in filters.OriginCountries)
                        {
                            sqlStr += " Deals.OrignCountries LIKE '%" + dcc + "%'";
                            if (count < filters.OriginCountries.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";

                    }

                    // Origin Locations
                    if (filters.OriginLocations != null && filters.OriginLocations.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var olc in filters.OriginLocations)
                        {
                            sqlStr += " Deals.OrignLocations LIKE '%" + olc.Trim() + "%'";
                            if (count < filters.OriginLocations.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // Destination Countries
                    if (filters.DestinationCountries != null && filters.DestinationCountries.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var dcc in filters.DestinationCountries)
                        {
                            sqlStr += " Deals.DestinationCountries LIKE '%" + dcc + "%'";
                            if (count < filters.DestinationCountries.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // Destination Locations
                    if (filters.DestinationLocations != null && filters.DestinationLocations.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var dlc in filters.DestinationLocations)
                        {
                            sqlStr += " Deals.DestinationLocations LIKE '%" + dlc.Trim() + "%'";
                            if (count < filters.DestinationLocations.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // revenue
                    if (filters.IsSpotDeals)
                    {
                        sqlStr += " AND Deals.RevenueUSDSpot > 0 ";
                    }
                    else
                    {
                        sqlStr += " AND Deals.RevenueUSD > 0 ";
                    }


                    // keyword
                    if (!string.IsNullOrEmpty(filters.Keyword))
                    {
                        filters.Keyword = filters.Keyword.ToLower();
                        sqlStr += " AND (Deals.DealName LIKE '%" + filters.Keyword + "%'  OR Deals.DealDescription LIKE '%" + filters.Keyword + "%' " +
                                  " OR Deals.CompanyName LIKE '%" + filters.Keyword + "%' OR Deals.PrimaryContactName LIKE '%" + filters.Keyword + "%'  ) ";
                    }

                    // report sort order
                    if (!string.IsNullOrEmpty(filters.SortBy))
                    {
                        switch (filters.SortBy.ToLower())
                        {
                            case "createddate asc":
                                sqlStr += " ORDER BY Deals.CreatedDate ASC ";
                                break;
                            case "createddate desc":
                                sqlStr += " ORDER BY Deals.CreatedDate DESC ";
                                break;
                            case "dealname asc":
                                sqlStr += " ORDER BY Deals.DealName ASC ";
                                break;
                            case "dealname desc":
                                sqlStr += " ORDER BY Deals.DealName DESC ";
                                break;
                            case "companyname asc":
                                sqlStr += " ORDER BY Deals.CompanyName ASC ";
                                break;
                            case "companyname desc":
                                sqlStr += " ORDER BY Deals.CompanyName DESC ";
                                break;
                            case "lastactivitydate asc":
                                sqlStr += " ORDER BY Deals.LastActivityDate ASC ";
                                break;
                            case "lastactivitydate desc":
                                sqlStr += " ORDER BY Deals.LastActivityDate DESC ";
                                break;
                            case "nextactivitydate asc":
                                sqlStr += " ORDER BY Deals.NextActivityDate ASC ";
                                break;
                            case "nextactivitydate desc":
                                sqlStr += " ORDER BY Deals.NextActivityDate DESC ";
                                break;
                        }
                    }

                    using (var conn = new SqlConnection(connection))
                    {
                        conn.Open();
                        // populate dataReader
                        var cmd = new SqlCommand(sqlStr, conn);
                        SqlDataReader dataReader = cmd.ExecuteReader();

                        // check if no records
                        if (!dataReader.HasRows)
                        {
                            conn.Close();
                            continue;
                        }

                        System.Data.DataTable dt = new System.Data.DataTable();
                        dt.Load(dataReader);

                        // iterate though salesStage totals records
                        foreach (DataRow dr in dt.Rows)
                        {
                            var dealReportItem = new DealsReportItem();
                            dealReportItem.DealId = int.Parse(dr["DealId"].ToString());
                            dealReportItem.SubscriberId = int.Parse(dr["SubscriberId"].ToString());
                            dealReportItem.CompanyName = dr["CompanyName"] is DBNull ? "" : dr["CompanyName"].ToString();

                            dealReportItem.CompanyId = dr["CompanyId"] is DBNull ? 0 : int.Parse(dr["CompanyId"].ToString());
                            dealReportItem.DealName = dr["DealName"] is DBNull ? "" : dr["DealName"].ToString();
                            dealReportItem.DealType = dr["DealType"] is DBNull ? "" : dr["DealType"].ToString();

                            dealReportItem.DateContractEnd = dr["ContractEndDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["ContractEndDate"].ToString()), "dd-MMM-yy");
                            dealReportItem.DateCreated = FormatDate(Convert.ToDateTime(dr["CreatedDate"].ToString()), "dd-MMM-yy");

                            dealReportItem.DateProposalDue = dr["DateProposalDue"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateProposalDue"].ToString()), "dd-MMM-yy");
                            dealReportItem.DateUpdated = dr["LastUpdate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["LastUpdate"].ToString()), "dd-MMM-yy");
                            dealReportItem.DateDecision = dr["DecisionDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DecisionDate"].ToString()), "dd-MMM-yy");
                            dealReportItem.DateFirstShipment = dr["EstimatedStartDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["EstimatedStartDate"].ToString()), "dd-MMM-yy");

                            dealReportItem.DateLost = dr["DateLost"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateLost"].ToString()), "dd-MMM-yy");
                            dealReportItem.DateWon = dr["DateWon"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateWon"].ToString()), "dd-MMM-yy");
                            dealReportItem.LastActivityDate = dr["LastActivityDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["LastActivityDate"].ToString()), "dd-MMM-yy");
                            dealReportItem.NextActivityDate = dr["NextActivityDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["NextActivityDate"].ToString()), "dd-MMM-yy");


                            dealReportItem.Industry = dr["Industry"] is DBNull ? "" : dr["Industry"].ToString();
                            dealReportItem.SalesRepName = dr["SalesRepName"] is DBNull ? "" : dr["SalesRepName"].ToString();
                            dealReportItem.SalesStage = dr["SalesStageName"] is DBNull ? "" : dr["SalesStageName"].ToString();
                            dealReportItem.LocationName = dr["LocationName"] is DBNull ? "" : dr["LocationName"].ToString();
                            dealReportItem.ReasonWonLost = dr["ReasonWonLost"] is DBNull || dr["ReasonWonLost"].ToString() == "Select Reason" ? "" : dr["ReasonWonLost"].ToString();


                            // origins 
                            dealReportItem.Origins = dr["OrignLocations"] is DBNull ? "" : dr["OrignLocations"].ToString();
                            dealReportItem.OriginCountries = dr["OrignCountries"] is DBNull ? "" : dr["OrignCountries"].ToString();

                            // destinations 
                            dealReportItem.Destinations = dr["DestinationLocations"] is DBNull ? "" : dr["DestinationLocations"].ToString();
                            dealReportItem.DestinationCountries = dr["DestinationCountries"] is DBNull ? "" : dr["DestinationCountries"].ToString();

                            dealReportItem.Comments = dr["Comments"] is DBNull ? "" : dr["Comments"].ToString();

                            // volumes
                            dealReportItem.CBMs = (int)double.Parse(dr["CBMs"].ToString());
                            dealReportItem.LBs = (int)double.Parse(dr["Lbs"].ToString());
                            dealReportItem.Tonnes = (int)double.Parse(dr["Tonnes"].ToString());
                            dealReportItem.TEUs = (int)double.Parse(dr["TEUs"].ToString());
                            dealReportItem.KGs = (int)double.Parse(dr["Kgs"].ToString());
                            dealReportItem.SpotCBMs = (int)double.Parse(dr["CBMsSpot"].ToString());
                            dealReportItem.SpotLBs = (int)double.Parse(dr["LbsSpot"].ToString());
                            dealReportItem.SpotTonnes = (int)double.Parse(dr["TonnesSpot"].ToString());
                            dealReportItem.SpotTEUs = (int)double.Parse(dr["TEUsSpot"].ToString());
                            dealReportItem.SpotKGs = (int)double.Parse(dr["KgsSpot"].ToString());

                            if (filters.ShippingFrquency == "Per Year")
                            {
                                dealReportItem.CBMs = (int)double.Parse(dr["CBMs"].ToString()) * 12;
                                dealReportItem.LBs = (int)double.Parse(dr["Lbs"].ToString()) * 12;
                                dealReportItem.Tonnes = (int)double.Parse(dr["Tonnes"].ToString()) * 12;
                                dealReportItem.TEUs = (int)double.Parse(dr["TEUs"].ToString()) * 12;
                                dealReportItem.KGs = (int)double.Parse(dr["Kgs"].ToString()) * 12;
                                dealReportItem.SpotCBMs = (int)double.Parse(dr["CBMsSpot"].ToString()) * 12;
                                dealReportItem.SpotLBs = (int)double.Parse(dr["LbsSpot"].ToString()) * 12;
                                dealReportItem.SpotTonnes = (int)double.Parse(dr["TonnesSpot"].ToString()) * 12;
                                dealReportItem.SpotTEUs = (int)double.Parse(dr["TEUsSpot"].ToString()) * 12;
                                dealReportItem.SpotKGs = (int)double.Parse(dr["KgsSpot"].ToString());
                            }


                            // consignee and shipper
                            dealReportItem.ConsigneeNames = dr["ConsigneeNames"] is DBNull ? "" : dr["ConsigneeNames"].ToString();
                            dealReportItem.ShipperNames = dr["ShipperNames"] is DBNull ? "" : dr["ShipperNames"].ToString();
                            dealReportItem.UpdatedBy = dr["UpdateUserName"] is DBNull ? "" : dr["UpdateUserName"].ToString();
                            // Services 
                            dealReportItem.Services = dr["Services"] is DBNull ? "" : dr["Services"].ToString();

                            // revenue profit amounts 
                            var revenue = double.Parse(dr["RevenueUSD"].ToString());
                            var profit = double.Parse(dr["ProfitUSD"].ToString());
                            var spotRevenue = double.Parse(dr["RevenueUSDSpot"].ToString());
                            var spotProfit = double.Parse(dr["ProfitUSDSpot"].ToString());
                            var userCurrencyProfit = profit;
                            var userCurrencyRevenue = revenue;
                            var userCurrencyProfitSpot = spotProfit;
                            var userCurrencyRevenueSpot = spotRevenue;
                            if (user.CurrencyCode != "USD")
                            {
                                if (filters.IsSpotDeals)
                                {
                                    userCurrencyProfitSpot = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, spotProfit);
                                    userCurrencyRevenueSpot = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, spotRevenue);
                                }
                                else
                                {
                                    userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                                    userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                                }
                            }

                            // Per Year reporting
                            if (filters.ShippingFrquency == "Per Year")
                            {
                                userCurrencyProfit = userCurrencyProfit * 12;
                                userCurrencyRevenue = userCurrencyRevenue * 12;
                                userCurrencyRevenueSpot = userCurrencyRevenueSpot * 12;
                                userCurrencyProfitSpot = userCurrencyProfitSpot * 12;
                            }

                            dealReportItem.Revenue = userCurrencyRevenue;
                            dealReportItem.Profit = userCurrencyProfit;
                            dealReportItem.SpotRevenue = userCurrencyRevenueSpot;
                            dealReportItem.SpotProfit = userCurrencyProfitSpot;
                            dealReportItem.CurrencyCode = filters.CurrencyCode;
                            dealReportItem.CurrencySymbol = currencySymbol;

                            // profit/revenue percentage
                            if (filters.IsSpotDeals)
                            {
                                dealReportItem.ProfitRevenuePercentage = Math.Round(userCurrencyProfitSpot / userCurrencyRevenueSpot * 100, 2);
                            }
                            else
                            {
                                dealReportItem.ProfitRevenuePercentage = Math.Round(userCurrencyProfit / userCurrencyRevenue * 100, 2);
                            }
                            response.Deals.Add(dealReportItem);

                            conn.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    var error = new Crm6.App_Code.Shared.WebAppError
                    {
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        RoutineName = "GetDealsForReport",
                        PageCalledFrom = "Helper/DealsReport",
                        SubscriberId = filters.SubscriberId,
                        SubscriberName = "",
                        ErrorMessage = ex.ToString(),
                        UserId = filters.UserId
                    };
                    new Logging().LogWebAppError(error);
                }
            }


            response.RecordCount = response.Deals.Count;
            response.ExcelUri = CreateExcel(response.Deals, filters);

            return response;

        }



        public DealsReportResponse GetDealsForReportForSubscriber(DealsReportFilters filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var response = new DealsReportResponse
            {
                CurrentPage = filters.CurrentPage,
                Deals = new List<DealsReportItem>()
            };

            try
            {
                // set currency code
                var user = context.Users.Where(u => u.UserId == filters.UserId).FirstOrDefault();
                if (user == null)
                {
                    return response;
                }

                if (string.IsNullOrEmpty(filters.CurrencyCode))
                {
                    filters.CurrencyCode = user.CurrencyCode;
                }
                var currencySymbol = new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode);

                // apply filters
                var locationCodes = new List<string>();
                var dealIds = new List<int>();

                // construct the SQL statement
                var sqlStr = "SELECT * From Deals INNER JOIN Users ON Deals.DealOwnerId = Users.UserId ";
                sqlStr += "WHERE Deals.Deleted = 0 AND Users.Deleted = 0 ";


                if (filters.SubscriberId > 0)
                {
                    sqlStr += " AND Deals.SubscriberId = " + filters.SubscriberId;
                }
                if (filters.UserIds != null && filters.UserIds.Count > 0)
                {
                    sqlStr += " AND Deals.DealOwnerId IN (" + string.Join(",", filters.UserIds) + ")";
                }

                if (filters.SalesStages != null && filters.SalesStages.Count > 0)
                {
                    if (filters.SalesStages[0] == "All")
                    {
                        // do nothing
                    }
                    else
                    {
                        sqlStr += " AND Deals.SalesStageName IN ('" + string.Join("','", filters.SalesStages) + "')";
                    }
                }

                // UserId
                if (filters.UserId > 0)
                {
                    // get sales manager user's location codes
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        if (filters.UserIds == null || filters.UserIds.Count == 0)
                        {
                            // get user Ids
                            var managingUserIds = (from t in context.LinkUserToManagers
                                                   where t.ManagerUserId == filters.UserId && !t.Deleted
                                                   select t.UserId).Distinct().ToList();
                            // add managing user ids
                            filters.UserIds = managingUserIds;
                            // add user
                            filters.UserIds.Add(user.UserId);
                        }
                    }

                    if (!string.IsNullOrEmpty(user.UserRoles))
                    {
                        if (user.UserRoles.Contains("Region Manager") || user.UserRoles.Contains("CRM Admin"))
                        {
                            if (!string.IsNullOrEmpty(user.RegionName))
                            {
                                // this user is a region manager, get all the deals for the region  
                                sqlStr += " AND Users.SubscriberId = " + filters.SubscriberId + " AND Users.RegionName = '" + user.RegionName + "'";
                            }
                        }
                        else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                        {
                            if (!string.IsNullOrEmpty(user.CountryCode))
                            {
                                // this user is a country manager, get all the deals for the country
                                sqlStr += " AND Users.SubscriberId = " + filters.SubscriberId + " AND Users.CountryCode = '" + user.CountryCode + "'";
                            }
                        }
                        else if (user.UserRoles.Contains("District Manager"))
                        {
                            if (!string.IsNullOrEmpty(user.DistrictCode))
                            {
                                var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                                if (district != null)
                                {
                                    // this user is a district manager, get all the deals for the district
                                    sqlStr += " AND Users.SubscriberId = " + filters.SubscriberId + " AND Users.DistrictCode = '" + user.DistrictCode + "'";
                                }
                            }
                        }
                        else if (user.UserRoles.Contains("Location Manager"))
                        {
                            if (!string.IsNullOrEmpty(user.LocationCode))
                            {
                                // this user is a location manager, get all the deals for the location
                                sqlStr += " AND Users.SubscriberId = " + filters.SubscriberId + " AND Users.LocationCode = '" + user.LocationCode + "'";
                            }
                        }
                        else
                        {

                            sqlStr += " AND Users.SubscriberId = " + filters.SubscriberId + " AND Users.UserId = " + filters.UserId;
                        }
                    }
                }


                // location codes
                if (locationCodes.Count > 0)
                {
                    sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", locationCodes) + "')";
                }

                // filter by district codes id passed
                if (filters.DistrictCodes != null && filters.DistrictCodes.Count > 0)
                {
                    var locCodes = context.Locations.Where(t => filters.DistrictCodes.Contains(t.DistrictCode) && t.LocationCode != "" && t.SubscriberId == filters.SubscriberId).Select(t => t.LocationCode).ToList();
                    if (locCodes.Count > 0)
                        sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", locCodes) + "')";
                }

                // filter by country codes id passed
                if (filters.CountryNames != null && filters.CountryNames.Count > 0)
                {
                    var locCodes = context.Locations.Where(t => filters.CountryNames.Contains(t.CountryName) && t.LocationCode != "" && t.SubscriberId == filters.SubscriberId)
                                                               .Select(t => t.LocationCode).ToList();
                    if (locCodes.Count > 0)
                        sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", locCodes) + "') AND  Users.CountryName IN ('" + string.Join("','", filters.CountryNames) + "')";
                }

                // filter location code
                if (filters.LocationCodes != null && filters.LocationCodes.Count > 0)
                {
                    sqlStr += " AND Deals.LocationCode IN ('" + string.Join("','", filters.LocationCodes) + "')";
                }


                // Date Type and related date fields criteria
                if (!string.IsNullOrEmpty(filters.DateFrom))
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                    switch (filters.DateType)
                    {
                        case "DecisionDate":
                            sqlStr += " AND Deals.DecisionDate > '" + dateFrom.ToString("s") + "'";
                            break;
                        case "FirstShipment":
                            sqlStr += " AND Deals.EstimatedStartDate >= '" + dateFrom.ToString("s") + "'";
                            break;
                        case "ContractEnd":
                            sqlStr += " AND Deals.ContractEndDate >= '" + dateFrom.ToString("s") + "'";
                            break;
                        case "CreatedDate":
                            sqlStr += " AND Deals.CreatedDate >= '" + dateFrom.ToString("s") + "'";
                            break;
                        case "UpdatedDate":
                            sqlStr += " AND Deals.LastUpdate >= '" + dateFrom.ToString("s") + "'";
                            break;
                        case "DateLost":
                            sqlStr += " AND Deals.DateLost >= '" + dateFrom.ToString("s") + "'";
                            break;
                        case "DateWon":
                            sqlStr += " AND Deals.DateWon >= '" + dateFrom.ToString("s") + "'";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(filters.DateTo))
                {
                    var dateTo = Convert.ToDateTime(filters.DateTo);
                    dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                    switch (filters.DateType)
                    {
                        case "DecisionDate":
                            sqlStr += " AND Deals.DecisionDate <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "FirstShipment":
                            sqlStr += " AND Deals.EstimatedStartDate <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "ContractEnd":
                            sqlStr += " AND Deals.ContractEndDate <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "CreatedDate":
                            sqlStr += " AND Deals.CreatedDate <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "UpdatedDate":
                            sqlStr += " AND Deals.LastUpdate <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "DateLost":
                            sqlStr += " AND Deals.DateLost <= '" + dateTo.ToString("s") + "'";
                            break;
                        case "DateWon":
                            sqlStr += " AND Deals.DateWon <= '" + dateTo.ToString("s") + "'";
                            break;
                    }
                }

                // Service Types
                if (filters.ServiceTypes != null && filters.ServiceTypes.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var serviceType in filters.ServiceTypes)
                    {
                        sqlStr += " Deals.Services LIKE '%" + serviceType + "%'";
                        if (count < filters.ServiceTypes.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // Deal Types
                if (filters.DealTypes != null && filters.DealTypes.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var dealType in filters.DealTypes)
                    {
                        sqlStr += " Deals.DealType LIKE '%" + dealType + "%'";
                        if (count < filters.DealTypes.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // Industries
                if (filters.Industries != null && filters.Industries.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var industry in filters.Industries)
                    {
                        sqlStr += " Deals.Industry LIKE '%" + industry + "%'";
                        if (count < filters.Industries.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // Campaigns
                if (filters.Campaigns != null && filters.Campaigns.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var campaign in filters.Campaigns)
                    {
                        sqlStr += " Deals.Campaign LIKE '%" + campaign + "%'";
                        if (count < filters.Campaigns.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // competitors
                if (filters.Competitors != null && filters.Competitors.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var competitor in filters.Competitors)
                    {
                        sqlStr += " Deals.Competitors LIKE '%" + competitor + "%'";
                        if (count < filters.Competitors.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }



                // Origin Countries
                if (filters.OriginCountries != null && filters.OriginCountries.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var dcc in filters.OriginCountries)
                    {
                        sqlStr += " Deals.OrignCountries LIKE '%" + dcc + "%'";
                        if (count < filters.OriginCountries.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";

                }

                // Origin Locations
                if (filters.OriginLocations != null && filters.OriginLocations.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var olc in filters.OriginLocations)
                    {
                        sqlStr += " Deals.OrignLocations LIKE '%" + olc.Trim() + "%'";
                        if (count < filters.OriginLocations.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // Destination Countries
                if (filters.DestinationCountries != null && filters.DestinationCountries.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var dcc in filters.DestinationCountries)
                    {
                        sqlStr += " Deals.DestinationCountries LIKE '%" + dcc + "%'";
                        if (count < filters.DestinationCountries.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // Destination Locations
                if (filters.DestinationLocations != null && filters.DestinationLocations.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var dlc in filters.DestinationLocations)
                    {
                        sqlStr += " Deals.DestinationLocations LIKE '%" + dlc.Trim() + "%'";
                        if (count < filters.DestinationLocations.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";
                }

                // revenue
                if (filters.IsSpotDeals)
                {
                    sqlStr += " AND Deals.RevenueUSDSpot > 0 ";
                }
                else
                {
                    sqlStr += " AND Deals.RevenueUSDSpot = 0 ";
                }


                // keyword
                if (!string.IsNullOrEmpty(filters.Keyword))
                {
                    filters.Keyword = filters.Keyword.ToLower();
                    sqlStr += " AND (Deals.DealName LIKE '%" + filters.Keyword + "%'  OR Deals.DealDescription LIKE '%" + filters.Keyword + "%' " +
                              " OR Deals.CompanyName LIKE '%" + filters.Keyword + "%' OR Deals.PrimaryContactName LIKE '%" + filters.Keyword + "%'  ) ";
                }

                // report sort order
                if (!string.IsNullOrEmpty(filters.SortBy))
                {
                    switch (filters.SortBy.ToLower())
                    {
                        case "createddate asc":
                            sqlStr += " ORDER BY Deals.CreatedDate ASC ";
                            break;
                        case "createddate desc":
                            sqlStr += " ORDER BY Deals.CreatedDate DESC ";
                            break;
                        case "dealname asc":
                            sqlStr += " ORDER BY Deals.DealName ASC ";
                            break;
                        case "dealname desc":
                            sqlStr += " ORDER BY Deals.DealName DESC ";
                            break;
                        case "companyname asc":
                            sqlStr += " ORDER BY Deals.CompanyName ASC ";
                            break;
                        case "companyname desc":
                            sqlStr += " ORDER BY Deals.CompanyName DESC ";
                            break;
                        case "lastactivitydate asc":
                            sqlStr += " ORDER BY Deals.LastActivityDate ASC ";
                            break;
                        case "lastactivitydate desc":
                            sqlStr += " ORDER BY Deals.LastActivityDate DESC ";
                            break;
                        case "nextactivitydate asc":
                            sqlStr += " ORDER BY Deals.NextActivityDate ASC ";
                            break;
                        case "nextactivitydate desc":
                            sqlStr += " ORDER BY Deals.NextActivityDate DESC ";
                            break;
                    }
                }

                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();
                    // populate dataReader
                    var cmd = new SqlCommand(sqlStr, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    // check if no records
                    if (!dataReader.HasRows) { return response; }

                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Load(dataReader);

                    // iterate though salesStage totals records
                    foreach (DataRow dr in dt.Rows)
                    {
                        var dealReportItem = new DealsReportItem();
                        dealReportItem.DealId = int.Parse(dr["DealId"].ToString());
                        dealReportItem.SubscriberId = int.Parse(dr["SubscriberId"].ToString());
                        dealReportItem.CompanyName = dr["CompanyName"] is DBNull ? "" : dr["CompanyName"].ToString();

                        dealReportItem.CompanyId = dr["CompanyId"] is DBNull ? 0 : int.Parse(dr["CompanyId"].ToString());
                        dealReportItem.DealName = dr["DealName"] is DBNull ? "" : dr["DealName"].ToString();
                        dealReportItem.DealType = dr["DealType"] is DBNull ? "" : dr["DealType"].ToString();

                        dealReportItem.DateContractEnd = dr["ContractEndDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["ContractEndDate"].ToString()), "dd-MMM-yy");
                        dealReportItem.DateCreated = FormatDate(Convert.ToDateTime(dr["CreatedDate"].ToString()), "dd-MMM-yy");

                        dealReportItem.DateProposalDue = dr["DateProposalDue"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateProposalDue"].ToString()), "dd-MMM-yy");
                        dealReportItem.DateUpdated = dr["LastUpdate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["LastUpdate"].ToString()), "dd-MMM-yy");
                        dealReportItem.DateDecision = dr["DecisionDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DecisionDate"].ToString()), "dd-MMM-yy");
                        dealReportItem.DateFirstShipment = dr["EstimatedStartDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["EstimatedStartDate"].ToString()), "dd-MMM-yy");

                        dealReportItem.DateLost = dr["DateLost"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateLost"].ToString()), "dd-MMM-yy");
                        dealReportItem.DateWon = dr["DateWon"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["DateWon"].ToString()), "dd-MMM-yy");
                        dealReportItem.LastActivityDate = dr["LastActivityDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["LastActivityDate"].ToString()), "dd-MMM-yy");
                        dealReportItem.NextActivityDate = dr["NextActivityDate"] is DBNull ? "" : FormatDate(Convert.ToDateTime(dr["NextActivityDate"].ToString()), "dd-MMM-yy");


                        dealReportItem.Industry = dr["Industry"] is DBNull ? "" : dr["Industry"].ToString();
                        dealReportItem.SalesRepName = dr["SalesRepName"] is DBNull ? "" : dr["SalesRepName"].ToString();
                        dealReportItem.SalesStage = dr["SalesStageName"] is DBNull ? "" : dr["SalesStageName"].ToString();
                        dealReportItem.LocationName = dr["LocationName"] is DBNull ? "" : dr["LocationName"].ToString();
                        dealReportItem.ReasonWonLost = dr["ReasonWonLost"] is DBNull || dr["ReasonWonLost"].ToString() == "Select Reason" ? "" : dr["ReasonWonLost"].ToString();


                        // origins 
                        dealReportItem.Origins = dr["OrignLocations"] is DBNull ? "" : dr["OrignLocations"].ToString();
                        dealReportItem.OriginCountries = dr["OrignCountries"] is DBNull ? "" : dr["OrignCountries"].ToString();

                        // destinations 
                        dealReportItem.Destinations = dr["DestinationLocations"] is DBNull ? "" : dr["DestinationLocations"].ToString();
                        dealReportItem.DestinationCountries = dr["DestinationCountries"] is DBNull ? "" : dr["DestinationCountries"].ToString();

                        dealReportItem.Comments = dr["Comments"] is DBNull ? "" : dr["Comments"].ToString();

                        // volumes
                        dealReportItem.CBMs = (int)double.Parse(dr["CBMs"].ToString());
                        dealReportItem.LBs = (int)double.Parse(dr["Lbs"].ToString());
                        dealReportItem.Tonnes = (int)double.Parse(dr["Tonnes"].ToString());
                        dealReportItem.TEUs = (int)double.Parse(dr["TEUs"].ToString());
                        dealReportItem.KGs = (int)double.Parse(dr["Kgs"].ToString());
                        dealReportItem.SpotCBMs = (int)double.Parse(dr["CBMsSpot"].ToString());
                        dealReportItem.SpotLBs = (int)double.Parse(dr["LbsSpot"].ToString());
                        dealReportItem.SpotTonnes = (int)double.Parse(dr["TonnesSpot"].ToString());
                        dealReportItem.SpotTEUs = (int)double.Parse(dr["TEUsSpot"].ToString());
                        dealReportItem.SpotKGs = (int)double.Parse(dr["KgsSpot"].ToString());

                        if (filters.ShippingFrquency == "Per Year")
                        {
                            dealReportItem.CBMs = (int)double.Parse(dr["CBMs"].ToString()) * 12;
                            dealReportItem.LBs = (int)double.Parse(dr["Lbs"].ToString()) * 12;
                            dealReportItem.Tonnes = (int)double.Parse(dr["Tonnes"].ToString()) * 12;
                            dealReportItem.TEUs = (int)double.Parse(dr["TEUs"].ToString()) * 12;
                            dealReportItem.KGs = (int)double.Parse(dr["Kgs"].ToString()) * 12;
                            dealReportItem.SpotCBMs = (int)double.Parse(dr["CBMsSpot"].ToString()) * 12;
                            dealReportItem.SpotLBs = (int)double.Parse(dr["LbsSpot"].ToString()) * 12;
                            dealReportItem.SpotTonnes = (int)double.Parse(dr["TonnesSpot"].ToString()) * 12;
                            dealReportItem.SpotTEUs = (int)double.Parse(dr["TEUsSpot"].ToString()) * 12;
                            dealReportItem.SpotKGs = (int)double.Parse(dr["KgsSpot"].ToString());
                        }
                         
                        // consignee and shipper
                        dealReportItem.ConsigneeNames = dr["ConsigneeNames"] is DBNull ? "" : dr["ConsigneeNames"].ToString();
                        dealReportItem.ShipperNames = dr["ShipperNames"] is DBNull ? "" : dr["ShipperNames"].ToString();
                        dealReportItem.UpdatedBy = dr["UpdateUserName"] is DBNull ? "" : dr["UpdateUserName"].ToString();
                        // Services 
                        dealReportItem.Services = dr["Services"] is DBNull ? "" : dr["Services"].ToString();

                        // revenue profit amounts 
                        var revenue = double.Parse(dr["RevenueUSD"].ToString());
                        var profit = double.Parse(dr["ProfitUSD"].ToString());
                        var spotRevenue = double.Parse(dr["RevenueUSDSpot"].ToString());
                        var spotProfit = double.Parse(dr["ProfitUSDSpot"].ToString());
                        var userCurrencyProfit = profit;
                        var userCurrencyRevenue = revenue;
                        var userCurrencyProfitSpot = spotProfit;
                        var userCurrencyRevenueSpot = spotRevenue;
                        if (user.CurrencyCode != "USD")
                        {
                            if (filters.IsSpotDeals)
                            {
                                userCurrencyProfitSpot = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, spotProfit);
                                userCurrencyRevenueSpot = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, spotRevenue);
                            }
                            else
                            {
                                userCurrencyProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, profit);
                                userCurrencyRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, revenue);
                            }
                        }

                        // Per Year reporting
                        if (filters.ShippingFrquency == "Per Year")
                        {
                            userCurrencyProfit = userCurrencyProfit * 12;
                            userCurrencyRevenue = userCurrencyRevenue * 12;
                            userCurrencyRevenueSpot = userCurrencyRevenueSpot * 12;
                            userCurrencyProfitSpot = userCurrencyProfitSpot * 12;
                        }

                        dealReportItem.Revenue = userCurrencyRevenue;
                        dealReportItem.Profit = userCurrencyProfit;
                        dealReportItem.SpotRevenue = userCurrencyRevenueSpot;
                        dealReportItem.SpotProfit = userCurrencyProfitSpot;
                        dealReportItem.CurrencyCode = filters.CurrencyCode;
                        dealReportItem.CurrencySymbol = currencySymbol;

                        // profit/revenue percentage
                        if (filters.IsSpotDeals)
                        {
                            if (userCurrencyRevenueSpot > 0)
                            dealReportItem.ProfitRevenuePercentage = Math.Round(userCurrencyProfitSpot / userCurrencyRevenueSpot * 100, 2);
                        }
                        else
                        {
                            if (userCurrencyRevenue > 0)
                                dealReportItem.ProfitRevenuePercentage = Math.Round(userCurrencyProfit / userCurrencyRevenue * 100, 2);
                        }
                        response.Deals.Add(dealReportItem);

                        conn.Close();
                    }
                }
                response.RecordCount = response.Deals.Count;
                response.ExcelUri = CreateExcel(response.Deals, filters);
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "GetDealsForReport",
                    PageCalledFrom = "Helper/DealsReport",
                    SubscriberId = filters.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filters.UserId
                };
                new Logging().LogWebAppError(error);
            }

            // return deals report JSON
            return response;
        }



        public RevenueResponse GetDealRevenue(Deal deal, List<Lane> lanes, int userId, string currencyCode, string shippingFrquency, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var response = new RevenueResponse
            {
                Revenue = 0.0f,
                Profit = 0.0f,
                SpotRevenue = 0.0f,
                SpotProfit = 0.0f,
            };

            if (deal != null)
            {
                // yearly revenue total- for lanes where shipping frequency is 'Per Year'
                var yearlyRevenueTotal = 0.0d;
                var yearlyProfitTotal = 0.0d;

                // get lanes for the deal
                var yearLanes = lanes.Where(t => t.ShippingFrequency.Equals("Per Year") && !t.Deleted).ToList();
                foreach (var oLane in yearLanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;

                    var targetRevenueYearly = oLane.Revenue;
                    if (sourceCurrencyCode != currencyCode)
                    {
                        // revenue 
                        targetRevenueYearly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.Revenue);
                    }
                    yearlyRevenueTotal += targetRevenueYearly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != currencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    yearlyProfitTotal += profit;
                }

                // monthly revenue total- for lanes where shipping frequency is 'Per Month'
                var monthlyRevenueTotal = 0.0d;
                var monthlyProfitTotal = 0.0d;
                var monthsLanes = lanes.Where(t => t.ShippingFrequency.Equals("Per Month") && !t.Deleted).ToList();
                foreach (var oLane in monthsLanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueMonthly = oLane.Revenue;
                    if (sourceCurrencyCode != currencyCode)
                    {
                        targetRevenueMonthly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.Revenue);
                    }
                    monthlyRevenueTotal += targetRevenueMonthly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != currencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfitTotal += profit;
                }

                // weekly revenue total- for lanes where shipping frequency is 'Per Week'
                var weeklyRevenueTotal = 0.0d;
                var weeklyProfitTotal = 0.0d;
                var weeklyLanes = lanes.Where(t => t.ShippingFrequency.Equals("Per Week") && !t.Deleted).ToList();
                foreach (var oLane in weeklyLanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueWeekly = oLane.Revenue;
                    if (sourceCurrencyCode != currencyCode)
                    {
                        targetRevenueWeekly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.Revenue);
                    }
                    weeklyRevenueTotal += targetRevenueWeekly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != currencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    weeklyProfitTotal += profit;
                }





                // SPOT
                var spotLanes = lanes.Where(t => !t.ShippingFrequency.Equals("Per Month") && !t.ShippingFrequency.Equals("Per Week")
                                                            && !t.ShippingFrequency.Equals("Per Year")).ToList();
                foreach (var oLane in spotLanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueSpot = oLane.Revenue;
                    if (sourceCurrencyCode != currencyCode)
                    {
                        targetRevenueSpot = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.Revenue);
                    }
                    response.SpotRevenue += targetRevenueSpot;

                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != currencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, currencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    response.SpotProfit += profit;
                }

                if (shippingFrquency == "Per Month")
                {
                    // return total  
                    response.Revenue = yearlyRevenueTotal / 12 + (monthlyRevenueTotal) + (weeklyRevenueTotal * 4);
                    response.Profit = yearlyProfitTotal / 12 + (monthlyProfitTotal) + (weeklyProfitTotal * 4);
                }
                else if (shippingFrquency == "Per Year")
                {
                    // return total  
                    response.Revenue = yearlyRevenueTotal + (monthlyRevenueTotal * 12) + (weeklyRevenueTotal * 52);
                    response.Profit = yearlyProfitTotal + (monthlyProfitTotal * 12) + (weeklyProfitTotal * 52);
                }
            }

            return response;
        }



        private double GetLaneVolumeByUnit(Lane lane, string service, string targetShippingFrequency)
        {
            var laneVolumeUnit = lane.VolumeUnit.ToLower();
            var volumeAmount = 0.0;
            if (service.ToLower().Contains("air"))
            {
                volumeAmount = laneVolumeUnit.Equals("tonnes") ? lane.VolumeAmount :
                                    (laneVolumeUnit.Equals("kgs")) ? (lane.VolumeAmount * 0.001) :
                                    (laneVolumeUnit.Equals("lbs")) ? (lane.VolumeAmount * 0.000453592) : 0;
            }
            else if (service.ToLower().Contains("ocean"))
            {
                volumeAmount = laneVolumeUnit.Equals("teus") ? lane.VolumeAmount :
                                                   (laneVolumeUnit.Equals("cbms")) ? (lane.VolumeAmount / 35) : 0;
            }
            else if (service.ToLower().Contains("road"))
            {
                volumeAmount = laneVolumeUnit.Equals("kgs") ? lane.VolumeAmount :
                                                   (laneVolumeUnit.Equals("lbs")) ? (lane.VolumeAmount * 0.000453592) : 0;
            }

            if (targetShippingFrequency.ToLower() != lane.ShippingFrequency.ToLower())
            {
                if (targetShippingFrequency == "Per Month")
                {
                    if (lane.ShippingFrequency == "Per Year")
                        volumeAmount = volumeAmount / 12;
                    if (lane.ShippingFrequency == "Per Week")
                        volumeAmount = volumeAmount * 4;
                }
                else if (targetShippingFrequency == "Per Year")
                {
                    if (lane.ShippingFrequency == "Per Month")
                        volumeAmount = volumeAmount * 12;
                    if (lane.ShippingFrequency == "Per Week")
                        volumeAmount = volumeAmount * 52;
                }
            }
            return volumeAmount;
        }



        public string CreateExcel(List<DealsReportItem> reportData, DealsReportFilters filters)
        {
            //return "";
            try
            {
                var currencySymbol = new Currencies().GetCurrencySymbolFromCode(filters.CurrencyCode) ?? filters.CurrencyCode;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Deal Report");

                var columnId = 1;
                ws.Cell(1, columnId).Value = "Company";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Deal name";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Deal Type";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Sales Rep";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Location";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Sales Stage";
                columnId += 1;

                ws.Cell(1, columnId).Value = "Decision Date";
                columnId += 1;
                ws.Cell(1, columnId).Value = "First Shipment Date";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Contract End Date";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Proposal Due Date";
                columnId += 1;


                switch (filters.DateType)
                {
                    case "DateLost":
                        ws.Cell(1, columnId).Value = "Lost Date";
                        columnId += 1;
                        ws.Cell(1, columnId).Value = "Won/Lost Reason";
                        columnId += 1;
                        break;
                    case "DateWon":
                        ws.Cell(1, columnId).Value = "Won Date";
                        columnId += 1;
                        ws.Cell(1, columnId).Value = "Won/Lost Reason";
                        columnId += 1;
                        break;
                    default:

                        if (filters.SalesStages != null && filters.SalesStages.Count == 1 && filters.SalesStages[0].ToLower() == "won")
                        {
                            ws.Cell(1, columnId).Value = "Won Date";
                            columnId += 1;
                        }
                        else if (filters.SalesStages != null && filters.SalesStages.Count == 1 && filters.SalesStages[0].ToLower() == "lost")
                        {
                            ws.Cell(1, columnId).Value = "Lost Date";
                            columnId += 1;
                        }
                        else
                        {
                            ws.Cell(1, columnId).Value = "Updated Date";
                            columnId += 1;
                        }
                        ws.Cell(1, columnId).Value = "Won/Lost Reason";
                        columnId += 1;
                        break;
                }

                ws.Cell(1, columnId).Value = "Created Date";
                columnId += 1;
                //ws.Cell(1, columnId).Value = "Updated Date";
                //columnId += 1;
                ws.Cell(1, columnId).Value = "Industry";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Origin Countries";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Origins Locations";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Destination Countries";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Destination Locations";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Shippers";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Consignees";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Services";
                columnId += 1;
                ws.Cell(1, columnId).Value = "Comments";
                columnId += 1;
                if (!filters.IsSpotDeals)
                {
                  //  ws.Cell(1, columnId).Value = "LBs";
                  //  columnId += 1;
                    ws.Cell(1, columnId).Value = "CBMs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "TEUs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "KGs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Tonnes";
                    columnId += 1;
                }
                if (filters.IsSpotDeals)
                {
                  //  ws.Cell(1, columnId).Value = "Spot LBs";
                  //  columnId += 1;
                    ws.Cell(1, columnId).Value = "Spot CBMs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Spot TEUs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Spot KGs";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Spot Tonnes";
                    columnId += 1;
                }
                if (!filters.IsSpotDeals)
                {
                    ws.Cell(1, columnId).Value = "Revenue (" + filters.CurrencyCode + ")";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Profit (" + filters.CurrencyCode + ")";
                    columnId += 1;
                }
                if (filters.IsSpotDeals)
                {
                    ws.Cell(1, columnId).Value = "Spot Revenue (" + filters.CurrencyCode + ")";
                    columnId += 1;
                    ws.Cell(1, columnId).Value = "Spot Profit (" + filters.CurrencyCode + ")";
                    columnId += 1;
                }
                ws.Cell(1, columnId).Value = "Profit/Revenue Percentage";

                // set header styles
                ws.Range(1, 1, 1, columnId).Style.Fill.BackgroundColor = XLColor.FromArgb(58, 166, 249);
                ws.Range(1, 1, 1, columnId).Style.Font.FontColor = XLColor.White;
                ws.Range(1, 1, 1, columnId).Style.Font.Bold = true;

                var nextRow = 2;
                var dataStartRow = nextRow;
                var dataEndRow = nextRow;
                var lastColId = 0;
                var totalCol = 0;
                foreach (var reportItem in reportData)
                {
                    columnId = 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.CompanyName;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.DealName;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.DealType;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.SalesRepName;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.LocationName;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.SalesStage;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateDecision) ? null : reportItem.DateDecision;
                    ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateFirstShipment) ? null : reportItem.DateFirstShipment;
                    ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateContractEnd) ? null : reportItem.DateContractEnd;
                    ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateProposalDue) ? null : reportItem.DateProposalDue;
                    ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                    columnId += 1;

                    switch (filters.DateType)
                    {
                        case "DateLost":
                            ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateLost) ? null : reportItem.DateLost;
                            ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                            columnId += 1;
                            ws.Cell(nextRow, columnId).Value = reportItem.ReasonWonLost;
                            columnId += 1;
                            break;
                        case "DateWon":
                            ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateWon) ? null : reportItem.DateWon;
                            ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                            columnId += 1;
                            ws.Cell(nextRow, columnId).Value = reportItem.ReasonWonLost;
                            columnId += 1;
                            break;
                        default:
                            if (filters.SalesStages != null && filters.SalesStages.Count == 1 && filters.SalesStages[0].ToLower() == "won")
                            {
                                ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateWon) ? null : reportItem.DateWon;
                                ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                                columnId += 1;
                            }
                            else if (filters.SalesStages != null && filters.SalesStages.Count == 1 && filters.SalesStages[0].ToLower() == "lost")
                            {
                                ws.Cell(nextRow, columnId).Value = string.IsNullOrEmpty(reportItem.DateLost) ? null : reportItem.DateLost;
                                columnId += 1;
                            }
                            else
                            {
                                ws.Cell(nextRow, columnId).Value = Convert.ToDateTime(reportItem.DateUpdated);
                                ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                                columnId += 1;
                            }
                            ws.Cell(nextRow, columnId).Value = reportItem.ReasonWonLost;
                            columnId += 1;
                            break;
                    }

                    ws.Cell(nextRow, columnId).Value = Convert.ToDateTime(reportItem.DateCreated);
                    ws.Cell(nextRow, columnId).Style.DateFormat.Format = "DD-MMM-YY";
                    columnId += 1;
                    //ws.Cell(nextRow, columnId).Value = Convert.ToDateTime(reportItem.DateUpdated);
                    //columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.Industry;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = (reportItem.OriginCountries + "").Replace("<br/>", Environment.NewLine);
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = (reportItem.Origins + "").Replace("<br/>", Environment.NewLine);
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = (reportItem.DestinationCountries + "").Replace("<br/>", Environment.NewLine);
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = (reportItem.Destinations + "").Replace("<br/>", Environment.NewLine);
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.ShipperNames;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.ConsigneeNames;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.Services;
                    columnId += 1;
                    ws.Cell(nextRow, columnId).Value = reportItem.Comments;
                    columnId += 1;
                    // volumes
                    totalCol = columnId;
                    if (!filters.IsSpotDeals)
                    {
                        //ws.Cell(nextRow, columnId).Value = reportItem.LBs; // P
                        //ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        //columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.CBMs; // Q
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.TEUs; // R
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.KGs; // S
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.Tonnes; // T
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                    }

                    // SPOT
                    // volumes
                    if (filters.IsSpotDeals)
                    {
                        //ws.Cell(nextRow, columnId).Value = reportItem.SpotLBs; // W
                        //ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        //columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.SpotCBMs; // X
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.SpotTEUs; // Y
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.SpotKGs; // Z
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        ws.Cell(nextRow, columnId).Value = reportItem.SpotTonnes; // AA
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        //  }
                    }

                    if (!filters.IsSpotDeals)
                    {
                        // revenue and profit 
                        ws.Cell(nextRow, columnId).Value = (int)reportItem.Revenue; // U
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;

                        ws.Cell(nextRow, columnId).Value = (int)reportItem.Profit; // V
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                    }
                    //   if (reportItem.SpotRevenue > 0)
                    //   {
                    if (filters.IsSpotDeals)
                    {
                        ws.Cell(nextRow, columnId).Value = (int)reportItem.SpotRevenue; // AB
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                        //  }

                        //   if (reportItem.SpotProfit > 0)
                        //   {
                        ws.Cell(nextRow, columnId).Value = (int)reportItem.SpotProfit; // AC
                        ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                        columnId += 1;
                    }

                    ws.Cell(nextRow, columnId).Value = reportItem.ProfitRevenuePercentage;
                    ws.Cell(nextRow, columnId).Style.NumberFormat.Format = "#,##0.00";
                    columnId += 1;

                    lastColId = columnId;
                    dataEndRow = nextRow;
                    nextRow += 1;
                }

                ws.Range(nextRow, totalCol, nextRow, lastColId).Style.Fill.BackgroundColor = XLColor.FromArgb(58, 166, 249);
                ws.Range(nextRow, totalCol, nextRow, lastColId).Style.Font.FontColor = XLColor.White;
                ws.Range(nextRow, totalCol, nextRow, lastColId).Style.Font.Bold = true;

                // totals
                if (!filters.IsSpotDeals)
                {
                    //ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    //ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    //totalCol += 1;
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                }
                if (filters.IsSpotDeals)
                {
                    //ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    //ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    //totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                }

                if (!filters.IsSpotDeals)
                {
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                }

                if (filters.IsSpotDeals)
                {
                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;

                    ws.Cell(nextRow, totalCol).FormulaA1 = "=SUM(" + GetExcelColumnName(totalCol) + dataStartRow + ":" + GetExcelColumnName(totalCol) + dataEndRow + ")";
                    ws.Cell(nextRow, totalCol).Style.NumberFormat.Format = "#,##0.00";
                    totalCol += 1;
                }



                 ws.Columns().AdjustToContents();

                //  wb.SaveAs("dealsReport.xlsx");


                // var dataTable = dt;

                // Add a DataTable as a worksheet
                ///  wb.Worksheets.Add(dataTable);

                var st = new MemoryStream();
                wb.SaveAs(st);

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                var containerReference = "temp";
                var container = blobClient.GetContainerReference(containerReference);

                var fileName = "dealsReport.xlsx";

                var blobReference = "deals_report_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);
                var blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (st)
                {
                    long streamlen = st.Length;
                    st.Position = 0;
                    blockBlob.UploadFromStream(st);
                }

                return new BlobStorageHelper().GetBlob(containerReference, blobReference);

            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "CreateExcel",
                    PageCalledFrom = "DealsReport",
                    SubscriberId = filters.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filters.UserId
                };
                new Logging().LogWebAppError(error);
            }
            return "";
        }


        #region * Helper Routines *


        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }


        public string FormatDate(DateTime? dateIn, string format = "dd-MMM-yy")
        {
            string returnValue;
            if (dateIn == null) return "";
            try
            {
                var dtm = Convert.ToDateTime(dateIn);
                returnValue = dtm.ToString(format);
            }
            catch (Exception)
            {
                returnValue = "";
            }
            return returnValue;
        }


        public string GetDealReportConsignees(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var consigneeList = (from l in context.Lanes
                                 where l.DealId == dealId && (l.ConsigneeCompany + "") != ""
                                 select l.ConsigneeCompany).Distinct().ToList();
            return string.Join(",", consigneeList);
        }


        public string GetDealReportDestinations(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var lanesList = context.Lanes.Where(t => t.DealId == dealId).ToList();
            var locationList = new List<string>();
            foreach (var lane in lanesList)
            {
                var strLocation = lane.DestinationName;
                if (!string.IsNullOrEmpty(lane.DestinationCountryName))
                    strLocation += string.IsNullOrEmpty(strLocation) ? lane.DestinationCountryName : (", " + lane.DestinationCountryName);

                locationList.Add(strLocation);
            }
            return string.Join(" | ", locationList);
        }


        public string GetDealReportOrigins(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanesList = context.Lanes.Where(t => t.DealId == dealId).ToList();
            var locationList = new List<string>();
            foreach (var lane in lanesList)
            {
                var strLocation = lane.OriginName;
                if (!string.IsNullOrEmpty(lane.OriginCountryName))
                    strLocation += string.IsNullOrEmpty(strLocation) ? lane.OriginCountryName : (", " + lane.OriginCountryName);

                locationList.Add(strLocation);
            }
            return string.Join(" | ", locationList);
        }


        public double GetDealReportProfit(int dealId, int subscriberId, string userCurrencyCode)
        {
            var dealProfit = 0.00;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanesList = context.Lanes.Where(t => t.DealId == dealId).ToList();
            foreach (var lane in lanesList)
            {
                var profit = lane.ProfitPercent * lane.Revenue / 100;
                profit = new Currencies().GetCalculatedCurrencyExchangeValue(lane.CurrencyCode, userCurrencyCode, profit);
                dealProfit += profit;
            }
            return dealProfit;
        }


        public double GetDealReportRevenue(int dealId, int subscriberId, string userCurrencyCode)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanesList = context.Lanes.Where(t => t.DealId == dealId).ToList();

            foreach (var lane in lanesList)
            {
                lane.Revenue = new Currencies().GetCalculatedCurrencyExchangeValue(lane.CurrencyCode, userCurrencyCode, lane.Revenue);
            }
            var revenue = lanesList.Select(t => t.Revenue).Sum();
            return revenue;
        }


        public string GetDealReportServices(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var servicesList = (from l in context.Lanes
                                where l.DealId == dealId
                                select l.Service).Distinct().ToList();

            return string.Join(",", servicesList);
        }


        public string GetDealReportShippers(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var shipperList = (from l in context.Lanes
                               where l.DealId == dealId && (l.ShipperCompany + "") != ""
                               select l.ShipperCompany).Distinct().ToArray();

            return string.Join(",", shipperList);
        }


        public int GetDealReportVolumeOceanFclTeus(int dealId, int subscriberId)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(t => t.DealId == dealId && t.Service == "Ocean FCL");
            if (lanes.Any())
            {
                var sum = lanes.Sum(t => t.VolumeAmount);
                volume = (int)sum;
            }

            return volume;
        }


        public int GetDealReportVolumeOceanlclCbms(int dealId, int subscriberId)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(t => t.DealId == dealId && t.Service == "Ocean LCL");
            if (lanes.Any())
            {
                var sum = lanes.Sum(t => t.VolumeAmount);
                volume = (int)sum;
            }
            return volume;
        }


        public int GetDealReportVolumeAirTonnage(int dealId, int subscriberId)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(t => t.DealId == dealId && t.Service == "Air");
            if (lanes.Any())
            {
                var sum = lanes.Sum(t => t.VolumeAmount);
                volume = (int)sum;
            }
            return volume;
        }


        public int GetDealReportVolumeRoadKgs(int dealId, int subscriberId)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(t => t.DealId == dealId && t.Service.StartsWith("Road"));
            if (lanes.Any())
            {
                var sum = lanes.Sum(t => t.VolumeAmount);
                volume = (int)sum;
            }
            return volume;
        }


        public int GetDealReportVolumeLogisticsCbms(int dealId, int subscriberId)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(t => t.DealId == dealId && t.Service == "Logistics");
            if (lanes.Any())
            {
                var sum = lanes.Sum(t => t.VolumeAmount);
                volume = (int)sum;
            }
            return volume;
        }


        public int GetDealReportVolumes(int dealId, int subscriberId, string volumeUnit)
        {
            int volume = 0;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            switch (volumeUnit)
            {
                case "LBs":
                    volume = context.Lanes.Where(t => t.DealId == dealId && t.VolumeUnit == "LBs").ToList().Sum(t => (int)t.VolumeAmount);
                    break;
                case "CBMs":
                    volume = context.Lanes.Where(t => t.DealId == dealId && t.VolumeUnit == "CBMs").ToList().Sum(t => (int)t.VolumeAmount);
                    break;
                case "TEUs":
                    volume = context.Lanes.Where(t => t.DealId == dealId && t.VolumeUnit == "TEUs").ToList().Sum(t => (int)t.VolumeAmount);
                    break;
                case "KGs":
                    volume = context.Lanes.Where(t => t.DealId == dealId && t.VolumeUnit == "KGs").ToList().Sum(t => (int)t.VolumeAmount);
                    break;
                case "Tonnes":
                    volume = context.Lanes.Where(t => t.DealId == dealId && t.VolumeUnit == "Tonnes").ToList().Sum(t => (int)t.VolumeAmount);
                    break;
                default:
                    break;
            }
            return volume;
        }


        #endregion


        #region * Dropdowns *


        public List<SelectList> GetIataCodesByCountryCodes(List<string> countryCodes, string keyword, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var iataCodes = sharedContext.Airports.Where(t => (countryCodes == null || countryCodes.Count == 0 || countryCodes.Contains(t.CountryCode)
                                        || t.IataCode.StartsWith(keyword))
                                        && (keyword == "" || t.AirportName.StartsWith(keyword)))
                        .Select(t => new SelectList
                        {
                            SelectText = t.IataCode + ((t.AirportName != null && t.AirportName != "") ? " - " + t.AirportName : ""),
                            SelectValue = t.IataCode
                        }).ToList();
            return iataCodes;
        }


        public List<SelectList> GetCurrencies(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new  DbSharedDataContext(sharedConnection);
            return sharedContext.Currencies.Select(l => new SelectList
            {
                SelectText = l.CurrencyName + " (" + l.CurrencyCode + l.CurrencySymbol + ")",
                SelectValue = l.CurrencyCode + "|" + l.CurrencySymbol
            }).OrderBy(l => l.SelectText).ToList();
        }


        public List<ListItem> GetDealDateTypes()
        {
            var items = new List<ListItem>
            {
                new ListItem { Value = "DecisionDate", Text = "Decision Date" },
                new ListItem { Value = "FirstShipment", Text = "First Shipment" },
                new ListItem { Value = "ContractEnd", Text = "Contract End" },
                new ListItem { Value = "CreatedDate", Text = "Created Date" },
                new ListItem { Value = "UpdatedDate", Text = "Updated Date" },
                new ListItem { Value = "DateLost", Text = "Date Lost" },
                new ListItem { Value = "DateWon", Text = "Date Won" }
            };

            return items;
        }


        /// <summary>
        /// gets IATA or UNLOCO based on the country codes and services passed
        /// </summary>
        /// <param name="countryCodes"></param>
        /// <param name="services"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<SelectList> GetLocationsByCountryCodesServices(string countryCodes, string services, string keyword, int subscriberId)
        {


            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
            var list = new List<SelectList>();
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(services) && string.IsNullOrEmpty(countryCodes))
            {
                return list;
            }

            var serviceList = string.IsNullOrEmpty(services) ? new List<string>() : services.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var countryCodesList = string.IsNullOrEmpty(countryCodes) ? new List<string>() : countryCodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (serviceList.Any())
            {
                foreach (var service in serviceList)
                {
                    list.AddRange(service == "Air"
                        ? GetIataCodesByCountryCodes(countryCodesList, keyword, subscriberId)
                        : GetUnlocoCodesByCountryCodesServices(countryCodesList, service, keyword, subscriberId));
                }
            }
            else
            {
                var iataCodes = GetIataCodesByCountryCodes(countryCodesList, keyword, subscriberId);
                var unlocoCodes = GetUnlocoCodesByCountryCodesServices(countryCodesList, null, keyword, subscriberId);
                list.AddRange(iataCodes);
                list.AddRange(unlocoCodes);

            }

            if (list.Any())
            {
                list = list.GroupBy(t => t.SelectValue).Select(x => x.First()).OrderBy(t => t.SelectText).ToList();
            }

            return list;
        }


        public List<SelectList> GetLocations(int subscriberId)
        {

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var locations = (from j in context.Locations
                             where j.SubscriberId == subscriberId && !j.Deleted
                             orderby j.LocationName
                             select new SelectList
                             {
                                 SelectText = j.LocationName,
                                 SelectValue = j.LocationCode
                             }).Distinct().ToList();
            return locations;
        }


        public List<SelectList> GetUnlocoCodesByCountryCodesServices(List<string> countryCodes, string service, string keyword, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
            // get the UNLOCO codes for the country
            var unlocoCodes = sharedContext.UnlocoCodes.Where(t => t.Active && (countryCodes == null || countryCodes.Count == 0 ||
            countryCodes.Contains(t.CountryCode)) && (keyword == "" || t.LocationName.StartsWith(keyword) || t.UnlocoCode1.StartsWith(keyword)));
            if (!string.IsNullOrEmpty(service))
            {
                switch (service.ToLower())
                {
                    case "ocean fcl":
                        unlocoCodes = unlocoCodes.Where(t => t.Seaport);
                        break;
                    case "ocean lcl":
                        unlocoCodes = unlocoCodes.Where(t => t.Seaport);
                        break;
                    case "road expedited":
                        unlocoCodes = unlocoCodes.Where(t => t.RoadTerminal);
                        break;
                    case "road ftl":
                        unlocoCodes = unlocoCodes.Where(t => t.RoadTerminal);
                        break;
                    case "road ltl":
                        unlocoCodes = unlocoCodes.Where(t => t.RoadTerminal);
                        break;
                }
            }

            // pass them as list items 
            return unlocoCodes.Select(t => new SelectList
            {
                SelectText = t.UnlocoCode1 + ((t.LocationName != null && t.LocationName != "") ? " - " + t.LocationName : ""),
                SelectValue = t.UnlocoCode1
            }).ToList();
        }

        public List<SelectList> GetUserSalesReps(int userId, int subscriberId)
        {
            List<SelectList> list;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var q = from u in context.Users
                    join l in context.LinkUserToManagers on u.UserId equals l.UserId
                    where l.ManagerUserId == userId
                    select u;
            list = q.Select(t => new SelectList
            {
                SelectText = t.FullName,
                SelectValue = t.UserId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
            return list;
        }

        #endregion



        public static Expression<Func<TElement, bool>> BuildOrExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector)
                throw new ArgumentNullException("valueSelector");

            if (null == values)
                throw new ArgumentNullException("values");

            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));

            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }



    }

    public class ListtoDataTableConverter
    {

        public DataTable ToDataTable<T>(List<T> items)
        {

            DataTable dt = new DataTable();

            DataRow _ravi = dt.NewRow();
            _ravi["Name"] = "ravi";
            _ravi["Marks"] = "500";
            dt.Rows.Add(_ravi);

            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}
