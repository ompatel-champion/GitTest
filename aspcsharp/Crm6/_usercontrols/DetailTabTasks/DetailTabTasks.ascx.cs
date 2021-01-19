using System;
using System.Linq;
using System.Web.UI.WebControls;
using Helpers;

namespace Crm6._usercontrols.DetailTabTasks
{
    public partial class DetailTabTasks : System.Web.UI.UserControl
    {
        public string DetailType {get; set;}

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var subscriberId = currentUser.User.SubscriberId;
            
            switch (DetailType) {
                case "deal":
                    var dealId = int.Parse(Request.QueryString["dealId"]);
                    if (dealId>0) {
                        var deal = new Helpers.Deals().GetDeal(dealId, subscriberId);
                        if (deal != null) PopulateContactsCombo(subscriberId, deal.CompanyId);
                        PopulateSalesRepCombo(subscriberId);
                    }
                    typeFieldsGroupContact.Visible = false;
                    typeFieldsGroupCompany.Visible = false;
                    break;
                case "company":
                    var companyId = int.Parse(Request.QueryString["companyId"]);
                    if (companyId>0) {
                        PopulateContactsCombo(subscriberId, companyId);
                        PopulateDealsCombo(subscriberId, companyId);
                    }
                    PopulateSalesRepCombo(subscriberId);
                    typeFieldsGroupContact.Visible = false;
                    typeFieldsGroupDeal.Visible = false;
                    break;
                case "contact":
                    var contactId = int.Parse(Request.QueryString["contactId"]);
                    if (contactId>0) {
                        var objContact = new Helpers.Contacts().GetContact(contactId, subscriberId);
                        if (objContact != null)
                        {
                            var contact = objContact.Contact;
                            PopulateDealsCombo(contact.SubscriberId, contact.ContactId);
                        }
                        PopulateSalesRepCombo(subscriberId);
                    }
                    typeFieldsGroupCompany.Visible = false;
                    typeFieldsGroupDeal.Visible = false;
                    break;
            }
        }

        private void PopulateDealsCombo(int subscriberId, int contactOrCompanyId)
        {
            DropDownList ddl = null;
            switch (DetailType) {
                case "company":
                    ddl = ddlTaskRelatedDealForCompany;
                    break;
                case "contact":
                    ddl = ddlTaskRelatedDealForContact;
                    break;
            }

            IOrderedEnumerable<App_Code.Deal> deals = null;
            if (DetailType=="contact") deals = new Helpers.Deals().GetContactDeals(contactOrCompanyId, subscriberId).OrderBy(t => t.DealName);
            else deals = new Helpers.Deals().GetCompanyDeals(contactOrCompanyId, subscriberId).OrderBy(t=>t.DealName);
            ddl.Items.Add(new ListItem("", ""));
            foreach (var deal in deals)
            {
                ddl.Items.Add(new ListItem(deal.DealName, deal.DealId.ToString()));
            }
        }
        
        private void PopulateSalesRepCombo(int subscriberId)
        {
            var users = new DropdownHelper().GetLinkedSubsciberSalesRepGlobalUserIds(subscriberId).OrderBy(t => t.SelectText);
            ddlTaskSalesReps.Items.Add(new ListItem("", "0"));
            foreach (var u in users)
            {
                ddlTaskSalesReps.Items.Add(new ListItem(u.SelectText, u.SelectValue));
            } 
            var fullName = LoginUser.GetLoggedInUser().User?.FullName; 
            var item = ddlTaskSalesReps.Items.FindByText(fullName);
            if (item != null) item.Selected = true;
        }

        private void PopulateContactsCombo(int subscriberId, int companyId)
        {
            DropDownList ddl = null;
            switch (DetailType) {
                case "deal":
                    ddl = ddlTaskRelatedContactForDeal;
                    break;
                case "company":
                    ddl = ddlTaskRelatedContactForCompany;
                    break;
            }

            var contactModel = new Helpers.Companies().GetCompanyContacts(companyId, subscriberId);
            var contacts = contactModel.Select(x => x.Contact).OrderBy(t => t.ContactName).ToList();
            ddl.Items.Add(new ListItem("", ""));
            foreach (var contact in contacts)
            {
                ddl.Items.Add(new ListItem(contact.ContactName, contact.ContactId.ToString()));
            }
        }
    }
}