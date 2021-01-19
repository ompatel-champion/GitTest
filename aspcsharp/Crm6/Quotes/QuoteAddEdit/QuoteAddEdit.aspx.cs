using Helpers;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Crm6.App_Code.Helpers;
using Models;
using System.Linq;
using Crm6.App_Code;

namespace Crm6.Admin.Quotes
{
    public partial class QuoteAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            if (!Page.IsPostBack)
            {
                var subscriberId = currentUser.Subscriber.SubscriberId;

                lblDealSubscriberId.Text = subscriberId.ToString();

                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    //  retrieve company and set company json
                    var companyId = int.Parse(Request.QueryString["companyId"]);
                    lblCompanyId.Text = companyId.ToString();
                    SetCompanyItem(companyId, subscriberId);
                    PopulateDealsCombo(subscriberId, companyId);
                    PopulateSalesRepCombo(subscriberId, companyId);
                }
                else
                {
                    PopulateCompaniesCombo(subscriberId);
                }

                if (Request.QueryString["quoteId"] != null && int.Parse(Request.QueryString["quoteId"]) > 0)
                {
                    var quoteId = int.Parse(Request.QueryString["quoteId"]);

                    if (quoteId > 0)
                        LoadQuote(quoteId, subscriberId);

                }
            }
        }

        private void LoadQuote(int quoteId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var quote = context.Quotes.FirstOrDefault(t => t.QuoteId == quoteId);

            if (quote != null)
            {
                PopulateDealsCombo(subscriberId, quote.CompanyId.Value);
                PopulateSalesRepCombo(subscriberId, quote.CompanyId.Value);

                lblQuoteId.Text = quote.QuoteId.ToString();

                ddlCompany.SelectedValue = quote.CompanyId?.ToString();
                ddlDeal.SelectedValue = quote.DealId?.ToString();
                txtBranch.Text = quote.BranchCode;
                txtTerms.Text = quote.IncotermText;
                txtCode.Text = quote.CompanyCode;
                txtPieces.Text = quote.TotalPackages.ToString();
            }
        }


        private void SetCompanyItem(int companyId, int subscriberId)
        {
            var dealSubscriberId = int.Parse(lblDealSubscriberId.Text);
            var company = new Helpers.Companies().GetCompany(companyId, dealSubscriberId);
            if (company != null)
            {
                var companyName = company.CompanyName + (company.City != null ? " - " + company.City : "");
                ddlCompany.Items.Clear();
                ddlCompany.Items.Add(new ListItem(companyName, companyId.ToString(), true));
                ddlCompany.Enabled = false;
            }
        }

        #region DropDowns

        private void PopulateDealsCombo(int subscriberId, int companyId)
        {
            var deals = new Helpers.Deals().GetCompanyDeals(companyId, subscriberId);

            ddlDeal.DataSource = deals;
            ddlDeal.DataTextField = "DealName";
            ddlDeal.DataValueField = "DealId";
            ddlDeal.DataBind();
        }

        private void PopulateCompaniesCombo(int subscriberId)
        {
            var companies = new DropdownHelper().GetCompanies(subscriberId, "");

            ddlCompany.DataSource = companies;
            ddlCompany.DataTextField = "SelectText";
            ddlCompany.DataValueField = "SelectValue";
            ddlCompany.DataBind();
        }

        private void PopulateSalesRepCombo(int subscriberId, int companyId)
        {
            var userModel = new Helpers.Companies().GetCompanyUsers(companyId, subscriberId);

            var users = userModel.Select(x => x.User).ToList();

            ddlSalesOwner.DataSource = users;
            ddlSalesOwner.DataTextField = "FullName";
            ddlSalesOwner.DataValueField = "UserId";
            ddlSalesOwner.DataBind();
        }



        #endregion

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            int companyId = 0;

            int.TryParse(ddlCompany.SelectedItem.Value, out companyId);
            var subscriberId = LoginUser.GetLoggedInUser().Subscriber.SubscriberId;

            PopulateDealsCombo(subscriberId, companyId);
            PopulateSalesRepCombo(subscriberId, companyId);

            lblCompanyId.Text = companyId.ToString();
        }
    }
}
