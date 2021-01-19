using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Admin.Quotes
{
    public partial class Quotes : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentUser = LoginUser.GetLoggedInUser();

                if (currentUser != null)
                {
                    lblUserId.Text = currentUser.User.UserId.ToString();
                    lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

                    PopulateFilterCombos(currentUser.Subscriber.SubscriberId);
                    LoadQuotes(currentUser.Subscriber.SubscriberId, "", "", "");
                }
                else
                {
                    Response.Redirect("/Login.aspx");
                }
            }
        }

        private void PopulateFilterCombos(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var quotes = (from t in context.Quotes where t.SubscriberId == subscriberId select t);

            if (quotes.Any())
            {
                ddlSalesReps.DataSource = quotes;
                ddlSalesReps.DataTextField = "CustomerName";
                ddlSalesReps.DataValueField = "CustomerName";
                ddlSalesReps.DataBind();

                ddlSalesReps.Items.Add("Sales Rep");
                ddlSalesReps.SelectedIndex = ddlSalesReps.Items.Count - 1;

                ddlBranch.DataSource = quotes;
                ddlBranch.DataTextField = "BranchName";
                ddlBranch.DataValueField = "BranchName";
                ddlBranch.DataBind();

                ddlBranch.Items.Add("Branch");
                ddlBranch.SelectedIndex = ddlBranch.Items.Count - 1;

                ddlStatus.DataSource = quotes;
                ddlStatus.DataTextField = "QuoteStatus";
                ddlStatus.DataValueField = "QuoteStatus";
                ddlStatus.DataBind();

                ddlStatus.Items.Add("Status");
                ddlStatus.SelectedIndex = ddlStatus.Items.Count - 1;
            }
        }

        private void LoadQuotes(int subscriberId, string branch, string status, string salesRep)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var quotes = (from t in context.Quotes where t.SubscriberId == subscriberId select t);

            if (string.IsNullOrWhiteSpace(branch) == false)
            {
                quotes = quotes.Where(x => x.BranchName.ToLower() == branch.ToLower());
            }

            if (string.IsNullOrWhiteSpace(status) == false)
            {
                quotes = quotes.Where(x => x.QuoteStatus.ToLower() == status.ToLower());
            }

            if (string.IsNullOrWhiteSpace(salesRep) == false)
            {
                quotes = quotes.Where(x => x.CustomerName.ToLower() == salesRep.ToLower());
            }

            if (quotes.Any())
            {
                List<QuoteExtended> listExtendedQuotes = new List<QuoteExtended>();

                foreach (Quote currentQuote in quotes.ToList())
                {
                    var deal = (from t in context.Deals where t.DealId == currentQuote.DealId select t).FirstOrDefault();

                    QuoteExtended extendedQuote = new QuoteExtended
                    {
                        CompanyName = currentQuote.CompanyName,
                        DealName = deal?.DealName,
                        CustomerName = currentQuote.CustomerName,
                        BranchName = currentQuote.BranchName,
                        QuoteCode = currentQuote.QuoteCode,
                        Destination = currentQuote.Destination,
                        CreatedDate = currentQuote.CreatedDate,
                        ValidTo = currentQuote.ValidTo,
                        TotalPackages = currentQuote.TotalPackages,
                        TotalWeight = currentQuote.TotalWeight,
                        Incoterm = currentQuote.Incoterm,
                        QuoteStatus = currentQuote.QuoteStatus
                    };

                    listExtendedQuotes.Add(extendedQuote);
                }

                rptQuotes.DataSource = listExtendedQuotes;
                rptQuotes.DataBind();
            }
        }

        private void SearchQuotes(int subscriberId, string keyword)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var quotes = (from t in context.Quotes where t.SubscriberId == subscriberId select t);

            quotes = quotes.Where(x => x.BranchName.ToLower().Contains(keyword.ToLower()) ||
            x.QuoteStatus.ToLower().Contains(keyword.ToLower()) ||
            x.CustomerName.ToLower().Contains(keyword.ToLower()) ||
            x.IncotermText.ToLower().Contains(keyword.ToLower()));

            if (quotes.Any())
            {
                List<QuoteExtended> listExtendedQuotes = new List<QuoteExtended>();

                foreach (Quote currentQuote in quotes.ToList())
                {
                    var deal = (from t in context.Deals where t.DealId == currentQuote.DealId select t).FirstOrDefault();

                    QuoteExtended extendedQuote = new QuoteExtended
                    {
                        CompanyName = currentQuote.CompanyName,
                        DealName = deal?.DealName,
                        CustomerName = currentQuote.CustomerName,
                        BranchName = currentQuote.BranchName,
                        QuoteCode = currentQuote.QuoteCode,
                        Destination = currentQuote.Destination,
                        CreatedDate = currentQuote.CreatedDate,
                        ValidTo = currentQuote.ValidTo,
                        TotalPackages = currentQuote.TotalPackages,
                        TotalWeight = currentQuote.TotalWeight,
                        Incoterm = currentQuote.Incoterm,
                        QuoteStatus = currentQuote.QuoteStatus
                    };

                    listExtendedQuotes.Add(extendedQuote);
                }

                rptQuotes.DataSource = listExtendedQuotes;
                rptQuotes.DataBind();
            }
            else
            {
                rptQuotes.DataSource = null;
                rptQuotes.DataBind();
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            var selectedBranch = ddlBranch.SelectedItem.Text;
            var selectedStatus = ddlStatus.SelectedItem.Text;
            var selectedSalesRep = ddlSalesReps.SelectedItem.Text;

            if (selectedBranch.Equals("Branch", StringComparison.InvariantCultureIgnoreCase))
            {
                selectedBranch = "";
            }

            if (selectedSalesRep.Equals("Sales Rep", StringComparison.InvariantCultureIgnoreCase))
            {
                selectedSalesRep = "";
            }

            if (selectedStatus.Equals("Status", StringComparison.InvariantCultureIgnoreCase))
            {
                selectedStatus = "";
            }

            var currentUser = LoginUser.GetLoggedInUser();

            LoadQuotes(currentUser.Subscriber.SubscriberId, selectedBranch, selectedStatus, selectedSalesRep);
        }

        protected void btnNewPortrixQuote_Click(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            var userId = currentUser.User.UserId;

            var dealId = 0; //TODO - get deal id
            var clientId = 582; //Todo - Get customer id

            Response.Redirect($"https://gpm-pls-demo.portrix-ls.de/app#quotation?tab=FILTER_RRC&client={clientId}&opportunityId={dealId}&userId={userId}");
        }

        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            SearchQuotes(currentUser.Subscriber.SubscriberId, txtSearch.Text);
        }
    }

    public class QuoteExtended :  Quote
    {
        public new string DealName { get; set; }
    }
}