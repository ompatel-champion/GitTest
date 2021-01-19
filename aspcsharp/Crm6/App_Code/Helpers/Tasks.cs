using Crm6.App_Code;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Shared;
using Crm6.App_Code.Login;

namespace Helpers
{

    public class Tasks
    {

        public List<Activity> GetTasks(TaskFilter filters)
        {
            var finalList = new List<Activity>();
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                .GlobalSubscribers.Where(t => t.SubscriberId == filters.SubscriberId)
                .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // user data context  
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var currentUser = userContext.Users.Where(t => t.UserId == filters.UserId).FirstOrDefault();
            var userTimeZone = currentUser.TimeZone;

            var utcOffsetDefault = "";
            var timezone = sharedContext.TimeZones.FirstOrDefault(t => t.TimeZoneName == userTimeZone);
            if (timezone != null)
            {
                TimeZoneInfo cstZone = null;
                if (!string.IsNullOrWhiteSpace(timezone.EnumTimeZoneID))
                {
                    cstZone = TimeZoneInfo.FindSystemTimeZoneById(timezone.EnumTimeZoneID.Trim());
                }
                utcOffsetDefault = timezone.UtcOffset;

                var activities = sharedContext.Activities.Where(t => !t.Deleted && (t.TaskId > 0 || t.ActivityType == "TASK")).Select(t => t);
                if (filters.SubscriberId > 0)
                {
                    var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                        .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                        .Select(s => s.LinkedSubscriberId)
                        .ToList();
                    activities = activities.Where(a => linkedSubscribers.Contains(a.SubscriberId));
                }

                if (filters.CompanyIdGlobal > 0) activities = activities.Where(i => i.CompanyIdGlobal == filters.CompanyIdGlobal);
                else if (filters.CompanyId > 0) activities = activities.Where(i => i.CompanyId == filters.CompanyId);
                if (filters.DealId > 0) {
                    var dealIdStr = filters.DealId.ToString();
                    activities = activities.Where(i => i.DealIds == dealIdStr ||
                        i.DealIds.Contains("," + dealIdStr + ",") ||
                        i.DealIds.StartsWith(dealIdStr + ",") ||
                        i.DealIds.EndsWith("," + dealIdStr));
                }
                if (filters.ContactId > 0) {
                    var contactIdStr = filters.ContactId.ToString();
                    activities = activities.Where(i => i.ContactIds == contactIdStr ||
                        i.DealIds.Contains("," + contactIdStr + ",") ||
                        i.DealIds.StartsWith(contactIdStr + ",") ||
                        i.DealIds.EndsWith("," + contactIdStr));
                }

                if (filters.DueDateFrom != null) activities = activities.Where(t => t.DueDate >= filters.DueDateFrom.Value);
                if (filters.DueDateTo != null) activities = activities.Where(t => t.DueDate <= filters.DueDateTo.Value);
                if (filters.Completed != null) activities = activities.Where(t => t.Completed == filters.Completed.Value);
                if (filters.UserIdGlobal > 0) activities = activities.Where(t => t.OwnerUserIdGlobal == filters.UserIdGlobal);
                if (!string.IsNullOrEmpty(filters.SortBy))
                {
                    switch (filters.SortBy)
                    {
                        case "createddate asc":
                            activities = activities.OrderBy(t => t.CreatedDate);
                            break;
                        case "createddate desc":
                            activities = activities.OrderByDescending(t => t.CreatedDate);
                            break;
                        case "duedate asc":
                            activities = activities.OrderBy(t => t.DueDate);
                            break;
                        case "duedate desc":
                            activities = activities.OrderByDescending(t => t.DueDate);
                            break;
                        default:
                            activities = activities.OrderBy(t => t.CreatedDate);
                            break;
                    }
                }

                // paging
                if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0) activities = activities.Skip(filters.RecordsPerPage * (filters.CurrentPage - 1)).Take(filters.RecordsPerPage);
                foreach (var t in activities)
                {
                    if (cstZone != null) t.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(t.CreatedDate, cstZone);
                    else t.CreatedDate = new Timezones().ConvertUtcToUserDateTime(t.CreatedDate, filters.LoggedinUserId);
                    finalList.Add(t);
                }
            }
            return finalList;
        }


        public ActivityModel GetTask(int activityId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(connection);
            var task = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (task != null)
            {
                var activityModel = new ActivityModel
                {
                    Task = task,
                    Invites = GetTaskContacts(task.ActivityId, subscriberId),
                    Deals = new List<Deal>()
                };

                // get deals
                var dealIds = (task.DealIds + "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var dealId in dealIds)
                {
                    var deal = new Deals().GetDeal(int.Parse(dealId), task.CompanySubscriberId);
                    if (deal != null)
                    {
                        activityModel.Deals.Add(deal);
                    }
                }

                return activityModel;
            }
            return null;
        }



        public List<ActivititesMember> GetTaskContacts(int activityId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return sharedContext.ActivititesMembers.Where(t => t.ActivitiesId == activityId && t.ContactId > 0 && !t.Deleted).ToList();
        }



        public int SaveTask(ActivityModel taskItem)
        {
            var sharedConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(taskItem.Task.SubscriberId);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection());
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var taskCampaigns = new List<string>();

            Crm6.App_Code.Login.GlobalUser user = null;
            var activity = sharedContext.Activities.FirstOrDefault(i => i.ActivityId == taskItem.Task.ActivityId);
            if (activity == null) {
                activity = new Activity();
                activity.UserId = taskItem.Task.UserId;
                activity.UserIdGlobal = taskItem.Task.UserIdGlobal;
                if (taskItem.Task.UserIdGlobal > 0) {
                    user = loginContext.GlobalUsers.FirstOrDefault(i => i.GlobalUserId == activity.UserIdGlobal);
                    if (user != null) {
                        activity.UserId = user.UserId;
                        activity.UserName = user.FullName ?? "";
                        activity.UserTimeZone = user.TimeZone;
                    }
                }
            }
            activity.OwnerUserIdGlobal = taskItem.Task.OwnerUserIdGlobal;
            if (activity.OwnerUserIdGlobal > 0)
            {
                if (user==null || user.GlobalUserId!=activity.OwnerUserIdGlobal) user = loginContext.GlobalUsers.FirstOrDefault(i => i.GlobalUserId == activity.OwnerUserIdGlobal);
                if (user != null)
                {
                    activity.OwnerUserId = user.UserId;
                    activity.OwnerUserName = user.FullName ?? "";
                    activity.UserLocation = user.LocationName;
                }
            }
            activity.UpdateUserIdGlobal = taskItem.Task.UpdateUserIdGlobal;
            if (activity.UpdateUserIdGlobal > 0) {
                if (user==null || user.GlobalUserId!=activity.UpdateUserIdGlobal) user = loginContext.GlobalUsers.FirstOrDefault(i => i.GlobalUserId == taskItem.Task.UpdateUserIdGlobal);
                if (user != null)
                {
                    activity.UpdateUserId = user.UserId;
                    activity.UpdateUserName = user.FullName ?? "";
                }
            }
            activity.TaskName = taskItem.Task.TaskName;
            activity.Description = taskItem.Task.Description;
            activity.ActivityType = "TASK";
            activity.DealIds = taskItem.Task.DealIds;
            activity.DueDate = taskItem.Task.DueDate;
            if (taskItem.Task.DueDate.HasValue)
                activity.ActivityDate = taskItem.Task.DueDate.Value;

            // company details
            var companySubscriberId = 0;
            if (taskItem.Task.CompanyIdGlobal > 0)
            {
                activity.CompanyIdGlobal = taskItem.Task.CompanyIdGlobal;
                // get global company  
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(i => i.GlobalCompanyId == taskItem.Task.CompanyIdGlobal);
                if (globalCompany != null)
                {
                    companySubscriberId = globalCompany.SubscriberId;
                    activity.CompanyId = globalCompany.CompanyId;
                    activity.CompanyName = globalCompany.CompanyName;
                    activity.CompanySubscriberId = companySubscriberId;
                    var company = context.Companies.FirstOrDefault(t => t.CompanyIdGlobal == taskItem.Task.CompanyIdGlobal);
                    if (company != null && !string.IsNullOrWhiteSpace(company.CampaignName))
                    {
                        // company campaigns
                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (campaigns.Count > 0)
                        {
                            taskCampaigns.AddRange(campaigns);
                        }
                    }
                }
            }
            else
            {
                activity.CompanyIdGlobal = 0;
                activity.CompanyId = 0;
                activity.CompanyName = "";
            }

            activity.Completed = taskItem.Task.Completed;
            activity.CompletionPercent = taskItem.Task.Completed ? 100 : 0;
            activity.LastUpdate = DateTime.UtcNow;
            
            activity.Campaigns = string.Join(",", taskCampaigns.Distinct().ToList());

            // insert new task
            if (activity.ActivityId < 1)
            {
                activity.SubscriberId = taskItem.Task.SubscriberId;
                activity.CreatedDate = activity.LastUpdate;
                activity.CreatedUserIdGlobal = taskItem.Task.UpdateUserIdGlobal;
                activity.CreatedUserName = activity.UpdateUserName ?? "";
                sharedContext.Activities.InsertOnSubmit(activity);
            }
            context.SubmitChanges();
            sharedContext.SubmitChanges();

            // deal details
            taskItem.Task.DealIds = activity.DealIds;
            // delete current linked deals
            var activityDeals = sharedContext.LinkActivityToDeals.Where(t => t.ActivityId == activity.ActivityId).ToList();
            if (activityDeals.Count > 0)
            {
                sharedContext.LinkActivityToDeals.DeleteAllOnSubmit(activityDeals);
                sharedContext.SubmitChanges();
            }

            if (!string.IsNullOrEmpty(activity.DealIds))
            {
                var dealIds = activity.DealIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var dealNames = new List<string>();
                var dealCompetitors = new List<string>();
                var dealTypes = new List<string>();
                foreach (var dealId in dealIds)
                {
                    var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == companySubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();
                    var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
                    var dealContext = new DbFirstFreightDataContext(connection);
                    var deal = dealContext.Deals.FirstOrDefault(u => u.DealId == int.Parse(dealId) && u.SubscriberId == activity.CompanySubscriberId);
                    if (deal != null)
                    {
                        dealNames.Add(deal.DealName);
                        dealCompetitors.Add(deal.Competitors);
                        dealTypes.Add(deal.DealType);

                        if (!string.IsNullOrWhiteSpace(deal.Campaign))
                        {
                            // deal campaigns
                            var campaigns = deal.Campaign.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (campaigns.Count > 0)
                                taskCampaigns.AddRange(campaigns);
                        }

                        new Deals().UpdateDealLastUpdateDate(int.Parse(dealId), activity.UpdateUserId, activity.CompanySubscriberId);

                        // add activity deals
                        sharedContext.LinkActivityToDeals.InsertOnSubmit(new LinkActivityToDeal
                        {
                            ActivityId = activity.ActivityId,
                            DealId = int.Parse(dealId),
                            DealName = deal.DealName,
                            SubscriberId = companySubscriberId,
                            DealSubscriberId = companySubscriberId,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = activity.UpdateUserId,
                            LastUpdate = DateTime.Now,
                            UpdateUserId = activity.UpdateUserId,
                            UpdateUserName = activity.UpdateUserName,
                            CreatedUserName = activity.UpdateUserName
                        });
                        sharedContext.SubmitChanges();
                    }
                }

                activity.DealNames = string.Join(",", dealNames);
                activity.DealTypes = string.Join(",", dealTypes);
                activity.Competitors = string.Join(",", dealCompetitors);
                activity.Campaigns = string.Join(",", taskCampaigns.Distinct().ToList());
            }

            // add task contacts    
            if (taskItem.Invites == null) taskItem.Invites = new List<ActivititesMember>();
            var invites = AddActivityContacts(activity, taskItem.Invites);
            // set task contact ids and contact names
            if (invites != null)
            {
                activity.ContactIds = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactId.ToString()));
                activity.ContactNames = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactName.ToString()));
            }
            sharedContext.SubmitChanges();
            foreach (var invite in invites) new Contacts().UpdateContactLastActivityDate(invite.ContactId ?? default(int));   

            if (activity.CompanyId > 0)
                new Companies().UpdateCompanyLastUpdateDate(activity.CompanyId, activity.UpdateUserId, companySubscriberId);

            var contactId = 0;
            int.TryParse(activity.ContactIds, out contactId);

            var currentDealId = 0;
            int.TryParse(activity.DealIds, out currentDealId);

            // Log Event 
            new Logging().LogUserAction(new UserActivity
            {
                UserId = taskItem.Task.UpdateUserId,
                SubscriberId = activity.SubscriberId,
                TaskName = activity.TaskName,
                CompanyName = activity.CompanyName,
                CompanyId = activity.CompanyId,
                ContactId = contactId,
                DealId = currentDealId,
                DealName = activity.DealNames,
                UserActivityMessage = "Saved Task"
            });

            // log event to intercom
            var eventName = "Saved task";
            var intercomHelper = new IntercomHelper();
            intercomHelper.IntercomTrackEvent(activity.CreatedUserId, activity.SubscriberId, eventName);

            return activity.ActivityId;
        }



        public List<ActivititesMember> AddActivityContacts(Activity taskItem, List<ActivititesMember> contacts)
        {
            try
            {
                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == taskItem.SubscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                // get current contacts
                var currentInvites = sharedContext.ActivititesMembers.Where(t => t.ActivitiesId == taskItem.ActivityId && !t.Deleted).ToList();

                // delete all the removed invites
                foreach (var taskInvite in currentInvites)
                {
                    ActivititesMember found = null;
                    if (taskInvite.ContactId > 0)
                    {
                        found = contacts.FirstOrDefault(t => t.ContactId == taskInvite.ContactId && t.ContactSubscriberId == taskInvite.ContactSubscriberId && !t.Deleted);
                    }


                    // if not found - invite has been deleted
                    if (found == null)
                    {
                        taskInvite.Deleted = true;
                        taskInvite.DeletedDate = DateTime.Now;
                        taskInvite.DeletedUserId = taskItem.UpdateUserId;
                        taskInvite.DeletedUserName = taskItem.UpdateUserName;
                        sharedContext.SubmitChanges();
                    }
                }

                if (contacts != null)
                {
                    // add new invites
                    foreach (var contact in contacts)
                    {
                        ActivititesMember invite = null;
                        if (contact.ContactId > 0)
                        {
                            invite = sharedContext.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == taskItem.ActivityId && t.ContactId == contact.ContactId && t.ContactSubscriberId == contact.ContactSubscriberId && !t.Deleted);
                        }

                        if (invite == null)
                        {
                            invite = new ActivititesMember
                            {
                                ActivitiesId = taskItem.ActivityId,
                                InviteType = contact.InviteType,
                                ContactId = contact.ContactId,
                                ContactName = contact.ContactName,
                                ContactSubscriberId = contact.ContactId > 0 ? contact.SubscriberId : 0,
                                SubscriberId = taskItem.SubscriberId,
                                CreatedUserId = taskItem.UpdateUserId,
                                CreatedDate = DateTime.UtcNow,
                                CreatedUserName = taskItem.UpdateUserName,
                                LastUpdate = DateTime.UtcNow,
                                UpdateUserId = taskItem.UpdateUserId,
                                UpdateUserName = taskItem.UpdateUserName,
                                AttendeeType = contact.AttendeeType,
                                UserIdGlobal = taskItem.UserIdGlobal,
                                UserName = taskItem.UserName,
                                UserId = taskItem.UserId,
                                
                            };
                            sharedContext.ActivititesMembers.InsertOnSubmit(invite);
                            sharedContext.SubmitChanges();
                        }
                        else
                        {
                            invite.LastUpdate = DateTime.UtcNow;
                            invite.UpdateUserId = taskItem.UpdateUserId;
                            invite.UpdateUserName = taskItem.UpdateUserName;
                            invite.AttendeeType = contact.AttendeeType;
                            sharedContext.SubmitChanges();
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
            return contacts;
        }
        

        public bool DeleteTask(int activityId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var activity = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (activity != null)
            {
                activity.Deleted = true;
                activity.DeletedUserId = userId;
                activity.DeletedDate = DateTime.UtcNow;
                activity.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();

                var contactId = 0;
                int.TryParse(activity.ContactIds, out contactId);

                var currentDealId = 0;
                int.TryParse(activity.DealIds, out currentDealId);

                // Log Event 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = userId,
                    TaskName = activity.TaskName,
                    CompanyName = activity.CompanyName,
                    SubscriberId = activity.SubscriberId,
                    CompanyId = activity.CompanyId,
                    DealId = currentDealId,
                    ContactId = contactId,
                    DealName = activity.DealNames,
                    UserActivityMessage = "Deleted Task: " + activity.TaskName
                });

                return true;
            }
            return false;
        }

        public bool CompleteTask(int activityId, int subscriberId, int userId, bool revert=false)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();

            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var fTask = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (fTask != null)
            {
                fTask.Completed = revert?false:true;
                fTask.LastUpdate = DateTime.UtcNow;
                fTask.UpdateUserId = userId;
                fTask.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool ToggleTaskCompleted(int activityId, bool state, int subscriberId, int userId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var fTask = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (fTask != null)
            {
                fTask.Completed = state;
                fTask.LastUpdate = DateTime.UtcNow;
                fTask.UpdateUserId = userId;
                fTask.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }

    }

}
