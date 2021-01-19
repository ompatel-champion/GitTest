using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Models;
using Crm6.App_Code;
using System.Web.UI.WebControls;
using Crm6.App_Code.Shared;
using Crm6.SiteWide;

namespace Crm6.Companies.CompanyDetail
{

    public class RelatedCompanyModel
    {
        public string LinkCompanyToCompanyId { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Owner { get; set; }
        public string RelationType { get; set; }
    }


    public partial class CompanyDetail : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentUser = LoginUser.GetLoggedInUser();

                lblUserId.Text = currentUser.User.UserId.ToString();
                lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
                lblUsername.Text = currentUser.User.FullName;
                lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();

                var companySubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["subscriberid"] != null &&
                    Utils.IsNumeric(Request.QueryString["subscriberid"]) &&
                    int.Parse(Request.QueryString["subscriberid"]) > 0)
                {
                    companySubscriberId = int.Parse(Request.QueryString["subscriberid"]);
                }
                lblCompanySubscriberId.Text = companySubscriberId.ToString();
                //   LoadSalesStages();

                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    lblCompanyId.Text = Request.QueryString["companyId"];

                    var connection = LoginUser.GetConnection();
                    var context = new DbFirstFreightDataContext(connection);

                    var primaryContactId = (from t in context.Companies where t.CompanyId == int.Parse(Request.QueryString["companyId"]) select t.PrimaryContactId).FirstOrDefault();
                    var companyOwnerUserId = (from t in context.Companies where t.CompanyId == int.Parse(Request.QueryString["companyId"]) select t.CompanyOwnerUserId).FirstOrDefault();

                    if (primaryContactId != null && primaryContactId > 0)
                    {
                        var primaryContact = (from t in context.Contacts where t.ContactId == primaryContactId select t).FirstOrDefault();

                        if (primaryContact != null)
                        {
                            lblPrimaryContactName.Text = primaryContact.ContactName;
                            lblPrimaryContactCountry.Text = primaryContact.BusinessCity + ", " + primaryContact.BusinessCountry;
                            lblPrimaryContactPosition.Text = primaryContact.Title;
                            lblPrimaryContactPhone.Text = primaryContact.BusinessPhone;
                            if (!string.IsNullOrWhiteSpace(primaryContact.BusinessPhone))
                            {
                                lblPrimaryContactPhone.Text = primaryContact.BusinessPhone;
                                aPrimaryContactPhone.Attributes["href"] = "tel:" + primaryContact.BusinessPhone;
                            }
                            lblPrimaryContactEmail.Text = Utils.AddWordBreakOpportunities(primaryContact.Email);
                            aPrimaryContactEmail.Attributes["href"] = "tel:" + primaryContact.Email;

                            // profile pic / svg initials
                            var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(primaryContact.ContactId, primaryContact.SubscriberId, "contact");
                            imgPrimaryContact.Attributes["src"] = profilePicUrl;
                        }
                        else
                        {
                            // hide primary contact container
                            divPrimaryContactContainer.Attributes["class"] += " hide";
                        }
                    }
                    else
                    {
                        // hide primary contact container
                        divPrimaryContactContainer.Attributes["class"] += " hide";
                    }

                    if (companyOwnerUserId != null && companyOwnerUserId > 0)
                    {
                        user = (from t in context.Users where t.UserId == companyOwnerUserId select t).FirstOrDefault();
                        if (user != null)
                        {
                            lblSalesOwnerId.Text = companyOwnerUserId.ToString();
                            lblSalesRepName.Text = user.FullName;
                            lblSalesRepPosition.Text = user.Title;
                            if (!string.IsNullOrWhiteSpace(user.City) && !string.IsNullOrWhiteSpace(user.CountryName)) lblSalesRepLocation.Text = user.City + ", " + user.CountryName;
                            else wrpSalesRepLocation.Visible = false;
                            if (!string.IsNullOrWhiteSpace(user.EmailAddress)) {
                                lblSalesRepEmail.Text = Utils.AddWordBreakOpportunities(user.EmailAddress);
                                aSalesRepEmail.Attributes["href"] = "mailto:" + user.EmailAddress;
                            } else wrpSalesRepEmail.Visible = false;
                            if (!string.IsNullOrWhiteSpace(user.Phone)) {
                                lblSalesRepPhone.Text = user.Phone;
                                aSalesRepPhone.Attributes["href"] = "tel:" + user.Phone;
                            } else wrpSalesRepPhone.Visible = false;
                            if (!string.IsNullOrWhiteSpace(user.MobilePhone) && (user.Phone!=user.MobilePhone)) {
                                lblSalesRepMobilePhone.Text = user.MobilePhone;
                                aSalesRepMobilePhone.Attributes["href"] = "tel:" + user.MobilePhone;
                            } else wrpSalesRepMobilePhone.Visible = false;
                            
                            // profile pic / svg initials
                            var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(user.UserId, user.SubscriberId);
                            imgProfile.Attributes["src"] = profilePicUrl;

                            companyInfo.DataBind();
                        }
                        else divSalesOwnerContainer.Visible = false;
                    }
                    else divSalesOwnerContainer.Visible = false;

                    LoadQuotes(int.Parse(Request.QueryString["companyId"]), currentUser.Subscriber.SubscriberId);
                    LoadUserActivity(currentUser.Subscriber.SubscriberId, int.Parse(Request.QueryString["companyId"]));
                    LoadRelatedCompanies(currentUser.Subscriber.SubscriberId, int.Parse(Request.QueryString["companyId"]));
                    LoadCompany();
                    PopulateGlobalCompaniesSection(int.Parse(Request.QueryString["subscriberid"]));
                }
            }
        }

        protected User user;

        protected string Test = "blah";

        protected bool IsVisible(string myString)
        {
            return string.IsNullOrEmpty(myString);
        }

        private void LoadRelatedCompanies(int subscriberId, int companyId, string keyword = "")
        {
            // the following code has to be moved to JQUERY - Bad pravtise to retrieve global companies (over 10,000) record once.

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            tblRelatedCompanies.Visible = false;
            pnlNoRelatedCompanies.Visible = true;
            var relatedCompanies = (from t in context.LinkCompanyToCompanies where (t.CompanyId == companyId || t.LinkedCompanyId == companyId) && t.Deleted == false && t.SubscriberId == subscriberId select t);

            if (relatedCompanies.Any())
            {
                List<RelatedCompanyModel> result = new List<RelatedCompanyModel>();

                foreach (var relation in relatedCompanies)
                {
                    var relatedCompanyDetails = (from t in context.Companies where t.CompanyId == relation.LinkedCompanyId && t.SubscriberId == subscriberId select t).FirstOrDefault();

                    if (relatedCompanyDetails != null)
                    {
                        var ownerDetails = (from t in context.Users where t.UserId == relatedCompanyDetails.CompanyOwnerUserId && t.SubscriberId == subscriberId select t).FirstOrDefault();

                        var final = new RelatedCompanyModel
                        {
                            CompanyName = relatedCompanyDetails.CompanyName,
                            City = relatedCompanyDetails.City,
                            Country = relatedCompanyDetails.CountryName,
                            Owner = ownerDetails?.FullName,
                            RelationType = relation.LinkType,
                            LinkCompanyToCompanyId = relation.LinkCompanyToCompanyId.ToString()
                        };

                        result.Add(final);
                    }
                }

                if (string.IsNullOrWhiteSpace(keyword) == false)
                {
                    result = result.Where(x => (x.City != null && x.City.ToLower().Contains(keyword.ToLower())) ||
                                               (x.CompanyName != null && x.CompanyName.ToLower().Contains(keyword.ToLower())) ||
                                               (x.Country != null && x.Country.ToLower().Contains(keyword.ToLower())) ||
                                               (x.Owner != null && x.Owner.ToLower().Contains(keyword.ToLower())) ||
                                               (x.RelationType != null && x.RelationType.ToLower().Contains(keyword.ToLower())))?.OrderBy(x => x.CompanyName).ToList();
                }

                if (result != null && result.Count > 0)
                {
                    tblRelatedCompanies.Visible = true;
                    pnlNoRelatedCompanies.Visible = false;
                    rptRelatedCompanies.DataSource = result;
                    rptRelatedCompanies.DataBind();
                }
                else
                {
                    tblRelatedCompanies.Visible = false;
                }

            }
        }


        private void LoadUserActivity(int subscriberId, int companyId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter); 
            var context = new DbFirstFreightDataContext(connection);

            var activity = (from t in context.UserActivities where t.CompanyId == companyId && t.SubscriberId == subscriberId select t).OrderByDescending(x=>x.UserActivityTimestamp);

            if (activity.Any())
            {
                rptUserActivity.DataSource = activity;
                rptUserActivity.DataBind();
            }
        }

        private void PopulateGlobalCompaniesSection(int subscriberId)
        {
            //var model = new Helpers.Companies().GetGlobalCompanies(new CompanyFilters() { SubscriberId = subscriberId });

            //if (model != null)
            //{
            //    model.Companies.Insert(0, new GlobalCompany { CompanyName = "", CompanyId = 0 });

            //    ddlGlobalCompanies.DataSource = model.Companies;
            //    ddlGlobalCompanies.DataTextField = "CompanyName";
            //    ddlGlobalCompanies.DataValueField = "CompanyId";
            //    ddlGlobalCompanies.DataBind();
            //}

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var companyLinkTypes = (from t in context.CompanyLinkTypes where t.SubscriberId == subscriberId select t);

            if (companyLinkTypes != null)
            {
                ddlLinkType.Items.Add(new ListItem("", "0"));
                ddlLinkType.DataSource = companyLinkTypes;
                ddlLinkType.DataTextField = "CompanyLinkTypeName";
                ddlLinkType.DataValueField = "CompanyLinkTypeId";
                ddlLinkType.DataBind();

                ddlChangeRelationType.Items.Add(new ListItem("", "0"));
                ddlChangeRelationType.DataSource = companyLinkTypes;
                ddlChangeRelationType.DataTextField = "CompanyLinkTypeName";
                ddlChangeRelationType.DataValueField = "CompanyLinkTypeId";
                ddlChangeRelationType.DataBind();
            }
        }
        
        private void LoadQuotes(int companyId, int subscriberId)
        {
            if (companyId > 0)
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);

                var quotes = (from t in context.Quotes where t.CompanyId == companyId && t.SubscriberId == subscriberId select t);

                if (quotes.Any())
                {
                    //    rptQuotes.DataSource = quotes;
                    //    rptQuotes.DataBind();
                }
            }
        }

        private void LoadCompany()
        {
            var companyId = int.Parse(lblCompanyId.Text);
            var subscriberId = int.Parse(lblCompanySubscriberId.Text);
            var company = new Helpers.Companies().GetCompany(companyId, subscriberId);
            if (company != null)
            {
                lblCompanyName.Text = company.CompanyName;
                lblCompanyOwnerId.Text = company.CompanyOwnerUserId.ToString();
                lblCompanySubscriberId.Text = company.SubscriberId.ToString();
                lblCompanyType.Text = StringHelpers.FormatSource(company.CompanyTypes);
                lblDivision.Text = StringHelpers.FormatDivision(company.UpdateUserName);
                lblGlobalCompanyId.Text = company.CompanyIdGlobal.ToString();
                lblIndustry.Text = string.IsNullOrWhiteSpace(company.Industry) ? "" : company.Industry;
                lblLeadSource.Text = StringHelpers.FormatSource(company.UpdateUserName);

                if (company.LastUpdate != DateTime.MinValue) lblLastUpdatedDate.Text = company.LastUpdate.ToString(Settings.DateFormat);
                lblLastUpdatedBy.Text = StringHelpers.FormatUsername(company.UpdateUserName);
                if (company.CreatedDate != DateTime.MinValue) lblCreatedDate.Text = company.CreatedDate.ToString(Settings.DateFormat);
                lblCreatedBy.Text = StringHelpers.FormatUsername(company.CreatedUserName);
                                
                if (!string.IsNullOrEmpty(company.Comments)) lblComments.Text = company.Comments;
                else wrpComments.Visible = false;

                if (!string.IsNullOrEmpty(company.Website))
                {
                    lblWebsite.Text = company.Website;
                    if (!company.Website.Contains("http"))
                    {
                        company.Website = "http://" + company.Website;
                    }
                    aLinkWebsite.Attributes["href"] = company.Website;
                    aLinkWebsite.Attributes["target"] = "_blank";
                }
                else
                {
                    lblWebsite.Text = "-";
                    aLinkWebsite.Attributes["href"] = "javascript:void(0)";
                }

                // address
                var addresses = new List<string>();
                if (!string.IsNullOrEmpty(company.Address))
                {
                    addresses.Add(company.Address);
                }

                string stateAndPostcode;

                if (!string.IsNullOrEmpty(company.StateProvince) && !string.IsNullOrEmpty(company.PostalCode))
                {
                    stateAndPostcode = $"{ company.StateProvince} { company.PostalCode}";
                }
                else if (!string.IsNullOrEmpty(company.StateProvince))
                {
                    stateAndPostcode = company.StateProvince;
                }
                else
                {
                    stateAndPostcode = company.PostalCode;
                }

                if (!string.IsNullOrEmpty(company.City))
                {
                    if (!string.IsNullOrEmpty(stateAndPostcode))
                        addresses.Add($"{company.City}, {stateAndPostcode}");
                    else
                        addresses.Add(company.City);
                }
                else
                {
                    addresses.Add(stateAndPostcode);
                }

                if (!string.IsNullOrEmpty(company.CountryName))
                    addresses.Add(company.CountryName);

                if (addresses.Count > 0)
                {
                    var address = string.Join("<br>", addresses.ToArray());
                    lblAddress.Text = string.IsNullOrEmpty(address) ? "-" : address;
                }

                //company logo / svg initials
                var logoUrl = new Helpers.Companies().GetCompanyLogoUrl(company.CompanyId, company.SubscriberId);
                imgCompany.Attributes["src"] = logoUrl;
            }
        }


        protected void btnReassignCompany_Click(object sender, EventArgs e)
        {
            Response.Redirect($"../Reassign/ReassignCompany.aspx?companyId={lblCompanyId.Text}");
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            LoadRelatedCompanies(currentUser.Subscriber.SubscriberId, int.Parse(Request.QueryString["companyId"]), txtKeyword.Text);
        }


        protected void btnAddRelatedCompany_Click(object sender, EventArgs e)
        {
            if (ddlLinkType.SelectedIndex > -1 && !string.IsNullOrWhiteSpace(ddlRelatedCompany.SelectedValue))
            {

                int linkedCompanyId = 0;
                int companyId = int.Parse(Request.QueryString["companyId"]);
                var currentUser = LoginUser.GetLoggedInUser();

                if (int.TryParse(ddlRelatedCompany.SelectedValue, out linkedCompanyId))
                {
                    var linkedCompanies = new Helpers.Companies().GetLinkedCompanies(companyId, currentUser.Subscriber.SubscriberId);

                    if (linkedCompanies.FirstOrDefault(x => x.LinkedCompanyId == linkedCompanyId) == null)
                    {
                        if (new Helpers.Companies().LinkCompany(new LinkCompanyRequest()
                        {
                            CompanyId = companyId,
                            LinkType = ddlLinkType.SelectedItem.Text,
                            LinkedCompanyId = linkedCompanyId,
                            SubscriberId = currentUser.Subscriber.SubscriberId,
                            UpdateUserId = currentUser.User.UserId
                        }))
                        {
                            Page.Response.Redirect(Page.Request.Url.ToString(), true);
                        }
                    }
                }
            }
        }


        protected void btnRemoveRelatedCompanies_Click(object sender, EventArgs e)
        {
            if (rptRelatedCompanies.Items.Count > 0)
            {

                List<int> listIdsToRemove = new List<int>();

                foreach (RepeaterItem item in rptRelatedCompanies.Items)
                {
                    CheckBox chk = item.Controls[1] as CheckBox;

                    if (chk.Checked)
                    {
                        Label lbl = item.Controls[3] as Label;

                        if (!string.IsNullOrWhiteSpace(lbl.Text))
                        {
                            int idToRemove;

                            if (int.TryParse(lbl.Text, out idToRemove) && idToRemove > 0)
                            {
                                listIdsToRemove.Add(idToRemove);
                            }
                        }
                    }
                }

                if (listIdsToRemove.Count > 0)
                {
                    var currentUser = LoginUser.GetLoggedInUser();

                    foreach (var id in listIdsToRemove)
                    {
                        new Helpers.Companies().DeleteLinkedCompany(id, currentUser.User.UserId, currentUser.Subscriber.SubscriberId);
                    }

                    Page.Response.Redirect(Page.Request.Url.ToString() + "#relatedcompanies", true);
                }
            }
        }


        protected void linkRequestAccess_Click(object sender, EventArgs e)
        {
            var linkIdLabelText = (((Control)sender).Parent.Controls[3] as Label)?.Text;

            if (!string.IsNullOrWhiteSpace(linkIdLabelText))
            {
                int linkId = 0;

                if (int.TryParse(linkIdLabelText, out linkId))
                {
                    RequestAccess(linkId);
                }
            }
        }


        private void RequestAccess(int linkId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var relatedCompany = (from t in context.LinkCompanyToCompanies where t.LinkCompanyToCompanyId == linkId select t).FirstOrDefault();
            new Helpers.Companies().RequestAccess(relatedCompany.LinkedCompanyId, currentUser.User.UserId, currentUser.Subscriber.SubscriberId, currentUser.Subscriber.SubscriberId);
        }


        protected void btnRequestAccess_Click(object sender, EventArgs e)
        {
            if (rptRelatedCompanies.Items.Count > 0)
            {
                List<int> listIdsToRequestAccessFor = new List<int>();

                foreach (RepeaterItem item in rptRelatedCompanies.Items)
                {
                    CheckBox chk = item.Controls[1] as CheckBox;

                    if (chk.Checked)
                    {
                        Label lbl = item.Controls[3] as Label;

                        if (!string.IsNullOrWhiteSpace(lbl.Text))
                        {
                            int idToRemove;

                            if (int.TryParse(lbl.Text, out idToRemove) && idToRemove > 0)
                            {
                                listIdsToRequestAccessFor.Add(idToRemove);
                            }
                        }
                    }
                }

                foreach (var current in listIdsToRequestAccessFor)
                {
                    RequestAccess(current);
                }

                Page.Response.Redirect(Page.Request.Url.ToString() + "#relatedcompanies", true);
            }
        }


        protected void btnChangeRelationType_Click(object sender, EventArgs e)
        {
            if (rptRelatedCompanies.Items.Count > 0)
            {
                List<int> listIdsToChangeType = new List<int>();

                foreach (RepeaterItem item in rptRelatedCompanies.Items)
                {
                    CheckBox chk = item.Controls[1] as CheckBox;

                    if (chk.Checked)
                    {
                        Label lbl = item.Controls[3] as Label;

                        if (!string.IsNullOrWhiteSpace(lbl.Text))
                        {
                            int id;

                            if (int.TryParse(lbl.Text, out id) && id > 0)
                            {
                                listIdsToChangeType.Add(id);
                            }
                        }
                    }
                }

                foreach (var current in listIdsToChangeType)
                {
                    ChangeRelationType(current);
                }

                Page.Response.Redirect(Page.Request.Url.ToString() + "#relatedcompanies", true);
            }
        }


        private void ChangeRelationType(int linkId)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var relatedCompany = (from t in context.LinkCompanyToCompanies where t.LinkCompanyToCompanyId == linkId select t).FirstOrDefault();

            if (relatedCompany != null)
            {
                relatedCompany.LinkType = ddlChangeRelationType.SelectedItem.Text;
                context.SubmitChanges();
            }
        }


        protected void linkView_Click(object sender, EventArgs e)
        {
            var linkIdLabelText = (((Control)sender).Parent.Controls[3] as Label)?.Text;

            if (!string.IsNullOrWhiteSpace(linkIdLabelText))
            {
                int linkId = 0;

                if (int.TryParse(linkIdLabelText, out linkId))
                {
                    var currentUser = LoginUser.GetLoggedInUser();

                    var connection = LoginUser.GetConnection();
                    var context = new DbFirstFreightDataContext(connection);

                    var sharedConnection = LoginUser.GetSharedConnection();
                    var sharedContext = new DbSharedDataContext(sharedConnection) { CommandTimeout = 0 };

                    var relatedCompany = (from t in context.LinkCompanyToCompanies where t.LinkCompanyToCompanyId == linkId select t).FirstOrDefault();

                    if (relatedCompany != null)
                    {
                        var globalCompany = (from t in sharedContext.GlobalCompanies where t.CompanyId == relatedCompany.LinkedCompanyId select t).FirstOrDefault();

                        if (HasPermissionToClickThrough(globalCompany.GlobalCompanyId, currentUser.User.UserId, currentUser.Subscriber.SubscriberId))
                        {
                            Response.Redirect($"../CompanyDetail/CompanyDetail.aspx?companyId={relatedCompany.LinkedCompanyId}&subscriberId={currentUser.Subscriber.SubscriberId}");
                        }
                    }
                }
            }
        }


        private bool HasPermissionToClickThrough(int globalCompanyId, int userId, int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            if (!string.IsNullOrEmpty(currentUser.User.UserRoles) &&
                                currentUser.User.UserRoles.Contains("CRM Admin"))
            {

                var companyIds = new Helpers.Companies().GetAccessibleCompanyIds(userId, subscriberId);

                if (companyIds != null && companyIds.Count > 0)
                {
                    if (companyIds.Contains(globalCompanyId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        protected void rptRelatedCompanies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var linkIdLabelText = (e.Item.Controls[3] as Label)?.Text;

            if (!string.IsNullOrWhiteSpace(linkIdLabelText))
            {
                int linkId = 0;

                if (int.TryParse(linkIdLabelText, out linkId))
                {
                    var currentUser = LoginUser.GetLoggedInUser();

                    var connection = LoginUser.GetConnection();
                    var context = new DbFirstFreightDataContext(connection);

                    var sharedConnection = LoginUser.GetSharedConnection();
                    var sharedContext = new DbSharedDataContext(sharedConnection) { CommandTimeout = 0 };

                    var relatedCompany = (from t in context.LinkCompanyToCompanies where t.LinkCompanyToCompanyId == linkId select t).FirstOrDefault();

                    if (relatedCompany != null)
                    {
                        var globalCompany = (from t in sharedContext.GlobalCompanies where t.CompanyId == relatedCompany.LinkedCompanyId select t).FirstOrDefault();

                        if (globalCompany != null && HasPermissionToClickThrough(globalCompany.GlobalCompanyId, currentUser.User.UserId, currentUser.Subscriber.SubscriberId))
                        {
                            e.Item.Controls[17].Visible = true;
                        }
                        else
                        {
                            e.Item.Controls[17].Visible = false;
                        }
                    }
                }
            }
        }

    }
}