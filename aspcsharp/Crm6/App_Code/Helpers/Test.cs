using System.Web;
using System.Web.SessionState;
using Models;
using Crm6.App_Code;
using System;

public class Test //: IRequiresSessionState
{

    private static DocumentModel GetTestUserProfilePicture()
    {
        // test user profile picture object
        var testUserProfilePicture = new DocumentModel { };

        testUserProfilePicture.DocumentId = 0;
        testUserProfilePicture.DocumentTypeId =0;
        testUserProfilePicture.FileName = "";
        testUserProfilePicture.DealId = 0;
        testUserProfilePicture.ContactId = 0;
        testUserProfilePicture.CompanyId = 0;
        testUserProfilePicture.UserId = 0;
        testUserProfilePicture.EmailId = 0;
        testUserProfilePicture.Title = "";
        testUserProfilePicture.CalendarEventId = 0;
        testUserProfilePicture.Description = "";
        testUserProfilePicture.DocType = "";
        testUserProfilePicture.DocumentBlobReference = "";
        testUserProfilePicture.DocumentContainerReference = "";
        testUserProfilePicture.UploadedDate = DateTime.Parse("");
        testUserProfilePicture.DocTypeText = "";
        testUserProfilePicture.UploadedBy = 0;
        testUserProfilePicture.UploadedByName = "";

        return testUserProfilePicture;
    }


    private static Subscriber GetTestSubscriber()
    {
        // test subscriber object
        var testSubscriber = new Subscriber{};

        testSubscriber.SubscriberId = 100;
        testSubscriber.Active = true;
        testSubscriber.Address = "1350 Abbot Kinney Blvd. #201";
        testSubscriber.Billable = true;
        testSubscriber.BillingCurrencyCode = "USD";
        testSubscriber.BillingCurrencySymbol = "$";
        testSubscriber.BillingUserRate = 60;
        testSubscriber.BillingUserReadOnlyRate = 25;
        testSubscriber.City = "Los Angeles";
        testSubscriber.CompanyName = "First Freight CRM";
        testSubscriber.ContactName = "Charles Emerson";
        testSubscriber.CountryName = "United States";
        testSubscriber.CreatedDate = new DateTime(2016,12,13);
        testSubscriber.CreatedUserId = 999;
        testSubscriber.CreatedUserName = "Admin";
        testSubscriber.CrmAdminEmail = "charles@firstfreight.com";
        testSubscriber.DataCenter = "USA";
        testSubscriber.Deleted = false;
        testSubscriber.DeletedDate = null;
        testSubscriber.DeletedUserId = 0;
        testSubscriber.DeletedUserName = "";
        testSubscriber.DefaultDateFormat = "dd/MM/yyyy";
        testSubscriber.DefaultLeadResponseDays = 2;
        testSubscriber.DefaultReportCurrencyCode ="USD";
        testSubscriber.DefaultReportDateFormat = "DD-MMM-YY";
        testSubscriber.DefaultShippingFrequency = "Per Month";
        testSubscriber.Email = "charles@firstfreight.com";
        testSubscriber.EmailDisclaimer = "";
        testSubscriber.EmailFrom = "CRM Admin";
        testSubscriber.ExchangeDomain = "";
        testSubscriber.ExchangeServerTimeZone = "UTC";
        testSubscriber.ExchangeUrl = "https://outlook.office365.com/EWS/Exchange.asmx";
        testSubscriber.Fax ="";
        testSubscriber.LastUpdate = new DateTime(2016, 12, 13); ;
        testSubscriber.LogonUrl = "crm.firstfreight.com";
        testSubscriber.Phone = "";
        testSubscriber.PostalCode = "90291";
        testSubscriber.SsoApiKey = "";
        testSubscriber.StateProvince = "CA";
        testSubscriber.SubDomain = "crm";
        testSubscriber.SubscriberApiKey = "";
        testSubscriber.SubscriberIpAddress = "";
        testSubscriber.SyncType = "Exchange";
        testSubscriber.SyncServiceDomain = "";
        testSubscriber.SyncServicePassword = "";
        testSubscriber.SyncServiceUrl = "https://outlook.office365.com/EWS/Exchange.asmx";
        testSubscriber.SyncServiceUserName = "";
        testSubscriber.UpdateUserId = 1;
        testSubscriber.UpdateUserName = "Todd Collins";
        testSubscriber.UserLimit = 0;

        return testSubscriber;
    }


    private static User GetTestUser()
    {
        // user object
        var testUser = new User { };
        testUser.SubscriberId = 100;
        testUser.UserId = 9392;
        testUser.Address = "";
        testUser.AdminUser = false;
        testUser.BillingCode = "3301";
        testUser.BrowserName = "Chrome";
        testUser.BrowserVersion = "45.0";
        testUser.City = "";
        testUser.CompanyRoster = true;
        testUser.CountryCode = "US";
        testUser.CountryName = "United States";
        testUser.CreatedDate = new DateTime( 2017,8,12,20,28,53);
        testUser.CreatedUserId = 0;
        testUser.CreatedUserName = "";
        testUser.CurrencyCode = "USD";
        testUser.CurrencySymbol = "$";
        testUser.DataCenter = "DEV";
        testUser.DateFormat = "MM/dd/yyyy";
        testUser.DateFormatReports = null;
        testUser.DefaultReminderMinutes = 0;
        testUser.Deleted = false;
        testUser.DeletedDate = null;
        testUser.DeletedUserId = 0;
        testUser.DeletedUserName = null;
        testUser.Department = "";
        testUser.DisplayLanguage = "English";
        testUser.DistrictCode = null;
        testUser.DistrictName = null;
        testUser.EmailAddress = "dmartin@firstfreight.com";
        testUser.EmailSummaryDaily = false;
        testUser.EmailSummaryLastSent = null;
        testUser.EmailActivityReminders = false;
        testUser.EmailActivityReminderMinutes = 0;
        testUser.Fax = "+590 277333";
        testUser.FirstName = "Dean";
        testUser.FullName = "Dean Martin";
        testUser.IpAddress = "96.251.72.247";
        testUser.LastName = "Martin";
        testUser.LanguageCode = "en-US";
        testUser.LanguageName = "";
        testUser.LanguagesSpoken = "English";
        testUser.LastLoginDate = new DateTime(2019,4,17,21,27,13);
        testUser.LastUpdate = new DateTime(2017,9,25,14,6,37);
        testUser.LocationCode = "";
        testUser.LocationId = 4411;
        testUser.LocationName = "Los Angeles";
        testUser.LoginEnabled = true;
        testUser.LoginFailures = 0;
        testUser.MobilePhone = "+590 277333";
        testUser.Password = "dino";
        testUser.PasswordHashed = "sha1:64000:18:sFPLxDHk314FbJYsjxPad7CWqjiKQf6L:HDJbBZov1EfXiTzQD5tEMJ1B";
        testUser.Phone = "+590 277333";
        testUser.PhoneExtension = "12";
        testUser.Phone2 = "+590 277333";
        testUser.ProfilePicture = null;
        testUser.PostalCode = ""; 
        testUser.RegionName = "North America";
        testUser.ReportDateFormat = null;
        testUser.SalesRepCode = null;
        testUser.ScreenResolution = "1920x1200";
        testUser.SignatureImage = null;
        testUser.SignatureText = null;
        testUser.StateProvince = null;
        testUser.SyncOnlyCrmCategoryItems = false;
        testUser.SyncAppointmentsLastDateTime = null;
        testUser.SyncContactsLastDateTime = null;
        testUser.SyncEmail = "dean.martin@firstfreight0.onmicrosoft.com";
        testUser.SyncPassword = "Office#1350";
        testUser.SyncTasksLastDateTime = null;
        testUser.SyncUserName = null;
        testUser.SyncType = null;
        testUser.TimeZone = "Pacific Time";
        testUser.TimeZoneCityNames = "Los Angeles, Oakland, San Francisco, Portland, Seattle, Vancouver";
        testUser.TimeZoneExchange = "(UTC -05:00) Eastern Time (US & Canada)";
        testUser.TimeZoneOffset = "-08:00";
        testUser.Title = "Sales Manager";
        testUser.UpdateUserId = 9392;
        testUser.UpdateUserName = "Dean Martin";
        testUser.UserActivationDate = null;
        testUser.UserDeactivationDate = null;
        testUser.UserRoles = "Sales Manager,CRM Admin";
        testUser.Version = null;
        testUser.SyncState = null;
        testUser.GoogleRefreshToken = null;
        testUser.GoogleSyncToken = null;
        testUser.SyncPasswordHashed = null;
        testUser.GoogleCalendarEmail = null;

        return testUser;
    }


    private static UserTimeZone GetTestUserTimeZone()
    {
        // test subscriber object
        var testUserTimeZone = new UserTimeZone { };

        testUserTimeZone.TimeZone = "";
        testUserTimeZone.TimeZoneAbbreviation = "";
        testUserTimeZone.TimeZoneFullName = "";
        testUserTimeZone.UtcOffsetHours = 0.00;
        testUserTimeZone.UtcOffsetMinutes = 0;
        testUserTimeZone.UtcOffsetSeconds = 0;

        return testUserTimeZone;
    }


    public static bool SetTestUser()
    {
        // test user object
        var testUser = GetTestUser();
        // test subscriber object
        var testSubscriber = GetTestSubscriber();
        // test user time zone object
        var testUserTimeZone = GetTestUserTimeZone();

        var userModel = new UserModel() { };
        userModel.User = testUser;
        userModel.ProfilePicture = null; //DocumentModel
        userModel.Subscriber = testSubscriber;
        userModel.TimeZone = testUserTimeZone;
        userModel.DataCenter = testUser.DataCenter;
        userModel.DataCenterConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
        userModel.SharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
        userModel.WritableSharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
        userModel.LoginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";

        // create test user session - dmartin@firstfreight.com
        CreateTestUserSession(userModel);

        return true;
    }


    public static void CreateTestUserSession(UserModel userModel)
    {
        // convert user model object to JSON string
        HttpContext.Current.Session["ffuser"] = userModel;
        HttpContext.Current.Session["DataCenter"] = userModel.DataCenter;
        HttpContext.Current.Session["DataCenterConnectionstring"] = userModel.DataCenterConnection;
        HttpContext.Current.Session["SharedCenterConnectionstring"] = userModel.SharedConnection;
        HttpContext.Current.Session["LoginCenterConnectionstring"] = userModel.LoginConnection;
    }


    public static void DeleteTestUserSession()
    {
        HttpContext.Current.Session.RemoveAll();
        HttpContext.Current.Session.Abandon();
    }

}
