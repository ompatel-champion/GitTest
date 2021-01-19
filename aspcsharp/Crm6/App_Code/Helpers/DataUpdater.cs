using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Helpers
{
    public class DataUpdater
    {

        public void UpdateDealReportFields()
        {
            var cStrings = new List<string>();
            //  cStrings.Add("Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605");
            // cStrings.Add("Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            cStrings.Add("Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            //  cStrings.Add("Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            //cStrings.Add("Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_KWEUS;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            //cStrings.Add("Data Source=hkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            //cStrings.Add("Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Sinotrans;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!");


            foreach (var connectionString in cStrings)
            {
                try
                {
                    // update deals
                    UpdateDeals(connectionString);
                    // update company sales team
                    //UpdateSalesTeam(connectionString);
                }
                catch (Exception ex)
                {
                }

            }





        }

        private void UpdateDeals(string connectionString)
        {
            var context = new DbFirstFreightDataContext(connectionString);

            // get deals
            var deals = context.Deals.Where(t => !t.Deleted && t.OrignCountries == null).ToList();

            foreach (var deal in deals)
            {
                // get lanes
                var lanes = context.Lanes.Where(t => !t.Deleted && t.DealId == deal.DealId && t.SubscriberId == deal.SubscriberId).ToList();

                // Consignee Names
                var consigneeList = lanes.Where(t => !string.IsNullOrEmpty(t.ConsigneeCompany))
                                         .Select(t => t.ConsigneeCompany).Distinct();
                deal.ConsigneeNames = string.Join(", ", consigneeList);
                // Shipper Names
                var shipperNamesList = lanes.Where(t => !string.IsNullOrEmpty(t.ShipperCompany))
                                            .Select(t => t.ShipperCompany).Distinct();
                deal.ShipperNames = string.Join(", ", shipperNamesList);

                // Services
                var servicesList = lanes.Select(t => t.Service).Distinct();
                deal.Services = string.Join(", ", servicesList);


                // set the origins/destination locations
                var originLocationList = new List<string>();
                var destLocationList = new List<string>();
                var originCountryList = new List<string>();
                var destCountryList = new List<string>();



                foreach (var lane in lanes)
                {
                    // origins
                    var strLocation = !string.IsNullOrEmpty(lane.OriginUnlocoCode) ?
                        (lane.OriginUnlocoCode) : (!string.IsNullOrEmpty(lane.OriginIataCode) ? lane.OriginIataCode : "");
                    if (!string.IsNullOrEmpty(strLocation))
                        strLocation = strLocation + " | " + lane.OriginName;
                    if (!originLocationList.Contains(strLocation) && !string.IsNullOrEmpty(strLocation))
                        originLocationList.Add(strLocation);

                    // destinations 
                    strLocation = "";
                    strLocation = !string.IsNullOrEmpty(lane.DestinationIataCode) ? lane.DestinationIataCode :
                                     (!string.IsNullOrEmpty(lane.DestinationUnlocoCode) ? lane.DestinationUnlocoCode : "");
                    if (!string.IsNullOrEmpty(strLocation))
                        strLocation = strLocation + " | " + lane.DestinationName;
                    if (!destLocationList.Contains(strLocation) && !string.IsNullOrEmpty(strLocation))
                        destLocationList.Add(strLocation);

                    // origin countries
                    if (!string.IsNullOrEmpty(lane.OriginCountryName) && !originCountryList.Contains(lane.OriginCountryName))
                        originCountryList.Add(lane.OriginCountryName);

                    // destination countries  
                    if (!string.IsNullOrEmpty(lane.DestinationCountryName) && !destCountryList.Contains(lane.DestinationCountryName))
                        destCountryList.Add(lane.DestinationCountryName);

                    // volumes
                    var volAmount = lane.VolumeAmount;
                    if (!lane.ShippingFrequency.Equals("Per Month") && !lane.ShippingFrequency.Equals("Per Year") && !lane.ShippingFrequency.Equals("Per Week"))
                    {

                        switch (lane.VolumeUnit)
                        {
                            case "LBs":
                                deal.LbsSpot += (int)volAmount;
                                break;
                            case "CBMs":
                                deal.CBMsSpot += (int)volAmount;
                                break;
                            case "TEUs":
                                deal.TEUsSpot += (int)volAmount;
                                break;
                            case "KGs":
                                deal.KgsSpot += (int)volAmount;
                                break;
                            case "Tonnes":
                                deal.TonnesSpot += (int)volAmount;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        volAmount = lane.VolumeAmount;
                        if (lane.ShippingFrequency == "Per Year")
                            volAmount = lane.VolumeAmount / 12;
                        if (lane.ShippingFrequency == "Per Week")
                            volAmount += lane.VolumeAmount * 52 / 12;


                        switch (lane.VolumeUnit)
                        {
                            case "LBs":
                                deal.Lbs += (int)volAmount;
                                break;
                            case "CBMs":
                                deal.CBMs += (int)volAmount;
                                break;
                            case "TEUs":
                                deal.TEUs += (int)volAmount;
                                break;
                            case "KGs":
                                deal.Kgs += (int)volAmount;
                                break;
                            case "Tonnes":
                                deal.Tonnes += (int)volAmount;
                                break;
                            default:
                                break;
                        }
                    }
                }
                var originCountryStr = string.Join(", ", originCountryList);
                var destCountryStr = string.Join(", ", destCountryList);
                var originLocationStr = string.Join(", ", originLocationList);
                var destLocationStr = string.Join(", ", destLocationList);

                deal.OrignLocations = originLocationStr;
                deal.DestinationLocations = destLocationStr;
                deal.OrignCountries = originCountryStr;
                deal.DestinationCountries = destCountryStr;

                context.SubmitChanges();
            }


        }


        private void UpdateSalesTeam(string connectionString)
        {
            var context = new DbFirstFreightDataContext(connectionString);

            var companies = context.Companies.Where(t => !t.Deleted).ToList();

            foreach (var company in companies)
            {
                if (company.CompanyIdGlobal > 0)
                {
                    var subscriberId = company.SubscriberId;
                    var sharedWriteableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                    var sharedContext = new DbSharedDataContext(sharedWriteableConnection);

                    var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(c => c.CompanyId == company.CompanyId && c.SubscriberId == company.SubscriberId);
                    if (globalCompany != null)
                    {

                        // check if the company owner is in link table - if not add
                        if (company.CompanyOwnerUserId > 0)
                        {
                            var loginConnection = LoginUser.GetLoginConnection();
                            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                            var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == company.CompanyOwnerUserId &&
                                                                                       t.SubscriberId == company.SubscriberId);
                            if (globalUser != null)
                            {

                                // add link company user - created user
                                var companyOwner = sharedContext.LinkGlobalCompanyGlobalUsers
                                        .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId &&
                                                             c.GlobalUserId == globalUser.GlobalUserId && !c.Deleted);
                                if (companyOwner == null)
                                {
                                    var companyUser = new LinkGlobalCompanyGlobalUser();
                                    companyUser.GlobalUserName = globalUser.FullName;
                                    companyUser.UserSubscriberId = globalUser.SubscriberId;
                                    companyUser.GlobalUserId = globalUser.GlobalUserId;
                                    companyUser.CreatedBy = globalUser.GlobalUserId;
                                    companyUser.CreatedByName = globalUser.FullName;
                                    companyUser.CreatedDate = DateTime.UtcNow;
                                    companyUser.LastUpdate = DateTime.UtcNow;
                                    companyUser.UpdateUserId = globalUser.GlobalUserId;
                                    companyUser.UpdateUserName = globalUser.FullName;
                                    companyUser.LinkType = "";
                                    companyUser.GlobalCompanyName = globalCompany.CompanyName;
                                    companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                                    companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                                    sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                                    sharedContext.SubmitChanges();
                                }
                            }
                        }

                        // update sales team
                        var usernames = sharedContext.LinkGlobalCompanyGlobalUsers
                                               .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                               .Select(t => t.GlobalUserName).ToList();

                        if (company != null)
                        {
                            company.SalesTeam = string.Join(",", usernames);
                            company.CompanyIdGlobal = globalCompany.GlobalCompanyId;
                            context.SubmitChanges();

                            // update global company sales team  
                            if (globalCompany != null)
                            {
                                globalCompany.SalesTeam = company.SalesTeam;
                                globalCompany.LastActivityDate = DateTime.UtcNow;
                                sharedContext.SubmitChanges();
                            }
                        }
                    }



                }

            }

        }



        public void UpdateActivities()
        {
            UpdateActivityUserLocation();

            return;

            var cStrings = new List<string>();
            //cStrings.Add("Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            //cStrings.Add("Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!");
            cStrings.Add("Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!");

            foreach (var connectionString in cStrings)
            {
                try
                {
                    // update deals
                    UpdateActivitiesForConnection(connectionString);
                }
                catch (Exception ex)
                {
                }

            }

        }


        private void UpdateActivitiesForConnection(string connectionString)
        {
            //var startDate = DateTime.Now.AddMonths(-18);

            //try
            //{
            //    var context = new DbFirstFreightDataContext(connectionString);
            //    var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());
            //    // get events
            //    var events = context.CalendarEvents.Where(t => !t.Deleted && !t.SavedAsActivity && t.CreatedDate > startDate).OrderByDescending(t => t.CreatedDate).ToList();

            //    foreach (var fEvent in events)
            //        try
            //        {
            //            var eventCampaigns = new List<string>();
            //            // deal details 
            //            if (fEvent.DealId > 0)
            //            {
            //                var deal = context.Deals.FirstOrDefault(u => u.DealId == fEvent.DealId);
            //                if (deal != null)
            //                {
            //                    fEvent.DealName = deal.DealName;
            //                    fEvent.DealTypes = deal.DealType;
            //                    fEvent.Competitors = deal.Competitors;
            //                    if (!string.IsNullOrWhiteSpace(deal.Campaign))
            //                    {
            //                        // deal campaigns
            //                        var campaigns = deal.Campaign.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                            eventCampaigns.AddRange(campaigns);
            //                    }
            //                }
            //            }

            //            // owner user 
            //            if (fEvent.OwnerUserId > 0)
            //            {
            //                var user = context.Users.FirstOrDefault(u => u.UserId == fEvent.OwnerUserId);
            //                if (user != null)
            //                {
            //                    fEvent.OwnerUserName = user.FullName;
            //                    fEvent.UserLocation = user.LocationName;
            //                }
            //            }

            //            // company details
            //            if (fEvent.CompanyId > 0)
            //            {
            //                var company = context.Companies.FirstOrDefault(t => t.CompanyId == fEvent.CompanyId && t.SubscriberId == fEvent.SubscriberId);
            //                if (company != null)
            //                {
            //                    fEvent.CompanyId = company.CompanyId;
            //                    fEvent.CompanyName = company.CompanyName;
            //                    fEvent.CompanyIdGlobal = company.CompanyIdGlobal;
            //                    if (!string.IsNullOrWhiteSpace(company.CampaignName))
            //                    {
            //                        // company campaigns
            //                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                        {
            //                            eventCampaigns.AddRange(campaigns);
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                fEvent.CompanyIdGlobal = 0;
            //                fEvent.CompanyId = 0;
            //                fEvent.CompanyName = "";
            //            }

            //            fEvent.Campaigns = string.Join(",", eventCampaigns.Distinct().ToList());
            //            context.SubmitChanges();

            //            // add calendar event invited users and external emails 
            //            var contacts = context.CalendarInvites.Where(t => t.CalendarEventId == fEvent.CalendarEventId && t.ContactId > 0 && !t.Deleted).ToList();
            //            var contactNames = new List<string>();
            //            var contactIds = new List<int>();
            //            foreach (var contact in contacts)
            //            {
            //                var c = context.Contacts.FirstOrDefault(t => t.ContactId == contact.ContactId && t.SubscriberId == fEvent.SubscriberId);
            //                if (c != null)
            //                {
            //                    contactNames.Add(c.FirstName + " " + c.LastName);
            //                    contactIds.Add(c.ContactId);
            //                }
            //            }
            //            fEvent.ContactIds = string.Join(",", contactIds.Select(t => t.ToString()));
            //            fEvent.Contacts = string.Join(",", contactNames);

            //            context.SubmitChanges();

            //            // add activity record
            //            var activityId = new Activities().SaveActivity(new Activity { CalendarEvent = fEvent }, fEvent.SubscriberId);
            //            if (activityId > 0)
            //            {
            //                fEvent.SavedAsActivity = true;
            //            }
            //            context.SubmitChanges();

            //        }
            //        catch (Exception ex)
            //        {
            //        }

            //    // get tasks
            //    var tasks = context.Tasks.Where(t => !t.Deleted && !t.SavedAsActivity && t.CreatedDate > startDate).ToList();

            //    foreach (var task in tasks)
            //    {
            //        try
            //        {
            //            var taskCampaigns = new List<string>();
            //            // task user 
            //            if (task.UserId > 0)
            //            {
            //                var user = context.Users.FirstOrDefault(u => u.UserId == task.UserId);
            //                if (user != null)
            //                {
            //                    task.UserName = user.FullName;
            //                    task.UserLocation = user.LocationName;
            //                }
            //            }

            //            // deal 
            //            if (task.DealId > 0)
            //            {
            //                var deal = context.Deals.FirstOrDefault(u => u.DealId == task.DealId);
            //                if (deal != null)
            //                {
            //                    task.DealName = deal.DealName;
            //                    task.DealTypes = deal.DealType;
            //                    task.Competitors = deal.Competitors;
            //                    if (!string.IsNullOrWhiteSpace(deal.Campaign))
            //                    {
            //                        // deal campaigns
            //                        var campaigns = deal.Campaign.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                            taskCampaigns.AddRange(campaigns);
            //                    }
            //                }
            //            }


            //            if (task.CompanyId > 0)
            //            {
            //                var company = context.Companies.FirstOrDefault(t => t.CompanyId == task.CompanyId && t.SubscriberId == task.SubscriberId);
            //                if (company != null)
            //                {
            //                    task.CompanyId = company.CompanyId;
            //                    task.CompanyName = company.CompanyName;
            //                    task.CompanyIdGlobal = company.CompanyIdGlobal;
            //                    if (!string.IsNullOrWhiteSpace(company.CampaignName))
            //                    {
            //                        // company campaigns
            //                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                        {
            //                            taskCampaigns.AddRange(campaigns);
            //                        }
            //                    }
            //                }
            //            }

            //            task.Campaigns = string.Join(",", taskCampaigns.Distinct().ToList());
            //            context.SubmitChanges();

            //            // add task contacts 
            //            var taskContacts = (from t in context.TaskContacts
            //                                where t.TaskId == task.TaskId && !t.Deleted
            //                                select t).ToList();
            //            var contactNames = new List<string>();
            //            var contactIds = new List<int>();
            //            foreach (var t in taskContacts)
            //            {
            //                var c = context.Contacts.FirstOrDefault(ca => ca.SubscriberId == t.SubscriberId && ca.ContactId == t.ContactId);
            //                if (c != null)
            //                {
            //                    t.FirstName = c.FirstName;
            //                    t.LastName = c.LastName;
            //                    context.SubmitChanges();

            //                    contactNames.Add(t.FirstName + " " + t.LastName);
            //                    contactIds.Add(t.ContactId);
            //                }
            //            }

            //            //set task contact ids and contact names
            //            task.ContactIds = string.Join(",", contactIds.Select(t => t.ToString()));
            //            task.Contacts = string.Join(",", contactNames);
            //            context.SubmitChanges();


            //            // add activity record
            //            var activityId = new Activities().SaveActivity(new Activity { TaskDbVersion = task }, task.SubscriberId);
            //            if (activityId > 0)
            //            {
            //                task.SavedAsActivity = true;
            //            }
            //            context.SubmitChanges();
            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //    }


            //    // get notes 
            //    var notes = context.Notes.Where(t => !t.Deleted && !t.SavedAsActivity && t.CreatedDate > startDate).ToList();

            //    foreach (var objNote in notes)
            //    {
            //        try
            //        {
            //            var noteCampaigns = new List<string>();

            //            // company details
            //            if (objNote.CompanyId > 0)
            //            {
            //                var company = context.Companies.FirstOrDefault(t => t.CompanyId == objNote.CompanyId && t.SubscriberId == objNote.SubscriberId);
            //                if (company != null)
            //                {
            //                    objNote.CompanyId = company.CompanyId;
            //                    objNote.CompanyName = company.CompanyName;
            //                    objNote.CompanyIdGlobal = company.CompanyIdGlobal;
            //                    if (!string.IsNullOrWhiteSpace(company.CampaignName))
            //                    {
            //                        // company campaigns
            //                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                        {
            //                            noteCampaigns.AddRange(campaigns);
            //                        }
            //                    }
            //                }

            //            }
            //            else
            //            {
            //                objNote.CompanyIdGlobal = 0;
            //                objNote.CompanyId = 0;
            //                objNote.CompanyName = "";
            //            }

            //            // contact details  
            //            if (objNote.ContactId > 0)
            //            {
            //                var contact = context.Contacts.FirstOrDefault(u => u.ContactId == objNote.ContactId);
            //                if (contact != null)
            //                {
            //                    objNote.ContactName = contact.FirstName + " " + contact.LastName;
            //                }
            //            }

            //            // deal details 
            //            if (objNote.DealId > 0)
            //            {
            //                var deal = context.Deals.FirstOrDefault(u => u.DealId == objNote.DealId);
            //                if (deal != null)
            //                {
            //                    objNote.DealName = deal.DealName;
            //                    objNote.DealTypes = deal.DealType;
            //                    objNote.Competitors = deal.Competitors;
            //                    if (!string.IsNullOrWhiteSpace(deal.Campaign))
            //                    {
            //                        // deal campaigns
            //                        var campaigns = deal.Campaign.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //                        if (campaigns.Count > 0)
            //                            noteCampaigns.AddRange(campaigns);
            //                    }
            //                }
            //            }

            //            objNote.Campaigns = string.Join(",", noteCampaigns.Distinct().ToList());

            //            if (objNote.CreatedUserId > 0)
            //            {
            //                var user = context.Users.FirstOrDefault(u => u.UserId == objNote.CreatedUserId);
            //                if (user != null)
            //                {
            //                    objNote.CreatedUserLocation = user.LocationName;
            //                }
            //            }

            //            context.SubmitChanges();


            //            // add activity record
            //            var activityId = new Activities().SaveActivity(new Activity { Note = objNote }, objNote.SubscriberId);
            //            if (activityId > 0)
            //            {
            //                objNote.SavedAsActivity = true;
            //            }
            //            context.SubmitChanges();
            //        }
            //        catch (Exception ex)
            //        {
            //        }

            //    }

            //}
            //catch (Exception ex)
            //{
            //}

        }


        private void UpdateActivityUserLocation()
        {
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(LoginUser.GetWritableSharedConnectionForSubscriberId(229));
            var activities = sharedContext.Activities.Where(t => t.CalendarEventId > 0 && !t.Deleted && t.DeletedDate == null).ToList();
            foreach (var activity in activities)
            {
                if (activity.UserId > 0)
                {
                    var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == activity.SubscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
                    var context = new DbFirstFreightDataContext(LoginUser.GetConnectionForDataCenter(subscriberDataCenter));
                    var user = context.Users.FirstOrDefault(u => u.UserId == activity.UserId);
                    if (user != null)
                    {
                        activity.Location = user.LocationName;
                        activity.DeletedDate = DateTime.Now;
                        sharedContext.SubmitChanges();
                    }
                }

            }

        }
    }
}