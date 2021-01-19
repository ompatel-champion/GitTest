using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Helpers.Sync;
using Models;
using Location = Crm6.App_Code.Location;
using Crm6.App_Code.Sync;
using System.Threading.Tasks;

namespace Helpers
{
    public class Users
    {

        public UserModel GetUser(int userId, int subscriberId)
        {
            // get connection for user
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                          .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                          .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            ///var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserModel
                {
                    User = u,
                    Subscriber = new Subscribers().GetSubscriber(u.SubscriberId)
                })
                .FirstOrDefault();
            if (user != null)
            {
                var docs = new Documents().GetDocumentsByDocType(1, userId, subscriberId);
                if (docs.Count > 0) user.ProfilePicture = docs.FirstOrDefault();
            }

            return user;
        }


        public List<User> GetUsers(UserFilter filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get users
            var users = from user in context.Users where !user.Deleted select user;

            // apply filters

            // subscriberId
            if (filters.SubscriberId > 0) // temp fix
                users = users.Where(t => t.SubscriberId == filters.SubscriberId && !t.AdminUser);

            // login enabled
            if (filters.LoginEnabled != null)
                users = users.Where(t => t.LoginEnabled == filters.LoginEnabled);
            else
                users = users.Where(t => t.LoginEnabled);

            // country
            if (!string.IsNullOrEmpty(filters.CountryCode))
                users = users.Where(t => t.CountryCode == filters.CountryCode);

            if (!string.IsNullOrEmpty(filters.CountryName))
                users = users.Where(t => t.CountryName == filters.CountryName);

            // location
            if (!string.IsNullOrEmpty(filters.LocationName))
                users = users.Where(t => t.LocationName == filters.LocationName);

            // keyword
            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();
                users = users.Where(t => t.FirstName.ToLower().Contains(filters.Keyword)
                                         || t.LastName.ToLower().Contains(filters.Keyword)
                                         || t.EmailAddress.ToLower().Contains(filters.Keyword)
                                         || t.CountryName.ToLower().Contains(filters.Keyword)
                                         || t.LocationName.ToLower().Contains(filters.Keyword)
                                         || t.Title.ToLower().Contains(filters.Keyword));
            }


            if (filters.UserId > 0)
            {
                var userIds = new List<int>();
                var user = context.Users.FirstOrDefault(t => t.UserId == filters.UserId && t.SubscriberId == filters.SubscriberId);

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.UserRoles))
                    {
                        // get sales manager user's location codes
                        if (user.UserRoles.Contains("Sales Manager"))
                        {
                            userIds.AddRange((from t in context.LinkUserToManagers
                                              where !t.Deleted && t.ManagerUserId == user.UserId
                                              select t.UserId).ToList());
                        }

                        if (user.UserRoles.Contains("CRM Admin"))
                        {
                            userIds.AddRange(users.Select(t => t.UserId).ToList());
                        }
                        else if (user.UserRoles.Contains("Region Manager"))
                        {
                            if (!string.IsNullOrEmpty(user.RegionName))
                            {
                                // this user is a region manager, get all the deals for the region
                                var locationCodes = context.Locations
                                                           .Where(t => t.RegionName == user.RegionName && t.LocationCode != "")
                                                           .Select(t => t.LocationCode).ToList();
                                userIds.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                               t.RegionName != null && t.RegionName != "" &&
                                                               t.RegionName.Equals(user.RegionName)).Select(t => t.UserId).ToList());
                            }
                        }
                        else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                        {
                            if (!string.IsNullOrEmpty(user.CountryCode))
                            {
                                // this user is a country manager, get all the deals for the country
                                var locationCodes = context.Locations
                                                           .Where(t => t.CountryCode == user.CountryCode && t.LocationCode != "")
                                                           .Select(t => t.LocationCode).ToList();
                                userIds.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                  t.CountryCode != null && t.CountryCode != "" && t.CountryCode.Equals(user.CountryCode)).Select(t => t.UserId).ToList());
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
                                    var locationCodes = context.Locations
                                                               .Where(t => t.DistrictCode == district.DistrictCode && t.LocationCode != "")
                                                               .Select(t => t.LocationCode).ToList();
                                    userIds.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                      t.DistrictCode != null && t.DistrictCode != ""
                                                      && t.DistrictCode.Equals(user.DistrictCode)).Select(t => t.UserId).ToList());
                                }
                            }
                        }
                        else if (user.UserRoles.Contains("Location Manager"))
                        {
                            if (user.LocationId > 0)
                            {
                                var location = new Helpers.Locations().GetLocation(user.LocationId, user.SubscriberId);
                                if (location != null)
                                {
                                    // this user is a location manager, get all the deals for the location 
                                    userIds.AddRange(users.Where(t => t.LocationCode == location.LocationCode
                                                          && t.CountryCode.Equals(user.CountryCode)).Select(t => t.UserId).ToList());
                                }
                            }
                        }

                        userIds.Add(user.UserId);

                        if (userIds.Count > 0)
                        {
                            users = users.Where(t => userIds.Contains(t.UserId));
                        }
                    }
                }

            }

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
                switch (filters.SortBy.ToLower())
                {
                    case "createddate asc":
                        users = users.OrderBy(t => t.CreatedDate);
                        break;
                    case "createddate desc":
                        users = users.OrderByDescending(t => t.CreatedDate);
                        break;
                    case "firstname asc":
                        users = users.OrderBy(t => t.FirstName);
                        break;
                    case "username asc":
                        users = users.OrderBy(t => t.FirstName + " " + t.SubscriberId);
                        break;
                    case "username desc":
                        users = users.OrderByDescending(t => t.FirstName + " " + t.SubscriberId);
                        break;
                    case "lastname desc":
                        users = users.OrderByDescending(t => t.LastName);
                        break;
                    case "emailaddress asc":
                        users = users.OrderBy(t => t.EmailAddress);
                        break;
                    case "emailaddress desc":
                        users = users.OrderByDescending(t => t.EmailAddress);
                        break;
                    case "title asc":
                        users = users.OrderBy(t => t.Title);
                        break;
                    case "title desc":
                        users = users.OrderByDescending(t => t.Title);
                        break;
                    case "countryname asc":
                        users = users.OrderBy(t => t.CountryName);
                        break;
                    case "countryname desc":
                        users = users.OrderByDescending(t => t.CountryName);
                        break;
                    case "locationname asc":
                        users = users.OrderBy(t => t.LocationName);
                        break;
                    case "locationname desc":
                        users = users.OrderByDescending(t => t.LocationName);
                        break;
                }

            return users.ToList();
        }


        public bool UpdateLoginDetails(LoginDetailsSaveRequest request)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                user.BrowserName = request.BrowserName;
                user.BrowserVersion = request.BrowserVersion;
                user.IpAddress = request.IpAddress;
                user.ScreenResolution = request.ScreenResolution;
                context.SubmitChanges();

                // update GlobalUser LastLoginDate
                // look for the user in live security database
                var loginConnection = LoginUser.GetLoginConnection();
                var loginContext = new DbLoginDataContext(loginConnection);

                // check for the global user
                var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == request.UserId && u.SubscriberId == request.SubscriberId);
                if (globalUser != null)
                {
                    globalUser.LastLoginDate = DateTime.UtcNow;
                    globalUser.BrowserName = request.BrowserName;
                    globalUser.BrowserVersion = request.BrowserVersion;
                    globalUser.IpAddress = request.IpAddress;
                    globalUser.ScreenResolution = request.ScreenResolution;
                    loginContext.SubmitChanges();
                    return true;
                }
            }

            return false;
        }


        public UserSaveResponse SaveUser(UserSaveRequest request)
        {
            var response = new UserSaveResponse();

            try
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var userDetails = request.User;
                // get the user by id or create new user object
                var user = context.Users.FirstOrDefault(l => l.UserId == userDetails.UserId) ?? new User();

                user.Address = userDetails.Address;
                user.BillingCode = userDetails.BillingCode;
                user.City = userDetails.City;
                user.CurrencyCode = userDetails.CurrencyCode;
                user.CurrencySymbol = new Currencies().GetCurrencySymbolFromCode(userDetails.CurrencyCode);
                user.DateFormat = userDetails.DateFormat;

                // TODO: Remove DateFormatReports
                user.DateFormatReports = string.IsNullOrEmpty(userDetails.ReportDateFormat) ? "" : userDetails.ReportDateFormat;

                user.ReportDateFormat = string.IsNullOrEmpty(userDetails.ReportDateFormat) ? "" : userDetails.ReportDateFormat;
                user.DisplayLanguage = userDetails.DisplayLanguage;
                user.EmailAddress = userDetails.EmailAddress;
                user.Fax = userDetails.Fax;
                user.FirstName = userDetails.FirstName;
                user.LanguageCode = userDetails.LanguageCode;
                user.LanguageName = userDetails.LanguageName;
                user.LanguagesSpoken = userDetails.LanguagesSpoken;
                user.LastName = userDetails.LastName;
                user.LastLoginDate = userDetails.LastLoginDate;
                user.LastUpdate = DateTime.UtcNow;
                user.LoginEnabled = userDetails.LoginEnabled;
                user.FullName = userDetails.FirstName + " " + userDetails.LastName;
                user.MobilePhone = string.IsNullOrEmpty(userDetails.MobilePhone) ? "" : userDetails.MobilePhone;
                user.Phone = userDetails.Phone;
                user.PostalCode = userDetails.PostalCode;
                user.StateProvince = userDetails.StateProvince;
                user.TimeZone = userDetails.TimeZone;
                user.TimeZoneCityNames = userDetails.TimeZoneCityNames;
                user.TimeZoneOffset = userDetails.TimeZoneOffset;
                user.Title = userDetails.Title;
                user.UpdateUserId = userDetails.UpdateUserId;
                user.UpdateUserName = GetUserFullNameById(userDetails.UpdateUserId, userDetails.SubscriberId);
                user.UserRoles = userDetails.UserRoles;

                // location
                user.LocationId = userDetails.LocationId;
                var location = new Locations().GetLocation(userDetails.LocationId, userDetails.SubscriberId);
                if (location != null)
                {
                    user.LocationCode = location.LocationCode;
                    user.LocationName = location.LocationName;

                    // country
                    user.CountryName = location.CountryName;
                    user.CountryCode = new Countries().GetCountryCodeFromCountryName(location.CountryName) ?? "";

                    // district
                    if (string.IsNullOrEmpty(location.DistrictCode) || location.DistrictCode == "0")
                    {
                        user.DistrictCode = "";
                        user.DistrictName = "";
                    }
                    else
                    {
                        user.DistrictCode = location.DistrictCode;
                        user.DistrictName = new Districts()
                            .GetDistrictNameFromCode(location.DistrictCode, location.SubscriberId);
                    }

                    // region
                    if (string.IsNullOrEmpty(location.RegionName) || location.RegionName == "0")
                    { 
                        user.RegionName = "";
                    }
                    else
                    {
                        user.RegionName = location.RegionName; 
                    }
                }

                // only update the password if passed
                if (!string.IsNullOrEmpty(userDetails.Password))
                {
                    user.Password = userDetails.Password;
                    user.PasswordHashed = PasswordEncryptor.CreateHash(userDetails.Password);
                }
                 
                if (user.UserId < 1)
                {
                    // new user - insert
                    user.CreatedUserId = user.UpdateUserId;
                    user.CreatedUserName = user.UpdateUserName;
                    user.CreatedDate = DateTime.UtcNow;
                    user.DataCenter = new Subscribers().GetDataCenter(userDetails.SubscriberId);
                    user.SubscriberId = userDetails.SubscriberId;

                    var userWithEmailAddress = context.Users.FirstOrDefault(l => l.EmailAddress == userDetails.EmailAddress && !l.Deleted);

                    if (userWithEmailAddress == null)
                    {
                        context.Users.InsertOnSubmit(user);
                    }
                    else
                    {
                        response.ActualError = "There is a user with the same email address";
                        response.Error = response.ActualError;
                        response.IsError = true;
                        return response;
                    }
                }

                context.SubmitChanges();

                // set the response user id
                response.UserId = user.UserId;

                // TODO: Why this country criteria If block????
                if (!string.IsNullOrWhiteSpace(user.CountryCode) && !string.IsNullOrWhiteSpace(user.CountryName))
                {

                    // save profile picture
                    var profilePic = request.ProfilePic;
                    if (profilePic != null)
                    {
                        profilePic.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.UserProfilePic);
                        profilePic.SubscriberId = userDetails.SubscriberId;
                        profilePic.UploadedBy = userDetails.UpdateUserId;
                        profilePic.UploadedByName = userDetails.UpdateUserName;
                        profilePic.UserId = user.UserId;
                        new Documents().SaveDocument(profilePic);
                    }

                    // save manager user ids
                    if (request.ManagerUserIds != null)
                    {
                        // delete all manager user ids first
                        var managerUsers = context.LinkUserToManagers.Where(t => t.ManagerUserId == user.UserId).ToList();
                        if (managerUsers.Count > 0)
                        {
                            context.LinkUserToManagers.DeleteAllOnSubmit(managerUsers);
                            context.SubmitChanges();
                        }

                        foreach (var userId in request.ManagerUserIds)
                        {
                            var managerSalesRep = new LinkUserToManager
                            {
                                CreatedDate = DateTime.Now,
                                CreatedUserId = user.UpdateUserId,
                                CreatedUserName = GetUserFullNameById(user.CreatedUserId, user.SubscriberId),
                                LastUpdate = DateTime.Now,
                                ManagerName = GetUserFullNameById(user.UserId, user.SubscriberId),
                                ManagerType = "",
                                ManagerUserId = user.UserId,
                                SubscriberId = user.SubscriberId,
                                UpdateUserId = user.UpdateUserId,
                                UpdateUserName = GetUserFullNameById(user.UpdateUserId, user.SubscriberId),
                                UserId = userId,
                                UserName = GetUserFullNameById(userId, user.SubscriberId),
                                // TODO: ??
                                UserRole = ""
                            };
                            context.LinkUserToManagers.InsertOnSubmit(managerSalesRep);
                            context.SubmitChanges();
                        }
                    }

                    // save user into login database - CRM_Security
                    var userIdGlobal = SaveLoginUser(user, new Subscribers().GetDataCenter(user.SubscriberId));
                    // save 
                    user.UserIdGlobal = userIdGlobal;
                    context.SubmitChanges();

                    // return the user id ??

                }

            }
            catch (Exception ex)
            {
                response.ActualError = ex.ToString();
                response.Error = "Error saving the user.";
                response.IsError = true;
            }
            return response;
        }


        public bool IsUserFoundWithSameEmail(string emailAddress, int subscriberId)
        {
            var loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection());
            var user = loginContext.GlobalUsers.FirstOrDefault(l => l.EmailAddress.ToLower() == emailAddress.ToLower().Trim() && l.SubscriberId == subscriberId);
            return user != null;
        }


        public string GetUserCurrencyCode(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var currencyCode = context.Users.Where(u => u.UserId == userId)
                .Select(u => u.CurrencyCode).FirstOrDefault();
            return currencyCode;
        }


        public string GetUserCurrencySymbol(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (!string.IsNullOrEmpty(user.CurrencySymbol)) return user.CurrencySymbol;
            return user.CurrencyCode;
        }


        public string GetUserFullNameById(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var fullName = context.Users.Where(u => u.UserId == userId)
                               .Select(u => u.FullName).FirstOrDefault() ?? "";
            return fullName;
        }


        public string GetUserFullNameByUserIdGlobal(int userIdGlobal)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var fullName = context.Users.Where(u => u.UserIdGlobal == userIdGlobal)
                               .Select(u => u.FullName).FirstOrDefault() ?? "";
            return fullName;
        }


        public string GetUserFullNameByIdUsingDataCenter(int userId, string dataCenter)
        {
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var fullName = context.Users.Where(u => u.UserId == userId).Select(u => u.FullName).FirstOrDefault() ?? "";
            return fullName;
        }


        public string GetUserEmailById(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var fullName = context.Users.Where(u => u.UserId == userId)
                               .Select(u => u.EmailAddress).FirstOrDefault() ?? "";
            return fullName;
        }


        public int GetUserIdByFullName(string fullName, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var userId = context.Users.Where(u => u.FullName == fullName)
                .Select(u => u.UserId).FirstOrDefault();
            return userId;
        }


        public string GetUserProfilePicUrl(int userId, int subscriberId = 0, string type = null)
        {
            var defImgUrl = "/_content/_img/no-pic.png";
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var id = userId;
            if (type == null || type == "user") id = context.Users.Where(u => u.UserId == userId || u.UserIdGlobal == userId).Select(u => u.UserId).FirstOrDefault();
            var profilePic = new Documents().GetDocumentsByDocType(Convert.ToInt32(type == "contact" ? DocumentTypeEnum.ContactProfilePic : DocumentTypeEnum.UserProfilePic), id, subscriberId).ToList();
            var imageUrl = profilePic.Count == 0 ? null : profilePic[0].DocumentUrl;
            var fullName = "";
            if (type == "contact")
            {
                var contactModel = new Helpers.Contacts().GetContact(userId, subscriberId);
                fullName = contactModel.Contact.FirstName + "  " + contactModel.Contact.LastName;
            }
            else
            {
                fullName = context.Users.Where(u => u.UserId == userId || u.UserIdGlobal == userId).Select(u => u.FullName).FirstOrDefault() ?? "";
            }
            //note: the user ID acts as a color index so the background color of initial type urls is always the same for the user or contact.
            return Utils.GetProfileImageUrl(imageUrl, fullName, defImgUrl, userId);
        }


        public string GetUserTimeZone(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var timezone = context.Users.Where(u => u.UserId == userId)
                .Select(u => u.TimeZone).FirstOrDefault();
            return timezone;
        }


        public bool DeleteUser(int userId, int deletingUserId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                user.Deleted = true;
                user.DeletedUserId = deletingUserId;
                user.DeletedDate = DateTime.UtcNow;
                user.DeletedUserName = new Users().GetUserFullNameById(deletingUserId, subscriberId);
                context.SubmitChanges();

                // delete user in shared DB ??????? TODO:
                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedContext = new DbSharedDataContext(sharedConnection);
                var sharedUser = sharedContext.GlobalUsers.FirstOrDefault(l => l.UserId == userId
                                                                               && l.SubscriberId == user.SubscriberId);
                if (sharedUser != null)
                {
                    sharedContext.GlobalUsers.DeleteOnSubmit(sharedUser);
                    sharedContext.SubmitChanges();
                }

                // update user in security DB
                var loginConnection = LoginUser.GetLoginConnection();
                var loginContext = new DbLoginDataContext(loginConnection);

                // check for the global user
                var globalUser = loginContext
                                .GlobalUsers
                                .FirstOrDefault(u => u.UserId == user.UserId && u.SubscriberId == user.SubscriberId);

                if (globalUser != null)
                {
                    loginContext.GlobalUsers.DeleteOnSubmit(globalUser);
                    loginContext.SubmitChanges();
                }

                return true;
            }

            return false;
        }


        public Location GetLocation(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Users.Where(l => l.UserId == userId)
                .Select(t => new Locations().GetLocation(t.LocationId, subscriberId)).FirstOrDefault();
        }


        public string GetUserDataCenter(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get the user by id or create new user object
            var user = context.Users.FirstOrDefault(l => l.UserId == userId);
            if (user != null) return user.DataCenter;
            return "";
        }


        private void UpdateGlobalUsersForDataCenter(string connection, string dataCenter)
        {
            var dbContext = new DbFirstFreightDataContext(connection);
            if (dbContext != null)
            {
                var users = dbContext.Users.Where(c => !c.Deleted).ToList();
                foreach (var user in users)
                {
                    // look for the user in live security database
                    var loginConnection = connection;
                    var loginContext = new DbLoginDataContext(loginConnection);

                    // check for the global user
                    var globalUser = loginContext
                                    .GlobalUsers
                                    .FirstOrDefault(u => u.UserId == user.UserId && u.SubscriberId == user.SubscriberId);

                    if (globalUser == null) SaveLoginUser(user, dataCenter);
                }
            }
        }


        public int SaveLoginUser(User userDetails, string dataCenter)
        {
            try
            {
                // look for the user in live security database
                var loginLiveConnection = LoginUser.GetLoginConnection();
                var loginContext = new DbLoginDataContext(loginLiveConnection);

                // get the user by id or create new user object
                var user = loginContext.GlobalUsers.FirstOrDefault(l =>
                                l.UserId == userDetails.UserId && l.SubscriberId == userDetails.SubscriberId) ??
                                new Crm6.App_Code.Login.GlobalUser();

                // TODO: Set Deleted Boolean in GlobalUsers
                // if login enabled do not add/update, if exists delete
                //if (!userDetails.LoginEnabled)
                //{
                //    if (user != null && user.GlobalUserId > 0)
                //    {
                //        loginContext.GlobalUsers.DeleteOnSubmit(user);
                //        loginContext.SubmitChanges();
                //    }
                //    return 0;
                //}

                user.CountryName = string.IsNullOrEmpty(userDetails.CountryName) ? "" : userDetails.CountryName;
                user.CurrencyCode = string.IsNullOrEmpty(user.CurrencyCode) ? "" : user.CurrencyCode;
                user.CurrencySymbol = new Currencies().GetCurrencySymbolFromCode(userDetails.CurrencySymbol);
                user.DataCenter = dataCenter;
                user.DateFormat = string.IsNullOrEmpty(userDetails.DateFormat) ? "" : userDetails.DateFormat;

                // TODO: remove
                user.DateFormatReports = string.IsNullOrEmpty(userDetails.DateFormatReports) ? "" : userDetails.DateFormatReports;

                user.DisplayLanguage = string.IsNullOrEmpty(user.DisplayLanguage) ? "" : user.DisplayLanguage;
                user.EmailAddress = userDetails.EmailAddress;
                user.FullName = userDetails.FirstName + " " + userDetails.LastName;
                user.LanguageCode = string.IsNullOrEmpty(user.LanguageCode) ? "" : user.LanguageCode;
                user.LanguageName = string.IsNullOrEmpty(user.LanguageName) ? "" : user.LanguageName;
                user.LanguagesSpoken = string.IsNullOrEmpty(userDetails.LanguagesSpoken) ? "" : userDetails.LanguagesSpoken;
                user.LastUpdate = DateTime.UtcNow;
                // location
                user.LocationId = userDetails.LocationId;
                user.LocationCode = string.IsNullOrEmpty(userDetails.LocationCode) ? "" : userDetails.LocationCode;
                user.LocationName = string.IsNullOrEmpty(userDetails.LocationName) ? "" : userDetails.LocationName;

                user.MobilePhone = string.IsNullOrEmpty(userDetails.MobilePhone) ? "" : userDetails.MobilePhone;
                user.Phone = string.IsNullOrEmpty(user.Phone) ? "" : user.Phone;
                // region 
                user.RegionName = string.IsNullOrEmpty(userDetails.RegionName) ? "" : userDetails.RegionName;
                // timezone
                user.TimeZone = string.IsNullOrEmpty(userDetails.TimeZone) ? "" : userDetails.TimeZone;
                user.TimeZoneCityNames = string.IsNullOrEmpty(userDetails.TimeZoneCityNames) ? "" : userDetails.TimeZoneCityNames;
                user.TimeZoneOffset = string.IsNullOrEmpty(userDetails.TimeZoneOffset) ? "" : userDetails.TimeZoneOffset;
                user.Title = userDetails.Title;
                user.UpdateUserId = userDetails.UpdateUserId;
                user.UpdateUserName = string.IsNullOrEmpty(userDetails.UpdateUserName) ? "" : userDetails.UpdateUserName;
                user.UserId = userDetails.UserId;

                if (user.GlobalUserId < 1)
                {
                    // new user - insert
                    user.SubscriberId = userDetails.SubscriberId;
                    user.CreatedUserId = user.UpdateUserId;
                    user.CreatedUserName = user.UpdateUserName;
                    user.CreatedDate = DateTime.UtcNow;
                    loginContext.GlobalUsers.InsertOnSubmit(user);
                }

                loginContext.SubmitChanges();

                // return the user id
                return user.GlobalUserId;
            }
            catch (Exception)
            {
            }

            return 0;
        }


        public User GetUserByConnection(string connection, int userId)
        {
            var context = new DbFirstFreightDataContext(connection);
            return context.Users.Where(u => u.UserId == userId)
                .Select(u => u).FirstOrDefault();
        }


        public List<int> GetManagerSalesRepIds(int managerId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return (from t in context.Users
                    join u in context.LinkUserToManagers on t.UserId equals u.UserId
                    where u.ManagerUserId == managerId && !u.Deleted && !t.Deleted
                    select u.UserId).ToList();
        }


        public List<Country> GetUserCountries(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                var countries = new List<Country>();
                var salesManagerCountries = new List<Country>();

                // get sales manager user's location codes
                if (user.UserRoles.Contains("Sales Manager"))
                {
                    var countryNames = (from t in context.LinkUserToManagers
                                        join j in context.Users on t.UserId equals j.UserId
                                        where t.ManagerUserId == userId && !t.Deleted && !j.Deleted && j.CountryName != ""
                                        select j.CountryName).Distinct().ToList();

                    salesManagerCountries = (from j in sharedContext.Countries
                                             where countryNames.Contains(j.CountryName)
                                             select j).Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        // Admin - don't filter for anything except subscriberId
                        var countryNames = context.Users
                                  .Where(t => t.CountryName != "" && t.SubscriberId == user.SubscriberId && !t.Deleted)
                                  .Select(t => t.CountryName).ToList();

                        countries = (from j in sharedContext.Countries
                                     where countryNames.Contains(j.CountryName)
                                     select j).Distinct().ToList();
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // region manager - get all countries of users for the region
                            var countryNames = context.Locations
                                   .Where(t => t.RegionName == user.RegionName && t.CountryName != "" && t.SubscriberId == user.SubscriberId)
                                   .Select(t => t.CountryName).ToList();

                            countries = (from j in sharedContext.Countries
                                         where countryNames.Contains(j.CountryName)
                                         select j).Distinct().ToList();


                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryName))
                            countries = (from j in sharedContext.Countries
                                         where j.CountryName == user.CountryName
                                         select j).Distinct().ToList();
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // district manager - get all countries of users for the district
                                var countryNames = context.Locations
                                    .Where(t => t.DistrictCode == district.DistrictCode
                                                && t.CountryName != ""
                                                && t.SubscriberId == user.SubscriberId)
                                    .Select(t => t.CountryName).ToList();

                                countries = (from j in sharedContext.Countries
                                             where countryNames.Contains(j.CountryName)
                                             select j).Distinct().ToList();
                            }
                        }
                    }
                    else if (user.LocationId > 0)
                    {
                        var location = new Locations().GetLocation(user.LocationId, user.SubscriberId);
                        if (location != null)
                        {
                            countries = (from j in sharedContext.Countries
                                         where j.CountryName == location.CountryName
                                         select j).Distinct().ToList();
                        }
                    }
                }

                countries.AddRange(salesManagerCountries);
                return countries.Distinct().OrderBy(t => t.CountryName).ToList();
            }

            return new List<Country>();
        }


        public List<Location> GetUserLocations(int userId, int subscriberId, string countryCode = "")
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                var locations = new List<Location>();
                var salesManagerLocations = new List<Location>();

                // get sales manager user's location codes
                if (user.UserRoles.Contains("Sales Manager"))
                {
                    var locationCodes = (from t in context.LinkUserToManagers
                                         join j in context.Users on t.UserId equals j.UserId
                                         where t.ManagerUserId == userId && !t.Deleted && !j.Deleted && j.CountryName != ""
                                         select j.LocationCode).Distinct().ToList();

                    salesManagerLocations = (from j in context.Locations
                                             where locationCodes.Contains(j.LocationCode)
                                             && j.SubscriberId == subscriberId && !j.Deleted
                                             && (countryCode == null || countryCode == "" || j.CountryCode == countryCode)
                                             select j).Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        // Admin - don't filter for anything except subscriberId
                        locations = (from j in context.Locations
                                     where j.SubscriberId == subscriberId && !j.Deleted
                                           && (countryCode == null || countryCode == "" || j.CountryCode == countryCode)
                                     select j).Distinct().ToList();
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // region manager - get all locations of users for the region
                            locations = context.Locations
                                 .Where(t => t.RegionName == user.RegionName && t.SubscriberId == user.SubscriberId && !t.Deleted
                                 && (countryCode == null || countryCode == "" || t.CountryCode == countryCode))
                                 .Select(t => t).ToList();


                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryName))
                            locations = context.Locations
                                .Where(t => t.CountryName == user.CountryName
                                      && t.SubscriberId == user.SubscriberId && !t.Deleted
                                      && (countryCode == null || countryCode == "" || t.CountryCode == countryCode))
                                .Select(t => t).ToList();
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // country manager - get all locations of users for the country
                                locations = context.Locations
                                                    .Where(t => t.DistrictCode == district.DistrictCode
                                                     && t.SubscriberId == user.SubscriberId && !t.Deleted
                                                           && (countryCode == null || countryCode == "" || t.CountryCode == countryCode))
                                                 .Select(t => t).ToList();
                            }
                        }
                    }
                    else if (user.LocationId > 0)
                    {
                        locations = context.Locations
                                                       .Where(t => t.LocationId == user.LocationId
                                                        && t.SubscriberId == user.SubscriberId && !t.Deleted
                                                              && (countryCode == null || countryCode == "" || t.CountryCode == countryCode))
                                                    .Select(t => t).ToList();
                    }
                }

                locations.AddRange(salesManagerLocations);
                return locations.Distinct().OrderBy(t => t.LocationName).ToList();
            }

            return new List<Location>();
        }


        public string UpdatePassword(int userId, int subscriberId, string oldPassword, string newPassword)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get the user by id or create new user object
            var user = context.Users.FirstOrDefault(l => l.UserId == userId);
            if (user != null)
            {
                if (user.Password == oldPassword || PasswordEncryptor.VerifyPassword(oldPassword, user.PasswordHashed))
                {
                    user.Password = newPassword;
                    user.PasswordHashed = PasswordEncryptor.CreateHash(newPassword);
                    context.SubmitChanges();
                    return "success";
                }

                return "Invalid current password";
            }

            return "success";
        }


        public int SaveProfile(UserSaveRequest request)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var userDetails = request.User;
            // get the user by id or create new user object
            var user = context.Users.FirstOrDefault(l => l.UserId == userDetails.UserId) ?? new User();

            user.Title = userDetails.Title;
            user.EmailAddress = userDetails.EmailAddress;
            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.FullName = userDetails.FirstName + " " + userDetails.LastName;
            user.CurrencyCode = userDetails.CurrencyCode;
            user.CurrencySymbol = new Currencies().GetCurrencySymbolFromCode(userDetails.CurrencySymbol);

            user.MobilePhone = string.IsNullOrEmpty(userDetails.MobilePhone) ? "" : userDetails.MobilePhone;
            user.Phone = userDetails.Phone;
            user.Fax = userDetails.Fax;
            user.DisplayLanguage = userDetails.DisplayLanguage;
            user.LanguageCode = userDetails.LanguageCode;
            user.LanguagesSpoken = userDetails.LanguagesSpoken;
            user.LanguageName = string.IsNullOrEmpty(user.LanguageName) ? "" : user.LanguageName;

            //country
            user.CountryName = userDetails.CountryName;
            user.CountryCode = new Countries().GetCountryCodeFromCountryName(userDetails.CountryName);
            //district
            if (string.IsNullOrEmpty(userDetails.DistrictCode) || userDetails.DistrictCode == "0")
            {
                user.DistrictCode = "";
                user.DistrictName = "";
            }
            else
            {
                user.DistrictCode = userDetails.DistrictCode;
                user.DistrictName = new Districts().GetDistrictNameFromCode(userDetails.DistrictCode, request.User.SubscriberId);
            }

            //region 
            user.RegionName = userDetails.RegionName;

            // location
            var location = new Locations().GetLocation(userDetails.LocationId, userDetails.SubscriberId);
            if (location != null)
            {
                user.LocationId = userDetails.LocationId;
                user.LocationCode = location.LocationCode;
                user.LocationName = location.LocationName;
            }

            user.TimeZone = userDetails.TimeZone;
            user.TimeZoneCityNames = userDetails.TimeZoneCityNames;
            user.TimeZoneOffset = userDetails.TimeZoneOffset;

            user.LastUpdate = DateTime.UtcNow;
            user.UpdateUserId = userDetails.UpdateUserId;
            user.UpdateUserName = GetUserFullNameById(userDetails.UpdateUserId, userDetails.SubscriberId);
            user.DateFormat = string.IsNullOrEmpty(userDetails.DateFormat) ? "" : userDetails.DateFormat;
            user.DateFormatReports =
                string.IsNullOrEmpty(userDetails.DateFormatReports) ? "" : userDetails.DateFormatReports;
            user.ReportDateFormat =
                string.IsNullOrEmpty(userDetails.ReportDateFormat) ? "" : userDetails.ReportDateFormat;

            if (user.UserId < 1)
            {
                // new user - insert
                user.SubscriberId = userDetails.SubscriberId;
                user.CreatedUserId = user.UpdateUserId;
                user.CreatedUserName = user.UpdateUserName;
                user.CreatedDate = DateTime.UtcNow;
                context.Users.InsertOnSubmit(user);
            }

            context.SubmitChanges();

            // save profile picture
            var profilePic = request.ProfilePic;
            if (profilePic != null)
            {
                profilePic.SubscriberId = userDetails.SubscriberId;
                profilePic.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.UserProfilePic);
                profilePic.UserId = user.UserId;
                profilePic.UploadedBy = userDetails.UpdateUserId;
                profilePic.UploadedByName = userDetails.UpdateUserName;
                new Documents().SaveDocument(profilePic);
            }

            // save into login databse
            SaveLoginUser(user, new Subscribers().GetDataCenter(user.SubscriberId));

            // return the user id
            return user.UserId;
        }


        public List<Crm6.App_Code.Login.GlobalUser> GetGlobaUsers(int subsbcriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new DbLoginDataContext(loginConnection);

            // get all subscriber ids
            var linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                                        .Where(t => t.GlobalSubscriberId == subsbcriberId)
                                                .Select(t => t.LinkedSubscriberId).ToList();
            linkedSubscriberIds.Add(subsbcriberId);
            linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
            return loginContext.GlobalUsers.Where(t => linkedSubscriberIds.Contains(t.SubscriberId))
                                 .OrderBy(t => t.FullName).ToList();
        }


        #region Reassign User

        public bool ReassignUser(ReassignUserRequest request)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var newFullName = new Users().GetUserFullNameById(request.NewUserId, request.SubscriberId);

            var sharedWriteableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(request.SubscriberId);
            var sharedContext = new DbSharedDataContext(sharedWriteableConnection);

            // companies
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new DbLoginDataContext(loginConnection);

            var departingGlobalUser = loginContext.GlobalUsers.FirstOrDefault(t =>
                t.UserId == request.DepartingUserId && t.SubscriberId == request.SubscriberId);
            var newGlobalUser = loginContext.GlobalUsers.FirstOrDefault(t =>
                t.UserId == request.NewUserId && t.SubscriberId == request.SubscriberId);

            if (departingGlobalUser != null && newGlobalUser != null)
            {

                var linkedCompanies = sharedContext.LinkGlobalCompanyGlobalUsers
                    .Where(t => t.GlobalUserId == departingGlobalUser.GlobalUserId && !t.Deleted).ToList();

                foreach (var linkedCompany in linkedCompanies)
                {
                    var gCompany = sharedContext.GlobalCompanies.FirstOrDefault(t =>
                     !t.Deleted && t.SubscriberId == linkedCompany.CompanySubscriberId &&
                     t.GlobalCompanyId == linkedCompany.GlobalCompanyId);

                    if (gCompany != null)
                    {
                        var company = context.Companies.Where(t => t.CompanyId == gCompany.CompanyId &&
                                                                   t.SubscriberId == gCompany.SubscriberId)
                            .FirstOrDefault();
                        if (gCompany != null)
                        {
                            // check if the company owner is the depating user - if yes update
                            if (company.CompanyOwnerUserId == request.DepartingUserId &&
                                company.SubscriberId == request.SubscriberId)
                            {
                                company.CompanyOwnerUserId = request.NewUserId;
                                company.LastUpdate = DateTime.UtcNow;
                                company.UpdateUserId = 9999;
                                company.UpdateUserName = "Reassign CRM Admin";
                                context.SubmitChanges();
                            }

                            // check if the new user is already in the company sales team
                            var newGlobalUserFoundInCompany = sharedContext.LinkGlobalCompanyGlobalUsers
                                .FirstOrDefault(l =>
                                    l.GlobalCompanyId == linkedCompany.GlobalCompanyId && l.GlobalUserId ==
                                                                                       newGlobalUser.GlobalUserId
                                                                                       && !l.Deleted);
                            if (newGlobalUserFoundInCompany == null)
                            {
                                // not found - remove old sales team memeber and allocate the new one
                                linkedCompany.GlobalUserId = newGlobalUser.GlobalUserId;
                                linkedCompany.GlobalUserName = newGlobalUser.FullName;
                                linkedCompany.CreatedBy = 9999;
                                linkedCompany.CreatedByName = "Reassign CRM Admin";
                                linkedCompany.LastUpdate = DateTime.UtcNow;
                                linkedCompany.UpdateUserId = 9999;
                                linkedCompany.UpdateUserName = "Reassign CRM Admin";
                                sharedContext.SubmitChanges();
                            }
                            else
                            {
                                // new user is already in the sales team list, so remove the departing user
                                linkedCompany.Deleted = true;
                                linkedCompany.DeletedBy = 9999;
                                linkedCompany.DeletedDate = DateTime.Now;
                                sharedContext.SubmitChanges();
                            }

                            // update sales team
                            try
                            {
                                var users = sharedContext.LinkGlobalCompanyGlobalUsers
                                    .Where(l => l.GlobalCompanyId == linkedCompany.GlobalCompanyId && !l.Deleted)
                                    .Select(t => t.GlobalUserName).Distinct().ToArray();
                                var strSalesTeam = string.Join(", ", users);
                                gCompany.SalesTeam = strSalesTeam;
                                gCompany.LastUpdate = DateTime.UtcNow;
                                gCompany.UpdateUserId = 9999;
                                gCompany.UpdateUserName = "Reassign CRM Admin";
                                sharedContext.SubmitChanges();
                                company.SalesTeam = strSalesTeam;
                                company.LastUpdate = DateTime.UtcNow;
                                company.UpdateUserId = 9999;
                                company.UpdateUserName = "Reassign CRM Admin";
                                context.SubmitChanges();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                var companies = context.Companies.Where(t => t.CompanyOwnerUserId == departingGlobalUser.UserId &&
                                                             t.SubscriberId == departingGlobalUser.SubscriberId)
                    .ToList();
                foreach (var company in companies)
                {
                    company.CompanyOwnerUserId = request.NewUserId;
                    company.LastUpdate = DateTime.UtcNow;
                    company.UpdateUserId = 9999;
                    company.UpdateUserName = "Reassign CRM Admin";

                    context.SubmitChanges();

                    try
                    {
                        var gCompany = sharedContext.GlobalCompanies.FirstOrDefault(t =>
                            !t.Deleted && t.SubscriberId == company.SubscriberId
                                       && t.CompanyId == company.CompanyId);

                        if (gCompany != null)
                        {
                            var linkOwnerUser = sharedContext.LinkGlobalCompanyGlobalUsers
                                .FirstOrDefault(l => l.GlobalCompanyId == gCompany.GlobalCompanyId &&
                                                     l.GlobalUserId == newGlobalUser.GlobalUserId
                                                     && !l.Deleted);
                            if (linkOwnerUser == null)
                            {
                                var companyUser = new LinkGlobalCompanyGlobalUser();
                                companyUser.GlobalUserName = newGlobalUser.FullName;
                                companyUser.UserSubscriberId = newGlobalUser.SubscriberId;
                                companyUser.GlobalUserId = newGlobalUser.GlobalUserId;
                                companyUser.CreatedBy = 9999;
                                companyUser.CreatedByName = "Reassign CRM Admin";
                                companyUser.CreatedDate = DateTime.UtcNow;
                                companyUser.LastUpdate = DateTime.UtcNow;
                                companyUser.UpdateUserId = 9999;
                                companyUser.UpdateUserName = "Reassign CRM Admin";
                                companyUser.LinkType = "";
                                companyUser.GlobalCompanyName = gCompany.CompanyName;
                                companyUser.GlobalCompanyId = gCompany.GlobalCompanyId;
                                companyUser.CompanySubscriberId = gCompany.SubscriberId;
                                sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                                sharedContext.SubmitChanges();
                            }

                            // update sale team
                            var users = sharedContext.LinkGlobalCompanyGlobalUsers
                                .Where(l => l.GlobalCompanyId == gCompany.GlobalCompanyId && !l.Deleted)
                                .Select(t => t.GlobalUserName).Distinct().ToArray();
                            var strSalesTeam = string.Join(", ", users);
                            gCompany.SalesTeam = strSalesTeam;
                            gCompany.LastUpdate = DateTime.UtcNow;
                            gCompany.UpdateUserId = 9999;
                            gCompany.UpdateUserName = "Reassign CRM Admin";
                            sharedContext.SubmitChanges();

                            var c = context.Companies.Where(t => t.CompanyId == gCompany.CompanyId &&
                                                                 t.SubscriberId == gCompany.SubscriberId)
                                .FirstOrDefault();
                            if (c != null)
                            {
                                c.SalesTeam = strSalesTeam;
                                context.SubmitChanges();
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // contacts
            var linkedContacts = context.LinkUserToContacts
                .Where(t => t.UserId == request.DepartingUserId && !t.Deleted).ToList();
            foreach (var linkedContact in linkedContacts)
            {
                linkedContact.UserId = request.NewUserId;
                linkedContact.LastUpdate = DateTime.UtcNow;
                linkedContact.UserName = newFullName;
                linkedContact.UpdateUserId = 9999;
                linkedContact.UpdateUserName = "Reassign CRM Admin";
                context.SubmitChanges();
            }

            // deals
            var linkedDeals = context.LinkUserToDeals.Where(t => t.UserId == request.DepartingUserId && !t.Deleted)
                .ToList();
            foreach (var linkedDeal in linkedDeals)
            {
                var deal = context.Deals.FirstOrDefault(t => t.DealId == linkedDeal.DealId);
                if (deal != null)
                {
                    // check if the deal owner is the depating user - if yes update
                    if (deal.DealOwnerId == request.DepartingUserId && deal.SubscriberId == request.SubscriberId)
                    {
                        deal.UpdateUserId = 9999;
                        deal.LastUpdate = DateTime.UtcNow;
                        deal.UpdateUserName = "Reassign CRM Admin";
                        deal.DealOwnerId = request.NewUserId;
                        context.SubmitChanges();
                    }

                    var newGlobalUserFoundInDeal = context.LinkUserToDeals
                        .FirstOrDefault(l => l.DealId == linkedDeal.DealId && l.UserId == newGlobalUser.UserId
                                                                           && !l.Deleted);
                    if (newGlobalUserFoundInDeal == null)
                    {
                        // not found - remove old sales team memeber and allocate the new one 
                        linkedDeal.UserId = request.NewUserId;
                        linkedDeal.UserName = newFullName;
                        linkedDeal.UpdateUserId = 9999;
                        linkedDeal.LastUpdate = DateTime.UtcNow;
                        linkedDeal.UpdateUserName = "Reassign CRM Admin";
                        context.SubmitChanges();
                    }
                    else
                    {
                        // new user is already in the sales team list, so remove the departing user
                        linkedDeal.Deleted = true;
                        linkedDeal.DeletedUserId = 9999;
                        linkedDeal.DeletedDate = DateTime.Now;
                        linkedDeal.DeletedUserName = "Reassign CRM Admin";
                        context.SubmitChanges();
                    }

                    // update the sales team
                    var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == linkedDeal.DealId && !t.Deleted)
                        .Select(t => t.UserName).Distinct().ToList();
                    deal.UpdateUserId = 9999;
                    deal.LastUpdate = DateTime.UtcNow;
                    deal.UpdateUserName = "Reassign CRM Admin";
                    deal.SalesTeam = string.Join(", ", salesTeamUsers);
                    context.SubmitChanges();
                }
            }

            // change deal owner id
            var deals = context.Deals.Where(t => t.DealOwnerId == request.DepartingUserId).ToList();
            foreach (var deal in deals)
            {
                deal.DealOwnerId = request.NewUserId;
                var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == deal.DealId && !t.Deleted)
                    .Select(t => t.UserName).Distinct().ToList();
                deal.SalesTeam = string.Join(", ", salesTeamUsers);
                deal.UpdateUserId = 9999;
                deal.LastUpdate = DateTime.UtcNow;
                deal.UpdateUserName = "Reassign CRM Admin";
                context.SubmitChanges();
            }

            // change deal sales rep id
            deals = context.Deals.Where(t => t.SalesRepId == request.DepartingUserId).ToList();
            foreach (var deal in deals)
            {
                deal.SalesRepId = request.NewUserId;
                deal.SalesRepName = new Users().GetUserFullNameById(request.NewUserId, request.SubscriberId);
                var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == deal.DealId && !t.Deleted)
                    .Select(t => t.UserName).Distinct().ToList();
                deal.SalesTeam = string.Join(", ", salesTeamUsers);
                deal.UpdateUserId = 9999;
                deal.LastUpdate = DateTime.UtcNow;
                deal.UpdateUserName = "Reassign CRM Admin";
                context.SubmitChanges();
            }

            // future calendar events
            var calendarEvents = sharedContext.Activities.Where(t =>
                                 t.OwnerUserId == request.DepartingUserId && !t.Deleted && t.StartDateTime > DateTime.UtcNow && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")).ToList();
            foreach (var calendarEvent in calendarEvents)
            {
                calendarEvent.OwnerUserId = request.NewUserId;
                calendarEvent.UpdateUserId = 9999;
                calendarEvent.UpdateUserName = "Reassign CRM Admin";
                calendarEvent.LastUpdate = DateTime.UtcNow;

                context.SubmitChanges();
            }

            // upcoming tasks
            var tasks = sharedContext.Activities.Where(t => t.UserId == request.DepartingUserId && !t.Deleted && (t.TaskId > 0 || t.ActivityType == "TASK")).ToList();
            foreach (var task in tasks)
            {
                task.UserId = request.NewUserId;
                task.UpdateUserId = 9999;
                task.UpdateUserName = "Reassign CRM Admin";
                task.LastUpdate = DateTime.UtcNow;
                context.SubmitChanges();
            }

            return true;
        }

        #endregion


        #region Sync Settings

        public async Task<bool> SaveSyncSettings(SyncUser syncUser)
        {
            var syncEngine = new ExchangeSyncEngine();
            var isValidated = false;

            // new Sync.TestO365SyncInitializer().TestO365Sync();
            //  return false; ;
            syncUser.Connection = LoginUser.GetConnectionForDataCenter();

            if (syncUser.SyncType.Equals("Office365", StringComparison.InvariantCultureIgnoreCase) || syncUser.SyncType.Equals("Exchange", StringComparison.InvariantCultureIgnoreCase))
            {
                await syncEngine.InitEws(syncUser);
                isValidated = syncEngine.ValidateSyncUser();
            }

            if (isValidated)
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var user = context.Users.FirstOrDefault(t => t.UserId == syncUser.UserId);
                if (user != null)
                {
                    user.SyncType = syncUser.SyncType;
                    user.SyncEmail = syncUser.SyncEmail;
                    user.SyncPassword = syncUser.SyncPassword;
                    user.SyncPasswordHashed = Utils.Encrypt(syncUser.SyncPassword);
                    user.SyncUserName = syncUser.SyncUsername;
                    user.LastUpdate = DateTime.Now;
                    user.UpdateUserId = syncUser.UserId;
                    user.UpdateUserName = GetUserFullNameById(syncUser.UserId, syncUser.SubscriberId);
                    context.SubmitChanges();
                    return true;
                }

                var syncMessage = "Sync settings saved for " + user.FirstName + " " + user.LastName;
                new SyncInitializer().LogSync(user, syncUser.SyncType + " sync activated.", 0, 0, syncMessage);
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = user.UserId,
                    SubscriberId = user.SubscriberId,
                    UserActivityMessage = syncMessage
                });
            }

            return false;
        }


        public bool DisableSync(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId && t.SubscriberId == subscriberId);
            if (user != null)
            {
                user.SyncType = "";
                user.LastUpdate = DateTime.Now;
                user.UpdateUserId = userId;
                user.UpdateUserName = GetUserFullNameById(userId, subscriberId);
                user.GoogleRefreshToken = "";
                user.GoogleCalendarEmail = "";
                context.SubmitChanges();
                // set static variable
                GoogleSyncAppSettings.GoogleApiRefreshToken = "";
                GoogleSyncAppSettings.GoogleEmail = "";


                context.SubmitChanges();

                var syncMessage = "Sync disabled for " + user.FirstName + " " + user.LastName;
                new SyncInitializer().LogSync(user, user.SyncType + " sync disabled.", 0, 0, syncMessage);
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = user.UserId,
                    SubscriberId = user.SubscriberId,
                    UserActivityMessage = syncMessage
                });

                return true;
            }
            return false;
        }

        #endregion


        #region Google Sync

        public bool IsGoogleSyncEnabledForUserId(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(u => u.UserId == userId && u.SyncType.Equals("Google Apps"));
            return user != null;
        }

        #endregion


        #region unused?

        public bool UpdateGlobalUsers()
        {
            // "USA":
            var connection =
                "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //UpdateGlobalUsersForDataCenter(connection, "USA");

            //// "EMEA":
            //connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //UpdateGlobalUsersForDataCenter(connection, "EMEA");

            //// "HKG":
            //connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //UpdateGlobalUsersForDataCenter(connection, "HKG");

            // "SINOTRANS":
            connection =
                "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Sinotrans;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
            UpdateGlobalUsersForDataCenter(connection, "SINOTRANS");

            return true;
        }

        #endregion

    }
}