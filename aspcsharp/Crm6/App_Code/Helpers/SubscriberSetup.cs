using System;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Models;
using GlobalSubscriber = Crm6.App_Code.Shared.GlobalSubscriber;
using GlobalUser = Crm6.App_Code.Shared.GlobalUser;

namespace Helpers
{

    public class SetupSubscriber
    {

        public string NewSubscriber(SubscriberSetupModel subscriberSetup)
        {
            string returnValue;

            // Create Subscriber Record
            var subscriberId = AddSubscriber(subscriberSetup);
            returnValue = subscriberId.ToString();

            // Set data center connection string
            var connection = "";
            var sharedConnection = "";
            switch (subscriberSetup.DataCenter.ToLower())
            {
                case "usa":
                    connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    sharedConnection = "";
                    break;
                case "emea":
                    connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    sharedConnection = "";
                    break;
                case "hkg":
                    connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    sharedConnection = "";
                    break;
                case "dev":
                    connection = "Data Source=ff-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=FfTest;Password=Ftest#Test!1";
                    sharedConnection = "";
                    break;
            }

            // Setup Default Tables
            returnValue += SetupDefaultCompanyLinkTypes(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultCompanySegments(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultCompanyTypes(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultContactTypes(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultDealTypes(subscriberId, connection) + Environment.NewLine;

            //returnValue += SetupDefaultEventCategories(subscriberId, connection) + Environment.NewLine;

            returnValue += SetupDefaultIndustries(subscriberId, connection) + Environment.NewLine;

            returnValue += SetupDefaultLostReasons(subscriberId, connection) + Environment.NewLine;

            returnValue += SetupDefaultRegions(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultSalesStages(subscriberId, connection) + Environment.NewLine;
            returnValue += SetupDefaultSources(subscriberId, connection) + Environment.NewLine;

            //returnValue += SetupDefaultWonReasons(subscriberId, connection) + Environment.NewLine;

            // Create FF CRM Admin User for SubscriberId
            var udFfAdmin = new User
            {
                AdminUser = true,
                CompanyRoster = false,
                CountryName = subscriberSetup.CountryName,
                CountryCode = new Countries().GetCountryCodeFromCountryName(subscriberSetup.CountryName),
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                DataCenter = subscriberSetup.DataCenter,
                DisplayLanguage = "en-US",
                EmailAddress = "admin@" + subscriberSetup.Subdomain + ".com",
                FirstName = subscriberSetup.Subdomain,
                FullName = subscriberSetup.Subdomain + " Admin",
                LastName = "Admin",
                LastUpdate = DateTime.UtcNow,
                LoginEnabled = true,
                Password = "123$5",
                SubscriberId = subscriberId,
                TimeZone = "Pacific Time",
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserRoles = "CRM Admin"
            };

            // Add User to subscriber data center
            var ffUserAdminId = AddNewSubscriberUser(udFfAdmin, connection);

            // Add FF CRM Admin User to Shared Database
            var globalConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var sharedContext = new DbSharedDataContext(globalConnection);
            var globalUser = new GlobalUser
            {
                CountryName = subscriberSetup.CountryName,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                DataCenter = subscriberSetup.DataCenter,
                DateFormat = "dd/MM/yyyy",
                DateFormatReports = "DD-MMM-YY",
                DisplayLanguage = "en-US",
                EmailAddress = "admin@" + subscriberSetup.Subdomain + ".com",
                FullName = subscriberSetup.Subdomain + " Admin",
                LanguageCode = "en-US",
                LastUpdate = DateTime.UtcNow,
                SubscriberId = subscriberId,
                TimeZone = "Pacific Time",
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserId = ffUserAdminId,
                UserRoles = "CRM Admin"
            };
            sharedContext.GlobalUsers.InsertOnSubmit(globalUser);
            sharedContext.SubmitChanges();

            // Add FF CRM Admin User to Security Database
            var securityConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var securityContext = new DbLoginDataContext(securityConnection);
            var securityUser = new Crm6.App_Code.Login.GlobalUser()
            {
                CountryName = subscriberSetup.CountryName,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                DataCenter = subscriberSetup.DataCenter,
                DateFormat = "dd/MM/yyyy",
                DateFormatReports = "DD-MMM-YY",
                DisplayLanguage = "en-US",
                EmailAddress = "admin@" + subscriberSetup.Subdomain + ".com",
                FullName = subscriberSetup.Subdomain + " Admin",
                LanguageCode = "en-US",
                LastUpdate = DateTime.UtcNow,
                SubscriberId = subscriberId,
                TimeZone = "Pacific Time",
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserId = ffUserAdminId,
                UserRoles = "CRM Admin"
            };
            securityContext.GlobalUsers.InsertOnSubmit(securityUser);
            securityContext.SubmitChanges();

            // Create Default Location
            var context = new DbFirstFreightDataContext(connection);
            var location = new Crm6.App_Code.Location
            { 
                Address = subscriberSetup.Address,
                City = subscriberSetup.City,
                CountryName = subscriberSetup.CountryName,
                PostalCode = subscriberSetup.PostalCode,
                LocationCode = "HQ",
                LocationName = subscriberSetup.City,
                LocationType = "Station",
                StateProvince = subscriberSetup.StateProvince,
                SubscriberId = subscriberSetup.SubscriberId
            };
            context.Locations.InsertOnSubmit(location);
            returnValue += "Created Loction: " + subscriberSetup.City + Environment.NewLine;

            // Create Subscriber CRM Admin User
            var ud = new User
            {
                AdminUser = false,
                CountryName = subscriberSetup.CountryName,
                CountryCode = new Countries().GetCountryCodeFromCountryName(subscriberSetup.CountryName),
                CompanyRoster = true,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                DataCenter = subscriberSetup.DataCenter,
                // TODO: Fix for selected Language
                DisplayLanguage = "en-US",
                EmailAddress = subscriberSetup.Email,
                FirstName = subscriberSetup.FirstName,
                FullName = subscriberSetup.ContactName,
                LastName = subscriberSetup.LastName,
                LastUpdate = DateTime.UtcNow,
                LocationCode = "HQ",
                LocationName = subscriberSetup.City,
                LoginEnabled = true,
                Password = subscriberSetup.Password,
                SubscriberId = subscriberId,
                TimeZone = subscriberSetup.Timezone,
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserRoles = "CRM Admin"
            };
            var subscriberCrmAdminUserId = AddNewSubscriberUser(ud, connection);

            // Setup New CRM Admin user in Shared Database
            globalUser = new GlobalUser
            {
                CountryName = subscriberSetup.CountryName,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                DataCenter = subscriberSetup.DataCenter,
                DateFormat = "dd/MM/yyyy",
                DateFormatReports = "DD-MMM-YY",
                // TODO: Fix for selected Language
                DisplayLanguage = "en-US",
                EmailAddress = subscriberSetup.Email,
                FullName = subscriberSetup.FirstName + " " + subscriberSetup.LastName,
                LanguageCode = "en-US",
                LastUpdate = DateTime.UtcNow,
                SubscriberId = subscriberId,
                TimeZone = subscriberSetup.Timezone,
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserId = subscriberCrmAdminUserId,
                UserRoles = "CRM Admin"
            };
            sharedContext.GlobalUsers.InsertOnSubmit(globalUser);
            sharedContext.SubmitChanges();

            // Add FF CRM Admin User to Security Database
            securityUser = new Crm6.App_Code.Login.GlobalUser()
            {
                CountryName = subscriberSetup.CountryName,
                CreatedDate = DateTime.UtcNow,
                CreatedUserId = 999,
                CreatedUserName = "Admin Setup",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                DataCenter = subscriberSetup.DataCenter,
                DateFormat = "dd/MM/yyyy",
                DateFormatReports = "DD-MMM-YY",
                // TODO: Fix for selected Language
                DisplayLanguage = "en-US",
                EmailAddress = subscriberSetup.Email,
                FullName = subscriberSetup.FirstName + " " + subscriberSetup.LastName,
                LanguageCode = "en-US",
                LastUpdate = DateTime.UtcNow,
                SubscriberId = subscriberId,
                TimeZone = subscriberSetup.Timezone,
                UpdateUserId = 999,
                UpdateUserName = "Admin Setup",
                UserActivationDate = DateTime.UtcNow,
                UserId = subscriberCrmAdminUserId,
                UserRoles = "CRM Admin"
            };
            securityContext.GlobalUsers.InsertOnSubmit(securityUser);
            securityContext.SubmitChanges();

            // Return full setup message
            return returnValue;
        }


        private static int AddSubscriber(SubscriberSetupModel subscriberSetup)
        { 
            // Get Next SubscriberId
            var securityConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var securityContext = new DbLoginDataContext(securityConnection); 

            var subscriberId = securityContext.GlobalSubscribers.Max(s => s.SubscriberId) + 1;
             
            // Add Subscriber to CRM_Security database 
            var securitySubscriber = new Crm6.App_Code.Login.GlobalSubscriber()
            {
                SubscriberId = subscriberId,
                Active = true,
                CompanyName = subscriberSetup.CompanyName,
                DataCenter = subscriberSetup.DataCenter,
                LastUpdate = DateTime.UtcNow,
                UpdateUserId = 999,
                UpdateUserName = "FF Admin"
            };
            securityContext.GlobalSubscribers.InsertOnSubmit(securitySubscriber);
            securityContext.SubmitChanges();

            // Set connection string context based on Subscriber DataCenter
            var connection = "";
            switch (securitySubscriber.DataCenter.ToLower())
            {
                case "usa":
                    connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    break;
                case "emea":
                    connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    break;
                case "hkg":
                    connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    break;
                case "dev":
                    connection = "Data Source=ff-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=FfTest;Password=Ftest#Test!1";
                    break;
                default:
                    connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                    break;
            }
            var context = new DbFirstFreightDataContext(connection);

            // Add New Subscriber Record to DataCenter Database
            var subscriber = new Subscriber
            {
                Active = true,
                Billable = true,
                CompanyName = subscriberSetup.CompanyName,
                ContactName = subscriberSetup.ContactName,
                CountryName = subscriberSetup.CountryName,
                CreatedUserId = 9999,
                CreatedDate = DateTime.UtcNow,
                CreatedUserName = "Subscriber Setup",
                CrmAdminEmail = subscriberSetup.Email,
                DataCenter = subscriberSetup.DataCenter,
                DefaultDateFormat = "dd/MM/yyyy",
                DefaultLeadResponseDays = 2,
                DefaultReportCurrencyCode = "USD",
                DefaultReportDateFormat = "DD-MMM-YY",
                DefaultShippingFrequency = "Per Month",
                Email = subscriberSetup.Email,
                LastUpdate = DateTime.UtcNow,
                LogonUrl = subscriberSetup.Subdomain + ".firstfreight.com",
                SubDomain = subscriberSetup.Subdomain,
                SubscriberId = subscriberId,
                UpdateUserId = 9999,
                UpdateUserName = "Subscriber Setup",
            };
            context.Subscribers.InsertOnSubmit(subscriber);
            context.SubmitChanges();

            return subscriberId;
        }


        private static string SetupDefaultCompanyLinkTypes(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultCompanyLinkTypes already exist for this subscriber
            var aCompanyLinkTypes = context.CompanyLinkTypes.FirstOrDefault(s => s.SubscriberId == newSubscriberId);
            if (aCompanyLinkTypes == null)
            {

                // Get Recordset of Default Company Link Types (Subscriber 0)
                var defaultCompanyLinkTypes = context.CompanyLinkTypes.Where(c => c.SubscriberId == 0)
                    .OrderBy(c => c.CompanyLinkTypeName).ToList();

                // Iterate through list and add records
                foreach (var companyLinkType in defaultCompanyLinkTypes)
                {
                    // Set Company Link Type Properties
                    var newCompanyLinkType = new CompanyLinkType()
                    {
                        CompanyLinkTypeName = companyLinkType.CompanyLinkTypeName,
                        SubscriberId = newSubscriberId,
                    };
                    // Add Default Company Link Type for SubscriberId
                    context.CompanyLinkTypes.InsertOnSubmit(newCompanyLinkType);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Company Link Type Records";
            }
            else
            {
                returnValue = "Default Company Link Type Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultCompanySegments(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultCompanySegments already exist for this subscriber
            var aSegments = context.CompanySegments.FirstOrDefault(s => s.SubscriberId == newSubscriberId && !s.Deleted);
            if (aSegments == null)
            {
                // Get Recordset of Default Company Segments (Subscriber 100)
                var defaultCompanySegments = context.CompanySegments.Where(s => !s.Deleted && s.SubscriberId == 0)
                    .OrderBy(s => s.SegmentCode).ToList();

                // Iterate through list and add records
                foreach (var companySegment in defaultCompanySegments)
                {
                    // Set Company Segment Properties
                    var newCompanySegment = new CompanySegment
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SegmentCode = companySegment.SegmentCode,
                        SegmentName = companySegment.SegmentName,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Account Segment for SubscriberId
                    context.CompanySegments.InsertOnSubmit(newCompanySegment);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Companyh Segment Records";
            }
            else
            {
                returnValue = "Default Company Segment Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultCompanyTypes(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultCompanyTypes already exist for this subscriber
            var aCompanyTypes = context.CompanyTypes.FirstOrDefault(s => s.SubscriberId == newSubscriberId && !s.Deleted);
            if (aCompanyTypes == null)
            {

                // Get Recordset of Default Company Types (Subscriber 0)
                var defaultCompanyTypes = context.CompanyTypes.Where(c => !c.Deleted && c.SubscriberId == 0)
                    .OrderBy(c => c.CompanyTypeName).ToList();

                // Iterate through list and add records
                foreach (var companyType in defaultCompanyTypes)
                {
                    // Set Company Type Properties
                    var newCompanyType = new CompanyType()
                    {
                        CompanyTypeName = companyType.CompanyTypeName,
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Company Type for SubscriberId
                    context.CompanyTypes.InsertOnSubmit(newCompanyType);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Company Type Records";
            }
            else
            {
                returnValue = "Default Company Type Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultContactTypes(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultContactTypes already exist for this subscriber
            var aContactTypes = context.ContactTypes.FirstOrDefault(c => c.SubscriberId == newSubscriberId && !c.Deleted);
            if (aContactTypes == null)
            {

                // Get Recordset of Default Contact Types (Subscriber 0)
                var defaultContactTypes = context.ContactTypes.Where(c => !c.Deleted && c.SubscriberId == 0)
                    .OrderBy(c => c.ContactTypeName).ToList();

                // Iterate through list and add records
                foreach (var contactType in defaultContactTypes)
                {
                    // Set Contact Type Properties
                    var newContactType = new ContactType
                    {
                        ContactTypeName = contactType.ContactTypeName,
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SortOrder = contactType.SortOrder,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Contact Type for SubscriberId
                    context.ContactTypes.InsertOnSubmit(newContactType);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Contact Type Records";
            }
            else
            {
                returnValue = "Default Contact Type Records Already Exist";
            }
            return returnValue;
        }
 

        private static string SetupDefaultDealTypes(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultDealypes already exist for this subscriber
            var aDealTypes = context.DealTypes.FirstOrDefault(s => s.SubscriberId == newSubscriberId && !s.Deleted);
            if (aDealTypes == null)
            {

                // Get Recordset of Default Deal Types (Subscriber 0)
                var defaultDealTypes = context.DealTypes.Where(d => !d.Deleted && d.SubscriberId == 0)
                    .OrderBy(d => d.SortOrder).ToList();

                // Iterate through list and add records
                foreach (var defaultDealType in defaultDealTypes)
                {
                    // Set Deal Type Properties
                    var dealType = new DealType
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        DealTypeName = defaultDealType.DealTypeName,
                        SortOrder = defaultDealType.SortOrder,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Deal Type for SubscriberId
                    context.DealTypes.InsertOnSubmit(dealType);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Deal Type Records";
            }
            else
            {
                returnValue = "Default Deal Type Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultIndustries(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultIndustries already exist for this subscriber
            var aIndustries = context.Industries.FirstOrDefault(i => i.SubscriberId == newSubscriberId && !i.Deleted);
            if (aIndustries == null)
            {

                // Get Recordset of Default Industry Sectors (Subscriber 0)
                var defaultIndustries = context.Industries.Where(i => !i.Deleted && i.SubscriberId == 0)
                    .OrderBy(i => i.IndustryName).ToList();

                // Iterate through list and add records
                foreach (var industry in defaultIndustries)
                {
                    // Set Industry Sector Properties
                    var newIndustry = new Industry
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        IndustryName = industry.IndustryName,
                        LastUpdate = DateTime.UtcNow,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Industry Sector for SubscriberId
                    context.Industries.InsertOnSubmit(newIndustry);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Industry Records";
            }
            else
            {
                returnValue = "Default Industry Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultLostReasons(int newSubscriberId, string connection)
        {
            string returnValue = "";
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultDealWonLostReasons already exist for this subscriber
            var aLostReasons = context.LostReasons.FirstOrDefault(w => w.SubscriberId == newSubscriberId && !w.Deleted);
            if (aLostReasons == null)
            {

                // Get Recordset of Default Won Lost Reasons (Subscriber 0)
                var defaultLostReasons = context.LostReasons.Where(w => !w.Deleted && w.SubscriberId == 0)
                    .OrderBy(w => w.LostReasonName).ToList();

                // Iterate through list and add records
                foreach (var defaultLostReason in defaultLostReasons)
                {
                    // Set Won Lost Reason Properties
                    var newLostReason = new LostReason
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SortOrder = defaultLostReason.SortOrder,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup",
                        LostReasonName = defaultLostReason.LostReasonName
                    };
                    // Add Default Lost Reason for SubscriberId
                    context.LostReasons.InsertOnSubmit(newLostReason);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Lost Reason Records";
            }
            else
            {
                returnValue = "Default Lost Reason Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultRegions(int newSubscriberId, string sharedConnection)
        {
            string returnValue;
            var recordsAdded = 0;
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // Regions
            // Check to see if DefaultRegions already exist for this subscriber
            var aRegions = sharedContext.Regions.FirstOrDefault(w => w.SubscriberId == newSubscriberId && !w.Deleted);
            if (aRegions == null)
            { 
                // Get Recordset of Default Regions (Subscriber 0)
                var defaultRegions = sharedContext.Regions.Where(r => !r.Deleted && r.SubscriberId == 0)
                                                  .OrderBy(r => r.RegionName).ToList();

                // Iterate through list and add records
                foreach (var defaultRegion in defaultRegions)
                {
                    // Set Region Properties
                    var newRegion = new Region
                    {
                        CreatedUserIdGlobal = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow, 
                        RegionName = defaultRegion.RegionName,
                        SubscriberId = newSubscriberId,
                        UpdateUserIdGlobal = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Deal Type for SubscriberId
                    sharedContext.Regions.InsertOnSubmit(newRegion);
                    sharedContext.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Region Records";
            }
            else
            {
                returnValue = "Default Region Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultSalesStages(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultSalesStages already exist for this subscriber
            var aSalesStages = context.SalesStages.FirstOrDefault(s => s.SubscriberId == newSubscriberId && !s.Deleted);
            if (aSalesStages == null)
            {

                // Get Recordset of Default Sales Stages (Subscriber 0)
                var defaultSalesStages =
                    context.SalesStages.Where(s => !s.Deleted && s.SubscriberId == 0)
                                       .OrderBy(s => s.SalesStageName).ToList();

                // Iterate through list and add records
                foreach (var defaultSalesStage in defaultSalesStages)
                {
                    // Set Sales Stage Properties
                    var newSalesStage = new SalesStage
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SalesStageName = defaultSalesStage.SalesStageName,
                        SortOrder = defaultSalesStage.SortOrder,
                        StagePercentage = defaultSalesStage.StagePercentage,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Sales Stages for SubscriberId
                    context.SalesStages.InsertOnSubmit(newSalesStage);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Sales Stage Records";
            }
            else
            {
                returnValue = "Default Sales Stage Records Already Exist";
            }
            return returnValue;
        }


        private static string SetupDefaultSources(int newSubscriberId, string connection)
        {
            string returnValue;
            var recordsAdded = 0;
            var context = new DbFirstFreightDataContext(connection);

            // Check to see if DefaultSources already exist for this subscriber
            var aSources = context.Sources.FirstOrDefault(s => s.SubscriberId == newSubscriberId && !s.Deleted);
            if (aSources == null)
            {

                // Get Recordset of Default Sources (Subscriber 0)
                var defaultSources = context.Sources.Where(s => !s.Deleted && s.SubscriberId == 0)
                    .OrderBy(s => s.SourceName).ToList();

                // Iterate through list and add records
                foreach (var source in defaultSources)
                {
                    // Set Sources Properties
                    var newSource = new Source
                    {
                        CreatedUserId = 9999,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = "Subscriber Setup",
                        LastUpdate = DateTime.UtcNow,
                        SourceName = source.SourceName,
                        SortOrder = source.SortOrder,
                        SubscriberId = newSubscriberId,
                        UpdateUserId = 9999,
                        UpdateUserName = "Subscriber Setup"
                    };
                    // Add Default Sources for SubscriberId
                    context.Sources.InsertOnSubmit(newSource);
                    context.SubmitChanges();
                    recordsAdded += 1;
                }
                returnValue = "Created " + recordsAdded + " Default Source Records";
            }
            else
            {
                returnValue = "Default Source Records Already Exist";
            }
            return returnValue;
        }


        private static int AddNewSubscriberUser(User ud, string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            // Add New Subscriber User Record
            var user = new User
            {
                AdminUser = ud.AdminUser,
                CompanyRoster = false,
                CountryCode = ud.CountryCode,
                CountryName = ud.CountryName,
                CreatedUserId = ud.CreatedUserId,
                CreatedDate = DateTime.UtcNow,
                CreatedUserName = ud.CreatedUserName,
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                DataCenter = ud.DataCenter,
                DateFormat = "dd/MM/yyyy",
                DateFormatReports = "DD-MMM-YY",
                DisplayLanguage = ud.DisplayLanguage,
                EmailAddress = ud.EmailAddress,
                FirstName = ud.FirstName,
                FullName = ud.LastName,
                LanguageCode = "en-US",
                LastName = ud.LastName,
                LastUpdate = DateTime.UtcNow,
                LoginEnabled = true,
                Password = ud.Password,
                ReportDateFormat = "DD-MMM-YY",
                SubscriberId = ud.SubscriberId,
                SyncType = "Do Not Sync",
                TimeZone = ud.TimeZone, 
                UpdateUserId = ud.UpdateUserId,
                UpdateUserName = ud.UpdateUserName,
                UserActivationDate = DateTime.UtcNow,
                UserRoles = ud.UserRoles
            };
            context.Users.InsertOnSubmit(user);
            context.SubmitChanges();
            var userId = ud.UserId;
            return userId;
        }

    }
}
