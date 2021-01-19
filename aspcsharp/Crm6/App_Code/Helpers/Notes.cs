using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code.Helpers;

namespace Helpers
{
    public class Notes
    {

        public NoteListResponse GetNotes(NoteFilter filters)
        {
            var response = new NoteListResponse
            {
                Notes = new List<Activity>()
            };

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // get notes
            var notes = sharedContext.Activities.Where(a => !a.Deleted && a.ActivityType == "NOTE");

            // apply filters
            if (filters.SubscriberId > 0)
            {
                // get linked subscriber notes as it always filters by the global company id afterwards
                var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                                                   .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                                                   .Select(s => s.LinkedSubscriberId)
                                                   .ToList();
                notes = notes.Where(a => linkedSubscribers.Contains(a.SubscriberId));
            }

            if (filters.DealId > 0)
                notes = notes.Where(a => a.DealIds.Contains(filters.DealId.ToString()) && a.CompanyIdGlobal == filters.GlobalCompanyId);
            else if (filters.ContactId > 0)
                notes = notes.Where(a => a.ContactIds.Contains(filters.ContactId.ToString()) && a.CompanyIdGlobal == filters.GlobalCompanyId);
            else if (filters.GlobalCompanyId > 0)
                notes = notes.Where(a => a.CompanyIdGlobal == filters.GlobalCompanyId);

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower())
                {
                    case "createddate asc":
                        notes = notes.OrderBy(t => t.CreatedDate);
                        break;
                    case "createddate desc":
                        notes = notes.OrderByDescending(t => t.CreatedDate);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                notes = notes.OrderByDescending(t => t.CreatedDate);
            }

            // return notes
            var noteList = notes.Select(a => a).ToList();

            // record count/ total pages 
            var recordCount = noteList.Count;
            var totalPages = 0;
            // apply paging
            if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
            {
                noteList = noteList.Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                                    .Take(filters.RecordsPerPage)
                                    .ToList();
                totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                    (recordCount / filters.RecordsPerPage) :
                                    ((recordCount / filters.RecordsPerPage) + 1);
            }

            foreach (var note in noteList)
            {
                note.CreatedDate = new Timezones().ConvertUtcToUserDateTime(note.CreatedDate, filters.UserId);
            }

            response.Notes = noteList.ToList();

            if (response.Notes.Count > 0)
            {
                response.TotalPages = totalPages;
                response.Records = recordCount;
            }

            // set the return note list
            return response;
        }



        public int SaveNote(Activity note)
        {
            // writeble shared context
            var sharedContext = new DbSharedDataContext(LoginUser.GetWritableSharedConnectionForSubscriberId(note.SubscriberId));
            // login context
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());

            // campaigns list
            var noteCampaigns = new List<string>();

            // get note (Activities)
            var objNote = sharedContext.Activities.FirstOrDefault(d => d.ActivityId == note.ActivityId) ?? new Activity();

            objNote.NoteContent = note.NoteContent;

            // set company
            var companySubscriberId = 0;
            if (note.CompanyIdGlobal > 0)
            {
                objNote.CompanyIdGlobal = note.CompanyIdGlobal;
                // get global company  
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == objNote.CompanyIdGlobal);
                if (globalCompany != null)
                {
                    companySubscriberId = globalCompany.SubscriberId;
                    objNote.CompanyId = globalCompany.CompanyId;
                    objNote.CompanyName = globalCompany.CompanyName;
                    objNote.CompanySubscriberId = companySubscriberId;
                    // data center connection 
                    var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                        .GlobalSubscribers.Where(t => t.SubscriberId == companySubscriberId)
                        .Select(t => t.DataCenter).FirstOrDefault();
                    var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
                    var companyContext = new DbFirstFreightDataContext(connection);
                    var company = companyContext.Companies.FirstOrDefault(t => t.CompanyIdGlobal == objNote.CompanyIdGlobal);
                    if (company != null && !string.IsNullOrWhiteSpace(company.CampaignName))
                    {
                        // company campaigns
                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (campaigns.Count > 0)
                        {
                            noteCampaigns.AddRange(campaigns);
                        }
                    }
                }
            }
            else
            {
                objNote.CompanyId = 0;
                objNote.CompanyIdGlobal = 0;
                objNote.CompanyName = "";
            }

            objNote.ContactNames = note.ContactNames;
            objNote.LastUpdate = DateTime.UtcNow;
            objNote.UpdateUserId = note.UpdateUserId;
            objNote.UpdateUserIdGlobal = note.UpdateUserIdGlobal;
            var updatedUserName = new Users().GetUserFullNameById(objNote.UpdateUserId, note.SubscriberId);
            objNote.UpdateUserName = updatedUserName;
            objNote.Campaigns = string.Join(",", noteCampaigns.Distinct().ToList());

            // insert new note (Activity)
            if (objNote.ActivityId < 1)
            {
                // owner user
                objNote.OwnerUserIdGlobal = note.OwnerUserIdGlobal;
                if (note.OwnerUserIdGlobal > 0)
                {
                    var user = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == objNote.OwnerUserIdGlobal);
                    if (user != null)
                    {
                        objNote.OwnerUserName = user.FullName;
                        objNote.UserLocation = user.LocationName;
                    }
                }

                objNote.ActivityType = "NOTE";
                objNote.SubscriberId = note.SubscriberId;
                objNote.UserId = note.UserId;
                objNote.UserIdGlobal = note.UserIdGlobal;
                objNote.CreatedDate = DateTime.UtcNow;
                objNote.ActivityDate = objNote.CreatedDate;
                objNote.CreatedUserId = objNote.UpdateUserId;
                objNote.CreatedUserIdGlobal = objNote.UpdateUserIdGlobal;
                objNote.CreatedUserLocation = objNote.UserLocation;
                objNote.CreatedUserName = objNote.OwnerUserName;
                sharedContext.Activities.InsertOnSubmit(objNote);
                sharedContext.SubmitChanges();
            }
            
            // add note contacts
            if (note.Invites != null) {
                var invites = new Activities().AddEditActivityMembers(objNote, note.Invites);
                if (invites != null)
                {
                    objNote.ContactIds = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactId.ToString()));
                    objNote.ContactNames = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactName.ToString()));
                    foreach (var invite in invites) new Contacts().UpdateContactLastActivityDate(invite.ContactId ?? default(int));
                }
            }
            else objNote.ContactIds = note.ContactIds;
            
            // deal details
            objNote.DealIds = note.DealIds;
            // delete current linked deals
            var activityDeals = sharedContext.LinkActivityToDeals.Where(t => t.ActivityId == objNote.ActivityId).ToList();
            if (activityDeals.Count > 0)
            {
                sharedContext.LinkActivityToDeals.DeleteAllOnSubmit(activityDeals);
                sharedContext.SubmitChanges();
            }
            
            if (!string.IsNullOrEmpty(objNote.DealIds))
            {
                var dealIds = objNote.DealIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
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

                    var deal = dealContext.Deals.FirstOrDefault(u => u.DealId == int.Parse(dealId) && u.SubscriberId == companySubscriberId);
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
                                noteCampaigns.AddRange(campaigns);
                        }

                        // add activity deals
                        sharedContext.LinkActivityToDeals.InsertOnSubmit(new LinkActivityToDeal
                        {
                            ActivityId = objNote.ActivityId,
                            DealId = int.Parse(dealId),
                            DealName = deal.DealName,
                            SubscriberId = companySubscriberId,
                            DealSubscriberId = companySubscriberId,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = objNote.UpdateUserId,
                            LastUpdate = DateTime.Now,
                            UpdateUserId = objNote.UpdateUserId,
                            UpdateUserName = objNote.UpdateUserName,
                            CreatedUserName = objNote.UpdateUserName
                        });
                        sharedContext.SubmitChanges();

                        // update deal last updated date
                        deal.LastUpdate = DateTime.UtcNow;
                        deal.UpdateUserId = objNote.UpdateUserId;
                        deal.UpdateUserName = objNote.UpdateUserName;
                        dealContext.SubmitChanges();
                    }
                }

                objNote.DealNames = string.Join(",", dealNames);
                objNote.DealTypes = string.Join(",", dealTypes);
                objNote.Competitors = string.Join(",", dealCompetitors);
                objNote.Campaigns = string.Join(",", noteCampaigns.Distinct().ToList());
            }
            sharedContext.SubmitChanges();

            var contactId = 0;
            int.TryParse(note.ContactIds, out contactId);

            var currentDealId = 0;
            int.TryParse(note.DealIds, out currentDealId);

            // Log Event
            new Logging().LogUserAction(new UserActivity
            {
                UserId = note.UpdateUserId,
                CompanyId = objNote.CompanyId,
                ContactId = contactId,
                CompanyName = note.CompanyName,
                ContactName = note.ContactNames,
                DealId = currentDealId,
                DealName = note.DealNames,
                UserActivityMessage = "Saved Note: " + note.NoteContent
            }) ;


            // log event to intercom
            var eventName = "Saved note";
            var intercomHelper = new IntercomHelper();
            intercomHelper.IntercomTrackEvent(note.CreatedUserId, note.SubscriberId, eventName);

            // update last update date of company
            if (note.CompanyId > 0)
                new Companies().UpdateCompanyLastUpdateDate(note.CompanyId, note.UpdateUserId, companySubscriberId);

            // return note Id
            return objNote.NoteId;
        }

        public bool DeleteNote(int activityId, int globalUserId)
        {
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            // get deleting global user
            var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == globalUserId);
            // get deleting note
            var note = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (note != null && globalUser != null)
            {
                note.Deleted = true;
                note.DeletedUserId = globalUser.UserId;
                note.DeletedDate = DateTime.UtcNow;
                note.DeletedUserName = globalUser.FullName;
                note.DeletedUserIdGlobal = globalUser.GlobalUserId;
                sharedContext.SubmitChanges();

                var contactId = 0;
                int.TryParse(note.ContactIds, out contactId);

                var dealId = 0;
                int.TryParse(note.DealIds, out dealId);

                new Logging().LogUserAction(new UserActivity
                {
                    UserId = note.UpdateUserId,
                    CompanyId = note.CompanyId,
                    ContactId = contactId,
                    CompanyName = note.CompanyName,
                    ContactName = note.ContactNames,
                    DealId = dealId,
                    DealName = note.DealNames,
                    UserActivityMessage = "Deleted Note: " + note.NoteContent
                }) ;

                return true;
            }
            return false;
        }
    }
}
