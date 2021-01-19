using Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Companies.LinkCompany
{
    public partial class LinkCompany : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    lblCompanyId.Text = Request.QueryString["companyId"];
                    //  retrieve company link types 
                    LoadCompanyLinkTypes();
                }
            }

        }


        private void LoadCompanyLinkTypes()
        {
            var linkTypes = new DropdownHelper().GetCompanyLinkTypes(int.Parse(lblSubscriberId.Text));
            ddlLinkTypes.Items.Add(new ListItem("Select Link Type...", ""));
            foreach (var linkType in linkTypes)
            {
                ddlLinkTypes.Items.Add(new ListItem(linkType.SelectText, linkType.SelectValue.ToString()));
            }
        }

    }
}