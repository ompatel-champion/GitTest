using System;
using System.Web.UI;
using System.Web;
using Models;
using Crm6.App_Code;
using System.Collections.Generic;
using System.Linq;

namespace Crm6
{
    public partial class TestPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ***** CRM ADMIN - DEV CRM ADMIN
            // https://develop.firstfreight.com/TestPage.aspx?token=906f01a3a56f4b55a5019ad49a5f8e0c 
            // https://localhost:44391/TestPage.aspx?token=906f01a3a56f4b55a5019ad49a5f8e0c


            // ***** LOCATION MANAGER - CARMEN MIRANDA 
            // https://develop.firstfreight.com/TestPage.aspx?token=4044d679f4f847af942d302595bb958a 
            // https://localhost:44391/TestPage.aspx?token=4044d679f4f847af942d302595bb958a


            // ***** SALES REP - DEAN MARTIN
            // https://develop.firstfreight.com/TestPage.aspx?token=aioausyh46214516hAeR1Qr 
            // https://localhost:44391/TestPage.aspx?token=aioausyh46214516hAeR1Qr


            // ***** DISTRICT MANAGER - GWEN STEFANI
            // https://develop.firstfreight.com/TestPage.aspx?token=3c341adeb0cb4e9d90746ba47b40b258 
            // https://localhost:44391/TestPage.aspx?token=3c341adeb0cb4e9d90746ba47b40b258


            // ***** COUNTRY MANAGER - CHARLES BRONSON
            // https://develop.firstfreight.com/TestPage.aspx?token=8e6ddf221cc04c23a79da5f08b78beb4 
            // https://localhost:44391/TestPage.aspx?token=8e6ddf221cc04c23a79da5f08b78beb4


            // ***** REGION MANAGER - BRUCE LEE
            // https://develop.firstfreight.com/TestPage.aspx?token=73141ac53fd947a398d8f34c7b7de949 
            // https://localhost:44391/TestPage.aspx?token=73141ac53fd947a398d8f34c7b7de949


            // ***** SALES MANAGER -  KYLIE MINOGUE
            // https://develop.firstfreight.com/TestPage.aspx?token=084a55ab70e441faac6b6612354be324 
            // https://localhost:44391/TestPage.aspx?token=084a55ab70e441faac6b6612354be324


            // ***** COUNTRY ADMIN -  SAYAKA AOKI
            // https://develop.firstfreight.com/TestPage.aspx?token=76952f97a165418aa718ca43ac88b8ac 
            // https://localhost:44391/TestPage.aspx?token=76952f97a165418aa718ca43ac88b8ac


            if (Request.QueryString["token"] != null)
            {
                // set user
                SetTestUser(Request.QueryString["token"]);

                // test if session has been set properly
                var user = LoginUser.GetLoggedInUser();
                if (user != null)
                {
                    Response.Write("success | " + Environment.NewLine);
                    Response.Write(user.User.FullName + " | uid = " + user.User.UserId + " | sid: " + user.User.SubscriberId + Environment.NewLine);
                }
                else
                {
                    Response.Write("auth failed");
                }
            }
            else
            {
                Response.Write("failed");
            }
        }


        public bool SetTestUser(string token)
        {
            // get test accounts
            var testAccounts = GetTestAccounts();
            var userModel = new UserModel();

            // get current user
            var currentUser = testAccounts.FirstOrDefault(t => t.Token == token);
            if (currentUser != null)
            {
                // get user and the profile picture based on user role
                switch (currentUser.UserRole.ToLower())
                {
                    case "crm admin":
                        userModel = GetTestUserAdmin(); // Dev CRM Admin - devadmin@firstfreight.com
                        break;
                    case "sales rep":
                        userModel = GetTestUserSalesRep();  // Dean Martin - dmartin@firstfreight.com 
                        break;
                    case "location manager":
                        userModel = GetTestUserLocationManager(); // Carmen Miranda - cmiranda@firstfreight.com
                        break;
                    case "district manager":
                        userModel = GetTestUserDistrictManager(); // Gwen Stefani - gwen@ff.com
                        break;
                    case "country manager":
                        userModel = GetTestUserCountryManager(); // Charles Bronson - cbronson@ff.com
                        break;
                    case "region manager":
                        userModel = GetTestUserRegionManager(); // Bruce Lee - blee@ff.com
                        break;
                    case "sales manager":
                        userModel = GetTestUserSalesManager(); // Kylie Minogue - kylie@headquarters.com
                        break;
                    case "country admin":
                        userModel = GetTestUserCountryAdmin(); // Sayaka Aoki - syakasan@gmail.com
                        break;
                    default:
                        break;
                }

                // test subscriber object
                var testSubscriber = GetTestSubscriber();
                // test user time zone object
                var testUserTimeZone = GetTestUserTimeZone();

                // set other user model
                userModel.Subscriber = testSubscriber;
                userModel.TimeZone = testUserTimeZone;
                userModel.DataCenter = userModel.User.DataCenter;
                userModel.DataCenterConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                userModel.SharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                userModel.WritableSharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                userModel.LoginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";

                // create test user session - dmartin@firstfreight.com
                CreateTestUserSession(userModel);

                return true;
            }
            return false;
        }


        private UserModel GetTestUserAdmin()
        {
            // user object
            var testUser = new User { };
            testUser.SubscriberId = 100;
            testUser.UserId = 9432;
            testUser.UserIdGlobal = 11811;
            testUser.Address = "";
            testUser.AdminUser = false;
            testUser.BillingCode = "3301";
            testUser.BrowserName = "Chrome";
            testUser.BrowserVersion = "45.0";
            testUser.City = "Los Angeles";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "US";
            testUser.CountryName = "United States";
            testUser.CreatedDate = new DateTime(2017, 8, 12, 20, 28, 53);
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
            testUser.EmailAddress = "devadmin@firstfreight.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "+310 - 557-8833";
            testUser.FirstName = "Dev CRM";
            testUser.FullName = "Dev CRM Admin";
            testUser.IpAddress = "96.251.72.247";
            testUser.LastName = "Admin";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = "";
            testUser.LanguagesSpoken = "English";
            testUser.LastLoginDate = new DateTime(2019, 4, 17, 21, 27, 13);
            testUser.LastUpdate = new DateTime(2017, 9, 25, 14, 6, 37);
            testUser.LocationCode = "LA";
            testUser.LocationId = 4411;
            testUser.LocationName = "Los Angeles";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "+590 277333";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:PqHC9h1Yqk19/EGxbCQiQj9/wTsA8ROR:utJ3vJCHyx8gp9Ru4LzBLReH";
            testUser.Phone = "+310 - 557-8833";
            testUser.PhoneExtension = "11";
            testUser.Phone2 = "+310 - 557-8833";
            testUser.ProfilePicture = null;
            testUser.PostalCode = ""; 
            testUser.RegionName = "The Americas";
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
            testUser.SyncPassword = "Dino#1350";
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
            testUser.UserRoles = "CRM Admin";
            testUser.Version = null;
            testUser.SyncState = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleCalendarEmail = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.CalendarEventId = 0;
            testUserProfilePicture.CompanyId = 0;
            testUserProfilePicture.ContactId = 0;
            testUserProfilePicture.DealId = 0;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "d90717a8-f982-4371-a911-fc3b4c1d14b9.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.DocumentId = 13527;
            testUserProfilePicture.DocumentTypeId = 0;
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/d90717a8-f982-4371-a911-fc3b4c1d14b9.jpg";
            testUserProfilePicture.EmailId = 0;
            testUserProfilePicture.FileName = "dibolik.jpg";
            testUserProfilePicture.LocationId = 0;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Title = "";
            testUserProfilePicture.UploadedBy = 9432;
            testUserProfilePicture.UploadedByName = "Dev CRM Admin";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9432;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;
        }



        private UserModel GetTestUserCountryAdmin()
        {
            var testUser = new User { }; 
            testUser.UserId = 9456;
            testUser.UserIdGlobal = 11830;
            testUser.SubscriberId = 100;
            testUser.Address = "3-7-1-2 Nishi Shinjuku";
            testUser.AdminUser = false;
            testUser.BillingCode = null;
            testUser.BrowserName = null;
            testUser.BrowserVersion = null;
            testUser.City = "Tokyo";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "JP";
            testUser.CountryName = "Japan";
            testUser.CreatedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "";
            testUser.CurrencyCode = "JPY";
            testUser.CurrencySymbol = "Â¥";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "dd/MM/yyyy";
            testUser.DateFormatReports = "dd-MMM-yyyy";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "en-US";
            testUser.DistrictCode = null;
            testUser.DistrictName = null;
            testUser.EmailAddress = "syakasan@gmail.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "81 03-3541-5151.";
            testUser.FirstName = "Sayaka";
            testUser.FullName = "Sayaka Aoki";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = null;
            testUser.LastName = "Aoki";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = "English ";
            testUser.LanguagesSpoken = "Japanese; English";
            testUser.LastLoginDate = null;
            testUser.LastUpdate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LocationCode = "TK";
            testUser.LocationId = 4420;
            testUser.LocationName = "Tokyo";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "81 03-3541-5151";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:5yiqE7iK1rC+6OWYHOjHafNQryq44zgt:Q208F5FQm/16VHf1PKwWK0v3";
            testUser.Phone = "81 03-3541-5151.";
            testUser.PhoneExtension = "";
            testUser.Phone2 = "81 03-3541-5151.";
            testUser.ProfilePicture = "";
            testUser.PostalCode = "03-5322-1234"; 
            testUser.RegionName = "SE Asia & Oceania";
            testUser.ReportDateFormat = "dd-MMM-yyyy";
            testUser.SalesRepCode = null;
            testUser.ScreenResolution = null;
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = null;
            testUser.SyncPassword = null;
            testUser.SyncState = null;
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "Japan Time";
            testUser.TimeZoneCityNames = null;
            testUser.TimeZoneExchange = "(UTC +09:00) Osaka; Sapporo; Tokyo";
            testUser.TimeZoneOffset = "+09:00";
            testUser.Title = "Country Manager; Japan";
            testUser.UpdateUserId = 0;
            testUser.UpdateUserName = "";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "Country Admin";
            testUser.Version = null;
            testUser.ConversionUserId = 2761;
            testUser.ConversionSalesRepCode = null;

             
            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { }; 
            testUserProfilePicture.DocumentId = 102;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "8cead54a-5ddb-4ab7-9d3c-5f327f702cda.jpg";
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/8cead54a-5ddb-4ab7-9d3c-5f327f702cda.jpg"; 
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.EmailId = 0;
            testUserProfilePicture.FileName = "profile6.jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Sayaka Aoki";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9456;
            testUserProfilePicture.UploadedByName = "Sayaka Aoki";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9456;
             
            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;


        }



        private UserModel GetTestUserSalesManager()
        {
            var testUser = new User { };
            testUser.UserId = 9430;
            testUser.UserIdGlobal = 11809;
            testUser.SubscriberId = 100;
            testUser.Address = "1 Ithica Road";
            testUser.AdminUser = false;
            testUser.BillingCode = null;
            testUser.BrowserName = "Firefox";
            testUser.BrowserVersion = "12.0";
            testUser.City = "Sydney";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "CN";
            testUser.CountryName = "China";
            testUser.CreatedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "";
            testUser.CurrencyCode = "AUD";
            testUser.CurrencySymbol = "$";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "dd/MM/yyyy";
            testUser.DateFormatReports = "dd-MMM-yyyy";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "en-US";
            testUser.DistrictCode = null;
            testUser.DistrictName = null;
            testUser.EmailAddress = "kylie@headquarters.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "(02) 8799 6503";
            testUser.FirstName = "Kylie";
            testUser.FullName = "Kylie Minogue";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = "108.13.230.146";
            testUser.LastName = "Minogue";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = "English ";
            testUser.LanguagesSpoken = "English; Australian";
            testUser.LastLoginDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LastUpdate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LocationCode = "BC";
            testUser.LocationId = 4403;
            testUser.LocationName = "Beijing";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "(02) 8799 6210";
            testUser.Password = "123$5";
            testUser.PasswordHashed = "sha1:64000:18:SM4iXJNOghJc89voOX86rLdLcFncVS/h:dpkdpBaXvFjNmPRbJDTTdcBn";
            testUser.Phone = "(02) 8799 6555";
            testUser.PhoneExtension = "";
            testUser.Phone2 = "(02) 8799 6990";
            testUser.ProfilePicture = "";
            testUser.PostalCode = "2004"; 
            testUser.RegionName = "Europe, Middle East & Africa";
            testUser.ReportDateFormat = "dd-MMM-yyyy";
            testUser.SalesRepCode = null;
            testUser.ScreenResolution = "1280x1024";
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "NSW";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = null;
            testUser.SyncPassword = null;
            testUser.SyncState = null;
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "Pacific Time";
            testUser.TimeZoneCityNames = null;
            testUser.TimeZoneExchange = "(UTC -08:00) Pacific Time (US & Canada)";
            testUser.TimeZoneOffset = "-08:00";
            testUser.Title = "Sales Manager";
            testUser.UpdateUserId = 0;
            testUser.UpdateUserName = "";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "Sales Manager";
            testUser.Version = null;
            testUser.ConversionUserId = 971;
            testUser.ConversionSalesRepCode = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.DocumentId = 13719;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "9415e28e-8547-4d31-a542-087d4c6846a2.jpg";
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/9415e28e-8547-4d31-a542-087d4c6846a2.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.FileName = "images (1).jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Kylie Minogue";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9430;
            testUserProfilePicture.UploadedByName = "Kylie Minogue";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9430;


            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;


        }

        private UserModel GetTestUserRegionManager()
        {
            var testUser = new User { };
            testUser.UserId = 9375;
            testUser.UserIdGlobal = 11770;
            testUser.SubscriberId = 100;
            testUser.Address = " Branch Tower B; 27 South Remin Road ";
            testUser.AdminUser = false;
            testUser.BillingCode = null;
            testUser.BrowserName = "Chrome";
            testUser.BrowserVersion = "31.0";
            testUser.City = "Shanghai";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "CN";
            testUser.CountryName = "China";
            testUser.CreatedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "";
            testUser.CurrencyCode = "CNY";
            testUser.CurrencySymbol = "$";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "dd/MM/yyyy";
            testUser.DateFormatReports = "";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "English ";
            testUser.DistrictCode = "SC";
            testUser.DistrictName = "South China";
            testUser.EmailAddress = "blee@ff.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "+86 21 68339152";
            testUser.FirstName = "Bruce";
            testUser.FullName = "Bruce Lee";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = "108.13.230.146";
            testUser.LastName = "Lee";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = null;
            testUser.LanguagesSpoken = "English; Mandarin";
            testUser.LastLoginDate = null;
            testUser.LastUpdate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LocationCode = "SG";
            testUser.LocationId = 4418;
            testUser.LocationName = "Shanghai";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "+86 21 68339151 ";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:Qc0HBVO+IbABp4IndhpfnwIxtCtP3QbZ:2PnJxD9B30SXsWRidyb6twWY";
            testUser.Phone = "+86 21 68339150 ";
            testUser.PhoneExtension = "21";
            testUser.Phone2 = "+86 21 68339152";
            testUser.ProfilePicture = "";
            testUser.PostalCode = ""; 
            testUser.RegionName = "Asia";
            testUser.ReportDateFormat = "MM/dd/yyyy";
            testUser.SalesRepCode = null;
            testUser.ScreenResolution = "1280x1024";
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = null;
            testUser.SyncPassword = null;
            testUser.SyncState = null;
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "China Time";
            testUser.TimeZoneCityNames = "Beijing; Chongqing; Hong Kong; Urumqi";
            testUser.TimeZoneExchange = "(UTC +08:00) Beijing; Chongqing; Hong Kong; Urumqi";
            testUser.TimeZoneOffset = "+08:00";
            testUser.Title = "Regional Manager - Asia-Pacific";
            testUser.UpdateUserId = 9463;
            testUser.UpdateUserName = "Todd Collins";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "Region Manager";
            testUser.Version = null;
            testUser.ConversionUserId = 909;
            testUser.ConversionSalesRepCode = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.DocumentId = 258;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "635dc82a-ce93-41ae-8a36-8e34a449b3ec.jpg";
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/635dc82a-ce93-41ae-8a36-8e34a449b3ec.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.FileName = "lee.jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Bruce Lee";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9375;
            testUserProfilePicture.UploadedByName = "Bruce Lee";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9375;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;
        }


        private UserModel GetTestUserCountryManager()
        {
            var testUser = new User { };
            testUser.UserId = 9380;
            testUser.UserIdGlobal = 11774;
            testUser.SubscriberId = 100;
            testUser.Address = "1350 Abbot Kinney Blvd. Suite 201";
            testUser.AdminUser = false;
            testUser.BillingCode = null;
            testUser.BrowserName = "Firefox";
            testUser.BrowserVersion = "16.0";
            testUser.City = "Los Angeles";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "US";
            testUser.CountryName = "United States";
            testUser.CreatedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "";
            testUser.CurrencyCode = "USD";
            testUser.CurrencySymbol = "$";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "MM/dd/yyyy";
            testUser.DateFormatReports = "";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "English ";
            testUser.DistrictCode = "";
            testUser.DistrictName = "";
            testUser.EmailAddress = "cbronson@ff.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "310-938-4495";
            testUser.FirstName = "Charles";
            testUser.FullName = "Charles Bronson";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = "108.13.230.146";
            testUser.LastName = "Bronson";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = null;
            testUser.LanguagesSpoken = "English";
            testUser.LastLoginDate = null;
            testUser.LastUpdate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LocationCode = "LA";
            testUser.LocationId = 4411;
            testUser.LocationName = "Los Angeles";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "310-938-4495";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:H2MDY0NPvyDVxF1nyjNFFikRdbiskWRl:HM1lIwJm4Gvcl4NgN9P1Qbkm";
            testUser.Phone = "310-938-4495";
            testUser.PhoneExtension = "56";
            testUser.Phone2 = "310-938-4495";
            testUser.ProfilePicture = "";
            testUser.PostalCode = "90291"; 
            testUser.RegionName = "The Americas";
            testUser.ReportDateFormat = "dd/MM/yyyy";
            testUser.SalesRepCode = null;
            testUser.ScreenResolution = "1280x1024";
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "CA";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = null;
            testUser.SyncPassword = null;
            testUser.SyncState = null;
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "Pacific Time";
            testUser.TimeZoneCityNames = "Los Angeles; Oakland; San Francisco; Portland; Seattle; Vancouver";
            testUser.TimeZoneExchange = "(UTC -08:00) Pacific Time (US & Canada)";
            testUser.TimeZoneOffset = "-08:00";
            testUser.Title = "Regional Manager; North America";
            testUser.UpdateUserId = 9463;
            testUser.UpdateUserName = "Todd Collins";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "Country Manager";
            testUser.Version = null;
            testUser.ConversionUserId = 920;
            testUser.ConversionSalesRepCode = null;

            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.DocumentId = 263;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/b9aa4693-fc75-450c-87fe-2fcd3d4c86d7.jpg";
            testUserProfilePicture.DocumentBlobReference = "b9aa4693-fc75-450c-87fe-2fcd3d4c86d7.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.FileName = "bronson.jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Charles Bronson";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9411;
            testUserProfilePicture.UploadedByName = "Dean Martin";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9411;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;

        }


        private UserModel GetTestUserDistrictManager()
        {
            var testUser = new User { };
            testUser.UserId = 9411;
            testUser.UserIdGlobal = 11795;
            testUser.SubscriberId = 100;
            testUser.Address = "1 Montgomery Street";
            testUser.AdminUser = false;
            testUser.BillingCode = "";
            testUser.BrowserName = "Chrome";
            testUser.BrowserVersion = "79.0";
            testUser.City = "San Francisco";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "US";
            testUser.CountryName = "United States";
            testUser.CreatedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "Todd Collins";
            testUser.CurrencyCode = "USD";
            testUser.CurrencySymbol = "$";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "MM/dd/yyyy";
            testUser.DateFormatReports = "";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "English ";
            testUser.DistrictCode = "WUS";
            testUser.DistrictName = "West USA";
            testUser.EmailAddress = "gwen@ff.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "+1 (310) 553-1237";
            testUser.FirstName = "Gwen";
            testUser.FullName = "Gwen Stefani";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = "10.0.1.42";
            testUser.LastName = "Stefani";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = null;
            testUser.LanguagesSpoken = "English";
            testUser.LastLoginDate = new DateTime(2020, 1, 14, 14, 35, 26);
            testUser.LastUpdate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUser.LocationCode = "SFO";
            testUser.LocationId = 6940;
            testUser.LocationName = "San Francisco";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "+1 (310) 553-1235";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:IFlLsh3fLo6U5fOZlr1InGOs99jnJHmu:YjIVMCMBgOwuihPINgJsJBoZ";
            testUser.Phone = "+1 (310) 553-1234";
            testUser.PhoneExtension = "";
            testUser.Phone2 = "";
            testUser.ProfilePicture = "";
            testUser.PostalCode = "94104"; 
            testUser.RegionName = "The Americas";
            testUser.ReportDateFormat = "MM/dd/yyyy";
            testUser.SalesRepCode = null;
            testUser.ScreenResolution = "1440x900";
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "CA";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = null;
            testUser.SyncPassword = null;
            testUser.SyncState = null;
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "Pacific Time";
            testUser.TimeZoneCityNames = "Los Angeles; Oakland; San Francisco; Portland; Seattle; Vancouver";
            testUser.TimeZoneExchange = "(UTC -08:00) Pacific Time (US & Canada)";
            testUser.TimeZoneOffset = "-08:00";
            testUser.Title = "District Manager; West USA";
            testUser.UpdateUserId = 9432;
            testUser.UpdateUserName = "Dev CRM Admin";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "District Manager";
            testUser.Version = null;
            testUser.ConversionUserId = 3618;
            testUser.ConversionSalesRepCode = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.DocumentId = 13718;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/c8e807a1-585b-4882-87e3-c7ecf7eccd6e.jpg";
            testUserProfilePicture.DocumentBlobReference = "c8e807a1-585b-4882-87e3-c7ecf7eccd6e.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.FileName = "images.jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Todd Collins";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9411;
            testUserProfilePicture.UploadedByName = "Dean Martin";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9411;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;



        }


        private UserModel GetTestUserLocationManager()
        {
            var testUser = new User { };
            testUser.UserId = 9384;
            testUser.UserIdGlobal = 11776;
            testUser.SubscriberId = 100;
            testUser.Address = "1350 Abbot Kinney Blvd. Suite 201";
            testUser.AdminUser = false;
            testUser.BillingCode = "";
            testUser.BrowserName = "Chrome";
            testUser.BrowserVersion = "79";
            testUser.City = "Los Angeles";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "US";
            testUser.CountryName = "United States";
            testUser.CreatedDate = new DateTime(2017, 9, 25, 14, 6, 37);
            testUser.CreatedUserId = 0;
            testUser.CreatedUserName = "Todd Collins";
            testUser.CurrencyCode = "USD";
            testUser.CurrencySymbol = "$";
            testUser.DataCenter = "DEV";
            testUser.DateFormat = "MM/dd/yyyy";
            testUser.DateFormatReports = "";
            testUser.DefaultReminderMinutes = 0;
            testUser.Deleted = false;
            testUser.DeletedDate = null;
            testUser.DeletedUserId = null;
            testUser.DeletedUserName = null;
            testUser.Department = "Sales ";
            testUser.DisplayLanguage = "English";
            testUser.DistrictCode = "WUS";
            testUser.DistrictName = "West USA";
            testUser.EmailAddress = "cmiranda@firstfreight.com";
            testUser.EmailSummaryDaily = false;
            testUser.EmailSummaryLastSent = null;
            testUser.EmailActivityReminders = false;
            testUser.EmailActivityReminderMinutes = 0;
            testUser.Fax = "";
            testUser.FirstName = "Carmen";
            testUser.FullName = "Carmen Miranda";
            testUser.Password = "ff#1";
            testUser.PasswordHashed = "sha1:64000:18:UxjedlqMPqUs6GqZtqkxF5Lqxz74lR7z:TGli3B9zwH+jwrRB3b7beyoM";
            testUser.GoogleCalendarEmail = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.IpAddress = "10.0.1.42";
            testUser.LastName = "Miranda";
            testUser.LanguageCode = "en-US";
            testUser.LanguageName = null;
            testUser.LanguagesSpoken = "English; Spanish";
            testUser.LastLoginDate = new DateTime(2020, 9, 25, 14, 6, 37);
            testUser.LastUpdate = new DateTime(2017, 9, 25, 14, 6, 37);
            testUser.LocationCode = "LA";
            testUser.LocationId = 4411;
            testUser.LocationName = "Los Angeles";
            testUser.LoginEnabled = true;
            testUser.LoginFailures = 0;
            testUser.MobilePhone = "(310) 354-1234";
            testUser.Phone = "(310) 354-1234";
            testUser.PhoneExtension = "";
            testUser.Phone2 = "";
            testUser.ProfilePicture = "";
            testUser.PostalCode = "90291"; 
            testUser.RegionName = "The Americas";
            testUser.ReportDateFormat = "MM/dd/yyyy";
            testUser.SalesRepCode = "";
            testUser.ScreenResolution = "1440x900";
            testUser.SignatureImage = "";
            testUser.SignatureText = "";
            testUser.StateProvince = "CA";
            testUser.SyncOnlyCrmCategoryItems = false;
            testUser.SyncAppointmentsLastDateTime = null;
            testUser.SyncContactsLastDateTime = null;
            testUser.SyncEmail = "";
            testUser.SyncPassword = "";
            testUser.SyncState = "";
            testUser.SyncTasksLastDateTime = null;
            testUser.SyncType = "Do Not Sync";
            testUser.SyncUserName = null;
            testUser.TimeZone = "Pacific Time";
            testUser.TimeZoneCityNames = "Los Angeles; Oakland; San Francisco; Portland; Seattle; Vancouver";
            testUser.TimeZoneExchange = "(UTC -08:00) Pacific Time (US & Canada)";
            testUser.TimeZoneOffset = "-08:00";
            testUser.Title = "Station Manager";
            testUser.UpdateUserId = 9432;
            testUser.UpdateUserName = "Dev CRM Admin";
            testUser.UserActivationDate = null;
            testUser.UserDeactivationDate = null;
            testUser.UserRoles = "Location Manager";
            testUser.Version = null;
            testUser.ConversionUserId = 3598;
            testUser.ConversionSalesRepCode = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.DocumentId = 259;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "777e0ffc-4849-4386-97e6-ba6effda38d9.jpg";
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/777e0ffc-4849-4386-97e6-ba6effda38d9.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.EmailId = 0;
            testUserProfilePicture.FileName = "carmen.jpg";
            testUserProfilePicture.Title = null;
            testUserProfilePicture.UploadedByName = "Todd Collins";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UploadedBy = 9463;
            testUserProfilePicture.UploadedByName = "Dean Martin";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9392;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;
        }



        private UserModel GetTestUserSalesRep()
        {
            // user object
            var testUser = new User { };
            testUser.SubscriberId = 100;
            testUser.UserId = 9392;
            testUser.UserIdGlobal = 11781;
            testUser.Address = "";
            testUser.AdminUser = false;
            testUser.BillingCode = "3301";
            testUser.BrowserName = "Chrome";
            testUser.BrowserVersion = "45.0";
            testUser.City = "";
            testUser.CompanyRoster = true;
            testUser.CountryCode = "US";
            testUser.CountryName = "United States";
            testUser.CreatedDate = new DateTime(2017, 8, 12, 20, 28, 53);
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
            testUser.LastLoginDate = new DateTime(2019, 4, 17, 21, 27, 13);
            testUser.LastUpdate = new DateTime(2017, 9, 25, 14, 6, 37);
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
            testUser.UserRoles = "Sales Rep";
            testUser.Version = null;
            testUser.SyncState = null;
            testUser.GoogleRefreshToken = null;
            testUser.GoogleSyncToken = null;
            testUser.SyncPasswordHashed = null;
            testUser.GoogleCalendarEmail = null;


            // test user profile picture object
            var testUserProfilePicture = new DocumentModel { };
            testUserProfilePicture.CalendarEventId = 0;
            testUserProfilePicture.CompanyId = 0;
            testUserProfilePicture.ContactId = 0;
            testUserProfilePicture.DealId = 0;
            testUserProfilePicture.Description = null;
            testUserProfilePicture.DocType = "UserProfilePic";
            testUserProfilePicture.DocTypeText = "UserProfilePic";
            testUserProfilePicture.DocumentBlobReference = "e425bfb3-2151-4fac-a853-20e631c4b64c.jpg";
            testUserProfilePicture.DocumentContainerReference = "users";
            testUserProfilePicture.DocumentId = 51;
            testUserProfilePicture.DocumentTypeId = 0;
            testUserProfilePicture.DocumentUrl = "https://crm6.blob.core.windows.net/users/e425bfb3-2151-4fac-a853-20e631c4b64c.jpg";
            testUserProfilePicture.EmailId = 0;
            testUserProfilePicture.FileName = "dino.jpg";
            testUserProfilePicture.LocationId = 0;
            testUserProfilePicture.SubscriberId = 100;
            testUserProfilePicture.Title = "";
            testUserProfilePicture.UploadedBy = 9392;
            testUserProfilePicture.UploadedByName = "Dean Martin";
            testUserProfilePicture.UploadedDate = new DateTime(2017, 8, 14, 14, 35, 26);
            testUserProfilePicture.UserId = 9392;

            // set user model object 
            var userModel = new UserModel
            {
                User = testUser,
                ProfilePicture = testUserProfilePicture
            };
            return userModel;
        }


        private Subscriber GetTestSubscriber()
        {
            // test subscriber object
            var testSubscriber = new Subscriber { };

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
            testSubscriber.CreatedDate = new DateTime(2016, 12, 13);
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
            testSubscriber.DefaultReportCurrencyCode = "USD";
            testSubscriber.DefaultReportDateFormat = "dd-MMM-yyyy";
            testSubscriber.DefaultShippingFrequency = "Per Month";
            testSubscriber.Email = "charles@firstfreight.com";
            testSubscriber.EmailDisclaimer = "";
            testSubscriber.EmailFrom = "CRM Admin";
            testSubscriber.ExchangeDomain = "";
            testSubscriber.ExchangeServerTimeZone = "UTC";
            testSubscriber.ExchangeUrl = "https://outlook.office365.com/EWS/Exchange.asmx";
            testSubscriber.Fax = "";
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



        private UserTimeZone GetTestUserTimeZone()
        {
            // test user time  zone object
            var testUserTimeZone = new UserTimeZone { };

            testUserTimeZone.TimeZone = "";
            testUserTimeZone.TimeZoneAbbreviation = "";
            testUserTimeZone.TimeZoneFullName = "";
            testUserTimeZone.UtcOffsetHours = 0.00;
            testUserTimeZone.UtcOffsetMinutes = 0;
            testUserTimeZone.UtcOffsetSeconds = 0;

            return testUserTimeZone;
        }



        public void CreateTestUserSession(UserModel userModel)
        {
            // convert user model object to JSON string
            HttpContext.Current.Session["ffuser"] = userModel;
            HttpContext.Current.Session["DataCenter"] = userModel.DataCenter;
            HttpContext.Current.Session["DataCenterConnectionstring"] = userModel.DataCenterConnection;
            HttpContext.Current.Session["SharedCenterConnectionstring"] = userModel.SharedConnection;
            HttpContext.Current.Session["LoginCenterConnectionstring"] = userModel.LoginConnection;
        }


        public void DeleteTestUserSession()
        {
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// get all the test accounts
        /// TODO: charles to provide other user role users
        /// </summary>
        /// <returns></returns>
        private List<TestUserAccount> GetTestAccounts()
        {
            var testUserAccounts = new List<TestUserAccount>();
            testUserAccounts.Add(new TestUserAccount { Token = "906f01a3a56f4b55a5019ad49a5f8e0c", UserId = 9432, UserIdGlobal = 11811, SubscriberId = 100, UserRole = "CRM Admin" });
            testUserAccounts.Add(new TestUserAccount { Token = "aioausyh46214516hAeR1Qr", UserId = 9392, UserIdGlobal = 11781, SubscriberId = 100, UserRole = "Sales Rep" });
            testUserAccounts.Add(new TestUserAccount { Token = "4044d679f4f847af942d302595bb958a", UserId = 9384, UserIdGlobal = 11776, SubscriberId = 100, UserRole = "Location Manager" });
            testUserAccounts.Add(new TestUserAccount { Token = "3c341adeb0cb4e9d90746ba47b40b258", UserId = 9411, UserIdGlobal = 11795, SubscriberId = 100, UserRole = "District Manager" });
            testUserAccounts.Add(new TestUserAccount { Token = "8e6ddf221cc04c23a79da5f08b78beb4", UserId = 9380, UserIdGlobal = 11774, SubscriberId = 100, UserRole = "Country Manager" });
            testUserAccounts.Add(new TestUserAccount { Token = "73141ac53fd947a398d8f34c7b7de949", UserId = 9375, UserIdGlobal = 11770, SubscriberId = 100, UserRole = "Region Manager" });
            testUserAccounts.Add(new TestUserAccount { Token = "084a55ab70e441faac6b6612354be324", UserId = 9430, UserIdGlobal = 11809, SubscriberId = 100, UserRole = "Sales Manager" });
            testUserAccounts.Add(new TestUserAccount { Token = "76952f97a165418aa718ca43ac88b8ac", UserId = 9456, UserIdGlobal = 11830, SubscriberId = 100, UserRole = "Country Admin" });
            return testUserAccounts;
        }
    }


    public class TestUserAccount
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public int UserIdGlobal { get; set; }
        public int SubscriberId { get; set; }
        public string UserRole { get; set; }
    }

}
