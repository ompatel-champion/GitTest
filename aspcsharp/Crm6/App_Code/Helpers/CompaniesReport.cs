using ClosedXML.Excel;
using Helpers;
using Microsoft.WindowsAzure.Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Crm6.App_Code.Helpers
{
    public class CompaniesReport
    {
        public CompaniesReportResponse GetReport(CompaniesReportFilters filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var response = new CompaniesReportResponse
            {
                Companies = new List<CompanyReportItem>()
            };

            var companies = context.Companies.Where(t => !t.Deleted && t.SubscriberId == filters.SubscriberId);

            if (!string.IsNullOrWhiteSpace(filters.Country))
            {
                companies = companies.Where(x => x.CountryName.ToLower() == filters.Country.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(filters.Source))
            {
                companies = companies.Where(x => x.Source.ToLower() == filters.Source.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(filters.Industry))
            {
                companies = companies.Where(x => x.Industry.ToLower() == filters.Industry.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(filters.Competitor))
            {
                companies = companies.Where(x => x.Competitors.ToLower() == filters.Competitor.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(filters.Campaign))
            {
                companies = companies.Where(x => x.CampaignName.ToLower() == filters.Campaign.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(filters.Status))
            {
                if (filters.Status.Equals("All Customers"))
                {
                    companies = companies.Where(x => x.IsCustomer);
                }
                else if (filters.Status.Equals("Active Customers"))
                {
                    companies = companies.Where(x => x.IsCustomer && x.Active);
                }
                else if (filters.Status.Equals("Inactive Customers"))
                {
                    companies = companies.Where(x => x.IsCustomer && !x.Active);
                }
                else if (filters.Status.Equals("Active Companies"))
                {
                    companies = companies.Where(x => x.Active);
                }
                else if (filters.Status.Equals("Inactive Companies"))
                {
                    companies = companies.Where(x => !x.Active);
                }
            }

            // add to global list
            foreach (var companyItem in companies)
            {
                var companyReportItem = new CompanyReportItem
                {
                    CreatedDate = companyItem.CreatedDate,
                    Address = companyItem.Address,
                    City = companyItem.City,
                    Company = companyItem.CompanyName,
                    Country = companyItem.CountryName,
                    Fax = companyItem.Fax,
                    LastActivityDate = companyItem.LastActivityDate,
                    Telephone = companyItem.Phone,
                    Industry = companyItem.Industry,
                    Source = companyItem.Source,
                    Competitor = companyItem.Competitors,
                    Campaign = companyItem.CampaignName,
                    Status = companyItem.IsCustomer && companyItem.Active ? "ACTIVE CUSTOMER" : (companyItem.IsCustomer && !companyItem.Active ? "INACTIVE CUSTOMER" : (companyItem.Active ? "ACTIVE" : "INACTIVE"))
                };

                // companyReportItem.ActivityDateStr = companyReportItem.ActivityDate.ToString("yyyy-MMM-dd");
                response.Companies.Add(companyReportItem);
            }

            response.Companies = response.Companies.OrderBy(t => t.Company).ToList();
            // create excel
            response.ExcelUri = CreateExcel(response.Companies, filters);
            return response;
        }

        private string CreateExcel(List<CompanyReportItem> reportData, CompaniesReportFilters filters)
        {
            try
            {
                var dt = new DataTable("Companies");
                dt.Clear();
                dt.Columns.Add("Company");
                dt.Columns.Add("City");
                dt.Columns.Add("Country");
                dt.Columns.Add("Industry");
                dt.Columns.Add("Source");
                dt.Columns.Add("Status");
                dt.Columns.Add("Competitor");
                dt.Columns.Add("Campaign");
                dt.Columns.Add("Telephone");
                dt.Columns.Add("Fax");
                dt.Columns.Add("Address");
                dt.Columns.Add("Created");
                dt.Columns.Add("Last Activity");

                foreach (var reportItem in reportData)
                {
                    DataRow dr = dt.NewRow();
                    dr["Company"] = reportItem.Company;
                    dr["City"] = reportItem.City;
                    dr["Country"] = reportItem.Country;
                    dr["Telephone"] = reportItem.Telephone;
                    dr["Fax"] = reportItem.Fax;
                    dr["Address"] = reportItem.Address;
                    dr["Created"] = reportItem.CreatedDate.ToString("dd MMMM, yyyy @ HH:mm");
                    dr["Last Activity"] = reportItem.LastActivityDate?.Date.ToString("dd MMMM, yyyy @ HH:mm");
                    dr["Industry"] = reportItem.Industry;
                    dr["Source"] = reportItem.Source;
                    dr["Status"] = reportItem.Status;
                    dr["Competitor"] = reportItem.Competitor;
                    dr["Campaign"] = reportItem.Campaign;

                    dt.Rows.Add(dr);
                }

                var wb = new XLWorkbook();
                var dataTable = dt;

                // Add a DataTable as a worksheet
                wb.Worksheets.Add(dataTable);
                var st = new MemoryStream();
                wb.SaveAs(st);

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                // Create the blob client
                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container
                var containerReference = "temp";
                var container = blobClient.GetContainerReference(containerReference);

                var fileName = "companies.xlsx";
                var blobReference = "companies_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);

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
                    PageCalledFrom = "Reports/CompaniesReport",
                    SubscriberId = filters.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filters.UserId
                };
                new Logging().LogWebAppError(error);
                return "";
            }
        }
    }
}