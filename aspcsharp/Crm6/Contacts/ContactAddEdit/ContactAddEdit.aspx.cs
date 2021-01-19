using Helpers;
using System;
using System.Web.UI.WebControls;

namespace Crm6.Contacts
{
    public partial class ContactAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();

            if (!Page.IsPostBack)
            {
                var contactSubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["subscriberid"] != null && Utils.IsNumeric(Request.QueryString["subscriberid"])
                    && int.Parse(Request.QueryString["subscriberid"]) > 0)
                {
                    contactSubscriberId = int.Parse(Request.QueryString["subscriberid"]);
                }
                lblContactSubscriberId.Text = contactSubscriberId.ToString();
                lblBreadcrumbHeader.Text = (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)?"Edit Contact":"Add Contact";

                // load drop downs
                LoadCountries();
                LoadContactTypes();
                LoadUsers();
                LoadBirthdayMonths();
                               
                lblGuid.Text = Guid.NewGuid().ToString();
                if (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)
                {
                    lblContactId.Text = Request.QueryString["contactId"];
                    LoadContact();
                }

                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    // retrieve company and set company json
                    lblCompanyId.Text = Request.QueryString["companyId"];
                    SetCompanyItem();
                }

                // check if quick add
                if (Request.QueryString["quickAdd"] != null && int.Parse(Request.QueryString["quickAdd"]) > 0)
                {
                    lblIsQuickAdd.Text = "1";
                }
            }
        }

        private void SetCompanyItem()
        {
            var subscriberId = int.Parse(lblContactSubscriberId.Text);
            var companyId = int.Parse(lblCompanyId.Text);
            var companyName = new Helpers.Companies().GetCompanyNameFromId(companyId, subscriberId);
            ddlCompany.Items.Add(new ListItem(companyName, companyId.ToString(), true));
            ddlCompany.Enabled = false;
        }

        private void LoadCountries()
        {
            var countries = new Helpers.DropdownHelper().GetCountries();
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }

            // load user country
            var currentUser = LoginUser.GetLoggedInUser();
            if (!string.IsNullOrEmpty(currentUser.User.CountryName))
            {
                ddlCountry.SelectedValue = currentUser.User.CountryName;
            }
        }
        
        private void LoadContactTypes()
        {
            var subscriberId = int.Parse(lblContactSubscriberId.Text);
            var contactTypes = new Helpers.DropdownHelper().GetContactTypes(subscriberId);

            foreach (var contactType in contactTypes)
            {
                ddlContactType.Items.Add(new ListItem(contactType.SelectText, contactType.SelectValue.ToString()));
            }
        }

        private void LoadBirthdayMonths() {
            ddlBirthdayMonth.Items.Add(new ListItem("",""));
            for (var i=1;i<=12;i++) {
                var monthItem = new ListItem(Utils.GetDateMonthNameFromNum(i), (i).ToString());
                ddlBirthdayMonth.Items.Add(monthItem);
            }
        }

        private void LoadContact()
        {
            var contactSubscriberId = int.Parse(lblContactSubscriberId.Text);
            int contactId = int.Parse(lblContactId.Text);
            var contactModel = new Helpers.Contacts().GetContact(contactId, contactSubscriberId);
            if (contactModel != null)
            {
                var contact = contactModel.Contact;
                txtFirstName.Text = contact.FirstName;
                txtLastName.Text = contact.LastName;
                txtEmail.Text = contact.Email;
                txtJobTitle.Text = contact.Title;
                
                Notes.Text = contact.Comments;
                ddlCompany.Items.Add(new ListItem(contact.CompanyName, contact.CompanyId.ToString(), true));
                lblCompanyId.Text = contact.CompanyId.ToString();
                txtMobile.Text = contact.MobilePhone;
                txtBusinessPhone.Text = contact.BusinessPhone;
                if (!string.IsNullOrEmpty(contact.ContactType) && ddlContactType.Items.FindByText(contact.ContactType) != null)
                    ddlContactType.Items.FindByText(contact.ContactType).Selected = true;
                txtCity.Text = contact.BusinessCity;
                txtAddress.Text = contact.BusinessAddress;
                txtPostalCode.Text = contact.BusinessPostalCode;
                ddlCountry.SelectedValue = contact.BusinessCountry;
                txtHobbies.Text = contact.Hobbies;
                OktoEmail.Checked = contact.ReceiveEmail;
                Married.Checked = contact.Married;
                HasChildren.Checked = contact.HasChildern;
                OktoCall.Checked = contact.OkToCall;
                HolidayCard.Checked = contact.HolidayCards;
                FormerEmployee.Checked = contact.FormerEmployee;
                txtStateProvince.Text = contact.BusinessStateProvince;
                SetPreviousEmployers(contact);
                ddlBirthdayMonth.SelectedValue = contact.BirthdayMonth.ToString();
                var dayOrdinal = contact.BirthdayDay>0?Utils.getDateOrdinalDay(contact.BirthdayDay):contact.BirthdayDay.ToString();
                txtBirthdayDay.Text = dayOrdinal;

                //contact profile pic / svg initials
                var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(contactId, contactSubscriberId, "contact");
                img_uploaded_profile_image.Attributes["src"] = profilePicUrl;
            }
        }
        
        private void SetPreviousEmployers(App_Code.Contact contact)
        {
            if (!string.IsNullOrWhiteSpace(contact.PreviousEmployees))
            {
                var preEmployees = contact.PreviousEmployees.Split(',');

                foreach (var preEmployee in preEmployees)
                {
                    if (ddlPreviousEmployers.Items.FindByValue(preEmployee) == null)
                    {
                        ddlPreviousEmployers.Items.Add(new ListItem(preEmployee, preEmployee));
                    }
                }
            }
            hdnPreviousEmployers.Value = contact.PreviousEmployees;
        }

        private void LoadUsers()
        {
            var users = new DropdownHelper().GetUsers(int.Parse(lblContactSubscriberId.Text));
            ddlOwner.Items.Add(new ListItem("", "0"));
            foreach (var user in users)
            {
                ddlOwner.Items.Add(new ListItem(user.SelectText, user.SelectValue.ToString()));
            }
            // select the logged in user as the sales rep
            var loggedinUserId = lblUserId.Text;
            if (ddlOwner.Items.FindByValue(loggedinUserId) != null)
            {
                ddlOwner.SelectedValue = loggedinUserId;
            }
        }
    }
}
