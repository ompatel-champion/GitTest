using Helpers;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crm6.Tasks
{
    public partial class TaskAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();

            if (!Page.IsPostBack)
            {
                var activitySubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["taskSubscriberId"] != null && Utils.IsNumeric(Request.QueryString["taskSubscriberId"]) && int.Parse(Request.QueryString["taskSubscriberId"]) > 0)
                {
                    activitySubscriberId = int.Parse(Request.QueryString["taskSubscriberId"]);
                }
                lblActivitySubscriberId.Text = activitySubscriberId.ToString();


                if (Request.QueryString["activityId"] != null && int.Parse(Request.QueryString["activityId"]) > 0)
                {
                    lblActivityId.Text = Request.QueryString["activityId"];
                    LoadTask();
                }
                else
                {
                    if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                        LoadDeal(int.Parse(Request.QueryString["dealId"]), activitySubscriberId);
                    else if (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)
                        LoadContact(int.Parse(Request.QueryString["contactId"]), activitySubscriberId);

                    if (Request.QueryString["globalcompanyId"] != null && int.Parse(Request.QueryString["globalcompanyId"]) > 0)
                        LoadCompany(0, int.Parse(Request.QueryString["globalcompanyId"]), activitySubscriberId);

                    if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                        LoadCompany(int.Parse(Request.QueryString["companyId"]), 0, activitySubscriberId);
                }
            }
        }


        private void LoadTask()
        {
            var activityId = int.Parse(lblActivityId.Text);
            if (activityId > 0)
            {
                var taskItem = new Helpers.Tasks().GetTask(activityId, int.Parse(lblActivitySubscriberId.Text));
                if (taskItem != null)
                {
                    txtTaskTitle.Text = taskItem.Task.TaskName;
                    txtTaskDescription.Text = taskItem.Task.Description;
                    txtDueDate.Text = taskItem.Task.DueDate.Value.ToString("dd, MMMM yyyy");
                    chkCompleted.Checked = taskItem.Task.Completed;
                    lblActivitySubscriberId.Text = taskItem.Task.SubscriberId.ToString();

                    // deal
                    //if (taskItem.Deal != null)
                    //{
                    //    var dealtaginuts = new List<AutoComplete> { new AutoComplete { id = taskItem.Deal.DealId, name = taskItem.Deal.DealName } };
                    //    txtTagDealId.Text = JsonConvert.SerializeObject(dealtaginuts);
                    //}


                    // companies
                    if (taskItem.Task.CompanyIdGlobal > 0)
                    {
                        var companytaginuts = new List<AutoComplete> {
                            new AutoComplete {
                                id = taskItem.Task.CompanyIdGlobal,
                                name = taskItem.Task.CompanyName
                            }
                        }.ToList();
                        txtTagCompanyId.Text = JsonConvert.SerializeObject(companytaginuts);
                    }

                    //// contacts
                    //if (taskItem.Contacts != null)
                    //{
                    //    var contacttaginuts = taskItem.Contacts.Select(t => new AutoComplete { id = t.ContactId, name = t.ContactName }).ToList();
                    //    txtTagContactId.Text = JsonConvert.SerializeObject(contacttaginuts);
                    //}
                }
            }
        }


        private void LoadDeal(int dealId, int subscriberId)
        {

            var deal = new Helpers.Deals().GetDeal(dealId, subscriberId);
            if (deal != null)
            {
                // deal
                var dealTagInuts = new List<AutoComplete> { new AutoComplete { id = deal.DealId, name = deal.DealName } };
                txtTagDealId.Text = JsonConvert.SerializeObject(dealTagInuts);

                //deal company
                var companyTagInuts = new List<AutoComplete> { new AutoComplete { id = deal.CompanyId, name = deal.CompanyName } };
                txtTagCompanyId.Text = JsonConvert.SerializeObject(companyTagInuts);
            }
        }


        private void LoadCompany(int companyId, int globalcompanyId, int subscriberId)
        {
            if (globalcompanyId > 0)
            {
                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedContext = new App_Code.Shared.DbSharedDataContext(sharedConnection);
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == globalcompanyId);
                if (globalCompany != null)
                {
                    var companyName = globalCompany.CompanyName + (globalCompany.City != null ? " - " + globalCompany.City : "");
                    var companyTagInuts = new List<AutoComplete> { new AutoComplete { id = globalCompany.GlobalCompanyId, name = companyName } };
                    txtTagCompanyId.Text = JsonConvert.SerializeObject(companyTagInuts);
                }
            }
            else
            {
                var taskSubscriberId = int.Parse(lblActivitySubscriberId.Text);
                var company = new Helpers.Companies().GetCompany(companyId, taskSubscriberId);
                if (company != null)
                {
                    // company
                    var companyName = company.CompanyName + (company.City != null ? " - " + company.City : "");
                    var companyTagInuts = new List<AutoComplete> { new AutoComplete { id = company.CompanyId, name = companyName } };
                    txtTagCompanyId.Text = JsonConvert.SerializeObject(companyTagInuts);
                }
            }
        }


        private void LoadContact(int contactId, int subscriberId)
        {
            var contact = new Helpers.Contacts().GetContact(contactId, subscriberId);
            if (contact != null)
            {
                // contact
                var contactTagInuts = new List<AutoComplete> { new AutoComplete { id = contact.Contact.ContactId, name = contact.Contact.ContactName } };
                txtTagContactId.Text = JsonConvert.SerializeObject(contactTagInuts);
                LoadCompany(contact.Contact.CompanyId, 0, subscriberId);
            }
        }
    }

}
