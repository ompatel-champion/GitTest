using ClosedXML.Excel;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Microsoft.WindowsAzure.Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class UserActivityReport
    {
        public UserActivityReportResponse GetReport(UserActivityReportFilters filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(LoginUser.GetSharedConnection());


            var response = new UserActivityReportResponse
            {
                Activities = new List<UserActivityReportItem>()
            };

            if (filters.DateFrom <= DateTime.MinValue)
                filters.DateFrom = new DateTime(2000, 1, 1);

            if (filters.DateTo <= DateTime.MinValue)
                filters.DateTo = DateTime.UtcNow;

            var userActivities = context.UserActivities.Where(t => t.SubscriberId == filters.SubscriberId &&
            t.UserActivityTimestamp >= filters.DateFrom &&
            t.UserActivityTimestamp <= filters.DateTo);

            if (filters.UserIds?.Count > 0)
                userActivities = userActivities.Where(t => filters.UserIds.Contains(t.UserId));

            if (filters.ContactIds?.Count > 0)
                userActivities = userActivities.Where(t => filters.ContactIds.Contains(t.ContactId.Value));

            // global ids has been passed
            if (filters.CompanyIds?.Count > 0)
            {
                var companyIds = new List<int>();
                foreach (var c in filters.CompanyIds)
                {
                    var company = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == c);
                    if (company != null)
                    {
                        companyIds.Add(company.CompanyId);
                    }
                } 
                userActivities = userActivities.Where(t => companyIds.Contains(t.CompanyId.Value));
            }

            if (filters.DealIds?.Count > 0)
                userActivities = userActivities.Where(t => filters.DealIds.Contains(t.DealId.Value));

            // add to global list
            foreach (var activity in userActivities)
            {
                var UserActivityReportItem = new UserActivityReportItem
                {
                    UserName = activity.UserName,
                    UserActivityTimestamp = activity.UserActivityTimestamp,
                    UserActivityMessage = activity.UserActivityMessage,
                    CalendarEventSubject = activity.CalendarEventSubject,
                    CompanyName = activity.CompanyName,
                    ContactName = activity.ContactName,
                    DealName = activity.DealName,
                    NoteContent = activity.NoteContent,
                    TaskName = activity.TaskName
                };

                // UserActivityReportItem.ActivityDateStr = UserActivityReportItem.ActivityDate.ToString("yyyy-MMM-dd");
                response.Activities.Add(UserActivityReportItem);
            }

            response.Activities = response.Activities.OrderBy(t => t.UserActivityTimestamp).ToList();
            // create excel
            response.ExcelUri = CreateExcel(response.Activities, filters);
            return response;
        }

        private string CreateExcel(List<UserActivityReportItem> reportData, UserActivityReportFilters filters)
        {
            try
            {
                var dt = new DataTable("UserActivity");
                dt.Clear();
                dt.Columns.Add("UserName");
                dt.Columns.Add("UserActivityTimestamp");
                dt.Columns.Add("UserActivityMessage");
                dt.Columns.Add("CalendarEventSubject");
                dt.Columns.Add("CompanyName");
                dt.Columns.Add("ContactName");
                dt.Columns.Add("DealName");
                dt.Columns.Add("NoteContent");
                dt.Columns.Add("TaskName");

                foreach (var reportItem in reportData)
                {
                    DataRow dr = dt.NewRow();
                    dr["UserName"] = reportItem.UserName;
                    dr["UserActivityMessage"] = reportItem.UserActivityMessage;
                    dr["CalendarEventSubject"] = reportItem.CalendarEventSubject;
                    dr["CompanyName"] = reportItem.CompanyName;
                    dr["ContactName"] = reportItem.ContactName;
                    dr["DealName"] = reportItem.DealName;
                    dr["UserActivityTimestamp"] = reportItem.UserActivityTimestamp?.ToString("dd MMMM, yyyy @ HH:mm");
                    dr["NoteContent"] = reportItem.NoteContent;
                    dr["TaskName"] = reportItem.TaskName;

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

                var fileName = "useractivity.xlsx";
                var blobReference = "useractivity_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);

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
                    PageCalledFrom = "Reports/UserActivityReport",
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