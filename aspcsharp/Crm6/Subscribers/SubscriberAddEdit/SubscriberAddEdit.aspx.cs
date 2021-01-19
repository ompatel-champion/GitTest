using System;
using System.Web.UI.WebControls;

namespace Crm6.Subscribers
{
    public partial class SubscriberAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();

            if (!Page.IsPostBack)
            {
                // load dropdowns
                LoadCountries();

                if (Request.QueryString["subscriberId"] != null && int.Parse(Request.QueryString["subscriberId"]) > 0)
                {
                    lblSubscriberId.Text = Request.QueryString["subscriberId"];
                    //LoadSubscriber();
                }
            }
        }


        private void LoadCountries()
        {
            var countries = new Helpers.DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("Select Country...", "0"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }


        private void LoadSubscriber()
        {
            int subscriberId = int.Parse(lblSubscriberId.Text);
            var subscriber = new Helpers.Subscribers().GetSubscriber(subscriberId);
            if (subscriber != null)
            {
                txtCompanyName.Text = subscriber.CompanyName;
                txtContactName.Text = subscriber.ContactName;
                txtEmail.Text = subscriber.Email;
                txtAddress.Text = subscriber.Address;
                txtCity.Text = subscriber.City;
                txtPostalCode.Text = subscriber.PostalCode;
                ddlCountry.SelectedValue = subscriber.CountryName;
                txtPhone.Text = subscriber.Phone;
                //txtComments.Text = subscriber.Comments;
            }
        }
    }

}