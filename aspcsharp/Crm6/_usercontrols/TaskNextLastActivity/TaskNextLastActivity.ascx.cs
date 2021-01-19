using System;
using System.Web.UI;
using Helpers;
using Models;

namespace Crm6._usercontrols.TaskNextLastActivity
{
    public partial class TaskNextLastActivity : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var userId = currentUser.User.UserId;
            if (!Page.IsPostBack)
            {
                var subscriberId = currentUser.User.SubscriberId;
                string[] qsNames = {"subscriberId","dealsubscriberId"};
                for (var i=0;i<qsNames.Length;i++) {
                    var qsVal = Request.QueryString[qsNames[i]];
                    if (!string.IsNullOrEmpty(qsVal)) {
                        subscriberId = int.Parse(qsVal);
                        break;
                    }
                }
                var contactId = Request.QueryString["contactId"];
                var companyId = Request.QueryString["companyId"];
                var dealId = Request.QueryString["dealId"];
                if (!string.IsNullOrEmpty(contactId)) {
                    LoadNextLastActivites(new Props_LoadNextLastActivites {
                        SubscriberId = subscriberId,
                        UserId = userId,
                        ContactId = int.Parse(contactId)
                    });
                }
                else if (!string.IsNullOrEmpty(companyId)) {
                    var company = new Helpers.Companies().GetCompany(int.Parse(companyId), subscriberId);
                    LoadNextLastActivites(new Props_LoadNextLastActivites {
                        SubscriberId = subscriberId,
                        UserId = userId,
                        GlobalCompanyId = company.CompanyIdGlobal,
                        CompanyId = int.Parse(companyId)
                    });
                }
                else if (!string.IsNullOrEmpty(dealId)) {
                    LoadNextLastActivites(new Props_LoadNextLastActivites {
                        SubscriberId = subscriberId,
                        UserId = userId,
                        DealId = int.Parse(dealId)
                    });
                }
            }
        }
        
        private class Props_LoadNextLastActivites
        {
            public int SubscriberId { get; set; }
            public int UserId { get; set; }
            public int GlobalUserId { get; set; }
            public int OwnerlUserId { get; set; }
            public int ContactId { get; set; }
            public int GlobalCompanyId { get; set; }
            public int CompanyId { get; set; }
            public int DealId  { get; set; }
        }
        private void LoadNextLastActivites(Props_LoadNextLastActivites props)
        {
            var activitesFilter = new ActivitiesFilter
            {
                SubscriberId = props.SubscriberId,
                UserId = props.UserId,
                ContactId = props.ContactId,
                CompanyId = props.CompanyId,
                CompanyIdGlobal = props.GlobalCompanyId,
                DealId = props.DealId,
                getNextLast = true,
                MatchActive = true
            };
            var activitesResponse = new Helpers.Activities().GetActivities(activitesFilter);
            var lastActivity = activitesResponse.LastActivity;
            var nextActivity = activitesResponse.NextActivity;
            
            if (nextActivity != null)
            {
                lblNextActivityTaskType.Text = nextActivity.ActivityType.ToUpper();
                var isEvent = nextActivity.ActivityType.ToLower()=="event";
                lblUpcomingEventDescription.Text = isEvent?nextActivity.Subject:nextActivity.TaskName;
                var upcomingDate = isEvent?nextActivity.StartDateTime:nextActivity.DueDate;
                lblUpcomingEventTime.Text = upcomingDate.Value.ToString("ddd, dd-MMM-yy");
                linkNextActivity.Attributes.Add("data-action", "edit-"+(isEvent?"event":"task"));
                if (isEvent)
                {
                    if (nextActivity.EndDateTime != null)
                        lblUpcomingEventStartEndTime.Text = nextActivity.StartDateTime.Value.ToString("HH:mm") + " - " +
                            nextActivity.EndDateTime.Value.ToString("HH:mm") + " " +
                            nextActivity.EventTimeZone;
                    lblUpcomingEventLocation.Text = nextActivity.Location;
                    lblUpcomingEventType.Text = nextActivity.EventType;
                    linkNextActivity.Attributes.Add("event-id", nextActivity.ActivityId.ToString());
                    linkNextActivity.Attributes.Add("event-subscriber-id", nextActivity.SubscriberId.ToString());    
                }
                else {
                    wrpLocationAndType.Visible = false;
                    linkNextActivity.Attributes.Add("data-toggle", "modal");
                    linkNextActivity.Attributes.Add("data-target", "#addTaskDialog");
                    linkNextActivity.Attributes.Add("data-modal-props", "{" +
                        "\"type\": \"edit\"," +
                        "\"activityId\":" +nextActivity.ActivityId.ToString() + "," +
                        "\"subscriberId\":" +nextActivity.SubscriberId.ToString() +
                    "}");
                }
            }
            else nextCard.Visible = false;
            
            if (lastActivity != null)
            {   
                lblLastActivityTaskType.Text = lastActivity.ActivityType.ToUpper();
                lblLastActivityCreatedBy.Text ="by " + lastActivity.CreatedUserName;
                lblLastActivityCreatedDate.Text = "Created " + lastActivity.CreatedDate.ToString("dd-MMM-yy");
                wrpNote.Visible = false;
                switch (lastActivity.ActivityType.ToLower()) {
                    case "event":
                        lblLastActivitySubject.Text = lastActivity.Subject;
                        linkLastActivity.Attributes.Add("data-action", "edit-event");
                        linkLastActivity.Attributes.Add("event-id", lastActivity.ActivityId.ToString());
                        linkLastActivity.Attributes.Add("event-subscriber-id", lastActivity.SubscriberId.ToString()); 
                        break;
                    case "task":
                        lblLastActivitySubject.Text = lastActivity.TaskName;
                        linkLastActivity.Attributes.Add("data-action", "edit-task");
                        linkLastActivity.Attributes.Add("data-toggle", "modal");
                        linkLastActivity.Attributes.Add("data-target", "#addTaskDialog");
                        linkLastActivity.Attributes.Add("data-modal-props", "{" +
                            "\"type\": \"edit\"," +
                            "\"activityId\":" +lastActivity.ActivityId.ToString() + "," +
                            "\"subscriberId\":" +lastActivity.SubscriberId.ToString() +
                        "}");
                        break;
                    case "note":
                        var maxLen = 100;
                        lblNote.Text = lastActivity.NoteContent.Length>maxLen ? lastActivity.NoteContent.Substring(0,maxLen-3)+"..." : lastActivity.NoteContent;
                        linkLastActivity.Visible = false;
                        wrpNote.Visible = true;
                        break;
                }
            }
            else activityCard.Visible = false;
        }
    }
}