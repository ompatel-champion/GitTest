using Crm6.App_Code;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Contacts.ContactDetail
{
    public partial class ContactDetail : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();
            if (!Page.IsPostBack)
            {
                var contactSubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["subscriberid"] != null && Utils.IsNumeric(Request.QueryString["subscriberid"])
                    && int.Parse(Request.QueryString["subscriberid"]) > 0)
                {
                    contactSubscriberId = int.Parse(Request.QueryString["subscriberid"]);
                }
                lblContactSubscriberId.Text = contactSubscriberId.ToString();
                if (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)
                {
                    lblContactId.Text = Request.QueryString["contactId"];
                    LoadContact();
                    LoadUserActivity(contactSubscriberId, int.Parse(lblContactId.Text));
                }
                LoadSalesStages();
            }
        }
        
        private void LoadSalesStages() {
            var salesStages = new SalesStages().GetSalesStages(int.Parse(lblSubscriberId.Text));
            var val = "";
            var sep = "";
            foreach (var stage in salesStages) {
                val += sep+stage.SalesStageName;
                sep = ",";
            }
            lblSalesStages.Text = val;
        }

        private void LoadContact()
        {
            var contactId = int.Parse(lblContactId.Text);
            var objContact = new Helpers.Contacts().GetContact(contactId, int.Parse(lblContactSubscriberId.Text));
            if (objContact != null)
            {
                var contact = objContact.Contact;
                LoadCompany(contact.CompanyId, contact.SubscriberId);
                LoadContactOwner(contact.ContactOwnerUserId, contact.SubscriberId);

                //title
                lblContactName.Text = contact.ContactName;
                                             
                //details card
                if (!string.IsNullOrEmpty(contact.ContactType)) lblContactType.Text = contact.ContactType;
                else wrpContactType.Visible = false;
                if (!string.IsNullOrEmpty(contact.Title)) lblContactJobTitle.Text = contact.Title;
                else wrpContactJobTitle.Visible = false;
                if (!string.IsNullOrEmpty(contact.Hobbies)) lblInterests.Text = contact.Hobbies;
                else wrpInterests.Visible = false;
                if (contact.BirthdayMonth>0 && contact.BirthdayDay>0) lblContactBirthday.Text = Utils.GetDateOrdinalMonthDay(contact.BirthdayMonth, contact.BirthdayDay);
                else wrpContactBirthday.Visible = false;
                if (!string.IsNullOrEmpty(contact.PreviousEmployees)) lblContactPrevEmployers.Text = contact.PreviousEmployees.Replace(",","</br>");
                else wrpContactPrevEmployers.Visible = false;
                chkOkToEmail.Attributes["class"] = "check check-"+(contact.ReceiveEmail ? "on":"off");
                chkMarried.Attributes["class"] = "check check-"+(contact.Married ? "on":"off");
                chkHasChildren.Attributes["class"] = "check check-"+(contact.HasChildern ? "on":"off");
                chkOkToCall.Attributes["class"] = "check check-"+(contact.OkToCall ? "on":"off");
                chkHolidayCard.Attributes["class"] = "check check-"+(contact.HolidayCards ? "on":"off");
                chkFormerEmployee.Attributes["class"] = "check check-"+(contact.FormerEmployee ? "on":"off");
                if (!string.IsNullOrEmpty(contact.Comments)) lblComments.Text = contact.Comments;
                else wrpComments.Visible = false;

                //info card
                var addresses = new List<string>();
                if (!string.IsNullOrEmpty(contact.BusinessAddress))
                    addresses.Add(contact.BusinessAddress);
                if (!string.IsNullOrEmpty(contact.BusinessCity))
                    addresses.Add(contact.BusinessCity);
                if (!string.IsNullOrEmpty(contact.BusinessStateProvince))
                    addresses.Add(contact.BusinessStateProvince);
                if (!string.IsNullOrEmpty(contact.BusinessPostalCode))
                    addresses.Add(contact.BusinessPostalCode);
                if (!string.IsNullOrEmpty(contact.BusinessCountry))
                    addresses.Add(contact.BusinessCountry);
                if (addresses.Count > 0) lblContactAddress.Text = string.Join(", ", addresses.ToArray());
                else wrpContactAddress.Visible = false;
                if (!string.IsNullOrEmpty(contact.Email)) lblContactEmail.Text = Utils.AddWordBreakOpportunities(contact.Email);
                else wrpContactEmail.Visible = false;
                if (!string.IsNullOrEmpty(contact.BusinessPhone)) lblBusinessPhone.Text = contact.BusinessPhone;
                else wrpBusinessPhone.Visible = false;
                if (!string.IsNullOrEmpty(contact.MobilePhone)) lblMobilePhone.Text = contact.MobilePhone;
                else wrpMobilePhone.Visible = false;
                //contact profile pic / svg initials
                var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(contactId, contact.SubscriberId, "contact");
                imgContact.Attributes["src"] = profilePicUrl;
                // website
                if (string.IsNullOrWhiteSpace(contact.Website))
                    aLinkWebsite.Visible = false;
                else
                    lblWebsite.Text = contact.Website;
            }
        }

        private void LoadCompany(int companyId, int subscriberId)
        {
            var company = new Helpers.Companies().GetCompany(companyId, subscriberId);
            if (company != null)
            {
                lblCompanyNameTitle.Text = company.CompanyName;
                lblGlobalCompanyId.Text = company.CompanyIdGlobal.ToString();
                lblCompanyId.Text = company.CompanyId.ToString();
                lblCompanySubscriberId.Text = company.SubscriberId.ToString();
                lblCompanyName.Text = company.CompanyName;

                //company logo / svg initials
                var logoUrl = new Helpers.Companies().GetCompanyLogoUrl(company.CompanyId, company.SubscriberId);
                imgCompany.Attributes["src"] = logoUrl;

                //address
                var addresses = new List<string>();
                if (!string.IsNullOrEmpty(company.Address))
                    addresses.Add(company.Address);
                if (!string.IsNullOrEmpty(company.City))
                    addresses.Add(company.City);
                if (!string.IsNullOrEmpty(company.StateProvince))
                    addresses.Add(company.StateProvince);
                if (!string.IsNullOrEmpty(company.PostalCode))
                    addresses.Add(company.PostalCode);
                if (!string.IsNullOrEmpty(company.CountryName))
                    addresses.Add(company.CountryName);
                if (addresses.Count > 0) lblAddress.Text = string.Join(", ", addresses.ToArray());
                else wrpAddress.Visible = false;
                
                //phone and fax
                if (!string.IsNullOrEmpty(company.Phone)) lblPhone.Text = company.Phone;
                else wrpPhone.Visible = false;
                if (!string.IsNullOrEmpty(company.Fax)) lblFax.Text = company.Fax;
                else wrpFax.Visible = false;

                if (!string.IsNullOrEmpty(company.Website))
                {
                    lblWebsite.Text = company.Website;
                    if (!company.Website.Contains("http")) company.Website = "http://" + company.Website;
                    aLinkWebsite.Attributes["href"] = company.Website;
                    aLinkWebsite.Attributes["target"] = "_blank";
                }
                else wrpLinkWebsite.Visible = false;
            }
        }

        private void LoadContactOwner(int contactOwnerId, int subscriberId)
        {
            if (contactOwnerId > 0)
            {
                var userModel = new Helpers.Users().GetUser(contactOwnerId, subscriberId);
                var user = userModel.User;
                if (user!=null) {
                    lblContactOwnerId.Text = user.UserId.ToString();
                    lblContactOwnerName.Text = user.FullName;
                    lblContactOwnerPosition.Text = user.Title;
                    if (!string.IsNullOrEmpty(user.City) && !string.IsNullOrEmpty(user.CountryName)) lblContactOwnerLocation.Text = user.City + ", " + user.CountryName;
                    else wrpContactOwnerLocation.Visible = false;
                    if (!string.IsNullOrWhiteSpace(user.EmailAddress)) {
                        aSalesRepEmail.Attributes["href"] = "mailto:" + user.EmailAddress;
                        lblContactOwnerEmail.Text = Utils.AddWordBreakOpportunities(user.EmailAddress);
                    } else wrpContactOwnerEmail.Visible = false;
                    if (!string.IsNullOrWhiteSpace(user.Phone)) {
                        lblContactOwnerPhone.Text = user.Phone;
                        aSalesRepPhone.Attributes["href"] = "tel:" + user.Phone;
                    } else wrpContactOwnerPhone.Visible = false;
                    if (!string.IsNullOrWhiteSpace(user.MobilePhone) && user.MobilePhone!=user.Phone) {
                        lblContactOwnerMobilePhone.Text = user.MobilePhone;
                        aSalesRepMobilePhone.Attributes["href"] = "tel:" + user.MobilePhone;
                    } else wrpContactOwnerMobilePhone.Visible = false;

                    //profile pic / svg initials
                    var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(user.UserId, user.SubscriberId);
                    imgProfile.Attributes["src"] = profilePicUrl;
                }
                else wrapContactOwnerCard.Visible = false;
            }
            else wrapContactOwnerCard.Visible = false;
        }

        private void LoadUserActivity(int subscriberId, int contactId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var activity = (from t in context.UserActivities where t.ContactId == contactId && t.SubscriberId == subscriberId select t).OrderByDescending(x => x.UserActivityTimestamp);

            if (activity.Any())
            {
                rptUserActivity.DataSource = activity;
                rptUserActivity.DataBind();
            }
        }
    }
}
