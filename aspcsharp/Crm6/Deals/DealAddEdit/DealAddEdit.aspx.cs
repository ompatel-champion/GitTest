using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Crm6.Deals
{
    public partial class DealAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUserId.Text = currentUser.User.UserId.ToString();
            if (!Page.IsPostBack)
            {
                var dealSubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["dealsubscriberId"] != null && Utils.IsNumeric(Request.QueryString["dealsubscriberId"]) && int.Parse(Request.QueryString["dealsubscriberId"]) > 0)
                {
                    dealSubscriberId = int.Parse(Request.QueryString["dealsubscriberId"]);
                }
                lblDealSubscriberId.Text = dealSubscriberId.ToString();

                // load drop downs
                LoadSalesStages(dealSubscriberId);
                LoadUsers(dealSubscriberId);
                LoadDealTypes(dealSubscriberId);
                LoadIndustries(dealSubscriberId);
                LoadCommodities(dealSubscriberId);
                LoadIncoterms(dealSubscriberId);
                LoadCompetitors(dealSubscriberId);
                LoadCampaigns(dealSubscriberId);

                if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                {
                    lblDealId.Text = Request.QueryString["dealId"];
                    LoadDeal();
                }

                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    //  retrieve company and set company json
                    var companyId = int.Parse(Request.QueryString["companyId"]);
                    lblCompanyId.Text = Request.QueryString["companyId"];
                    SetCompanyItem(companyId, dealSubscriberId);
                }

                if (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)
                {
                    //  retrieve contact and set company and contact json
                    var contactId = int.Parse(Request.QueryString["contactId"]);
                    SetContactItem(contactId, dealSubscriberId);
                }
            }
        }

        #region set form elements

        private void SetCompanyItem(int companyId, int subscriberId)
        {
            var dealSubscriberId = int.Parse(lblDealSubscriberId.Text);
            // TODO: GlobalCompany
            var company = new Helpers.Companies().GetCompany(companyId, dealSubscriberId);
            if (company != null && !company.Deleted && company.CompanyIdGlobal > 0)
            {
                lblCompanyId.Text = company.CompanyId.ToString();
                var companyName = company.CompanyName + (company.City != null ? " - " + company.City : "");
                ddlCompany.Items.Add(new ListItem(companyName, company.CompanyIdGlobal.ToString(), true));
                ddlCompany.Enabled = false;
                SetDealIndustryFromCompany(companyId, subscriberId);
                if (int.Parse(lblDealId.Text) < 1)
                {
                    if (!string.IsNullOrWhiteSpace(company.Industry) && ddlIndustry.Items.FindByValue(company.Industry) != null)
                    {
                        ddlIndustry.SelectedValue = company.Industry;
                    }
                }
            }
        }


        private void SetContactItem(int contactId, int subscriberId)
        {
            var contact = new Helpers.Contacts().GetContact(contactId, subscriberId);
            if (contact != null)
            {
                // TODO: GlobalCompany
                SetCompanyItem(contact.Contact.CompanyId, subscriberId);
                ddlContact.Items.Add(new ListItem(contact.Contact.FirstName + " " + contact.Contact.LastName,
                    contactId.ToString(), true));
                ddlContact.Enabled = false;
            }
        }


        private void SetCommodities(App_Code.Deal deal)
        {
            if (!string.IsNullOrWhiteSpace(deal.Commodities))
            {
                var commodities = deal.Commodities.Split(',');

                foreach (var commodity in commodities)
                {
                    if (ddlCommodities.Items.FindByValue(commodity) == null)
                    {
                        ddlCommodities.Items.Add(new ListItem(commodity, commodity));
                    }
                }
            }
            hdnCommodities.Value = deal.Commodities;
        }


        private void SetIncoterms(App_Code.Deal deal)
        {
            hdnIncoterms.Value = deal.Incoterms;
        }


        private void SetIndustries(App_Code.Deal deal)
        {
            hdnIndustry.Value = deal.Industry;
        }


        private void SetCompetitors(App_Code.Deal deal)
        {
            if (!string.IsNullOrWhiteSpace(deal.Competitors))
            {
                var competitors = deal.Competitors.Split(',');

                foreach (var competitor in competitors)
                {
                    if (ddlCompetitors.Items.FindByValue(competitor) == null)
                    {
                        ddlCompetitors.Items.Add(new ListItem(competitor, competitor));
                    }
                }
            }
            hdnCompetitors.Value = deal.Competitors;
        }


        private void SetCampaigns(App_Code.Deal deal)
        {
            if (!string.IsNullOrWhiteSpace(deal.Campaign))
            {
                var campaigns = deal.Campaign.Split(',');
                foreach (var campaign in campaigns)
                {
                    if (ddlCampaign.Items.FindByValue(campaign) == null)
                    {
                        ddlCampaign.Items.Add(new ListItem(campaign, campaign));
                    }
                }
            }
            hdnCampaigns.Value = deal.Campaign;
        }


        private void SetDealIndustryFromCompany(int companyId, int subscriberId)
        {
            // sets deal industry from company industry
            var industry = new Helpers.Companies().GetCompanyIndustry(companyId, subscriberId);
            ddlIndustry.SelectedValue = industry;
        }

        #endregion


        #region DropDowns

        private void LoadSalesStages(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var salesStages = new DropdownHelper().GetSalesStages(subscriberId, true);
            foreach (var stage in salesStages)
            {
                ddlSalesStage.Items.Add(new ListItem(stage.SelectText, stage.SelectText));
            }
        }


        private void LoadCommodities(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var commodities = new DropdownHelper().GetCommodities(subscriberId);
            foreach (var c in commodities)
            {
                ddlCommodities.Items.Add(new ListItem(c.SelectText, c.SelectText));
            }
        }


        private void LoadCompetitors(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var competitors = new DropdownHelper().GetCompetitors(subscriberId);
            foreach (var c in competitors)
            {
                ddlCompetitors.Items.Add(new ListItem(c.SelectText, c.SelectText));
            }
        }


        private void LoadIncoterms(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var commodities = new DropdownHelper().GetIncoterms(subscriberId);
            foreach (var i in commodities)
            {
                ddlIncoterms.Items.Add(new ListItem(i.SelectText, i.SelectText));
            }
        }

        private void LoadIndustries(int subscriberId)
        {
            var industries = new DropdownHelper().GetIndustries(subscriberId);
            ddlIndustry.Items.Add(new ListItem("", ""));
            foreach (var industry in industries)
            {
                ddlIndustry.Items.Add(new ListItem(industry.SelectText, industry.SelectValue));
            }
        }


        private void LoadCampaigns(int subscriberId)
        {
            var campaigns = new DropdownHelper().GetCampaigns(subscriberId);
            foreach (var campaign in campaigns)
            {
                ddlCampaign.Items.Add(new ListItem(campaign.SelectText, campaign.SelectValue));
            }
        }


        private void LoadUsers(int subscriberId)
        {
            // Global Users
            var salesReps = new DropdownHelper().GetSalesReps(subscriberId);
            ddlDealOwner.Items.Add(new ListItem("Sales Rep...", "0"));
            foreach (var salesRep in salesReps)
            {
                ddlDealOwner.Items.Add(new ListItem(salesRep.SelectText, salesRep.SelectValue.ToString()));
            }
            // select the logged in user as the deal owner
            var loggedinUserId = lblUserId.Text;
            if (ddlDealOwner.Items.FindByValue(loggedinUserId) != null)
            {
                ddlDealOwner.SelectedValue = loggedinUserId;
            }
        }


        private void LoadDealTypes(int subscriberId)
        {
            var dealTypes = new DropdownHelper().GetDealTypes(subscriberId);
            ddlDealType.Items.Add(new ListItem("", ""));
            foreach (var dealType in dealTypes)
            {
                ddlDealType.Items.Add(new ListItem(dealType.SelectText, dealType.SelectValue.ToString()));
            }
        }

        #endregion


        private void LoadDeal()
        {
            var dealId = int.Parse(lblDealId.Text);
            if (dealId > 0)
            {
                lblBreadcrumbHeader.Text = "Edit Deal";
                var dealSubscriberId = int.Parse(lblDealSubscriberId.Text);

                var deal = new Helpers.Deals().GetDeal(dealId, dealSubscriberId);
                if (deal != null)
                {
                    lblDealSubscriberId.Text = deal.SubscriberId.ToString();
                    txtDealName.Text = deal.DealName;
                    var companyCity = new Helpers.Companies().GetCompanyCity(deal.CompanyId, dealSubscriberId);
                    var companyName = deal.CompanyName + " - " + companyCity;

                    var company = new Helpers.Companies().GetCompany(deal.CompanyId, int.Parse(lblSubscriberId.Text));

                    if (!company.Deleted && company.CompanyIdGlobal > 0)
                        ddlCompany.Items.Add(new ListItem(companyName, company.CompanyIdGlobal.ToString(), true));

                    ddlContact.Items.Add(new ListItem(deal.PrimaryContactName, deal.PrimaryContactId.ToString(), true));

                    // TODO: Where to select default value
                    ddlDealType.SelectedValue = deal.DealType;
                    ddlSalesStage.SelectedValue = deal.SalesStageName.ToString();
                    if (ddlDealOwner.Items.FindByValue(deal.DealOwnerId.ToString()) != null)
                        ddlDealOwner.SelectedValue = deal.DealOwnerId.ToString();

                    SetCommodities(deal);
                    SetIndustries(deal);
                    SetCompetitors(deal);
                    SetIncoterms(deal);
                    SetCampaigns(deal);

                    // initial dates - if exist
                    lblProposalDate.Text = deal.DateProposalDue?.ToString("dd-MMM-yy") ?? "";
                    lblDecisionDate.Text = deal.DecisionDate?.ToString("dd-MMM-yy") ?? "";
                    lblFirstShipmentDate.Text = deal.EstimatedStartDate?.ToString("dd-MMM-yy") ?? "";
                    lblContractEndDate.Text = deal.ContractEndDate?.ToString("dd-MMM-yy") ?? "";
                    // edit dates
                    txtProposalDate.Text = deal.DateProposalDue?.ToString("dd-MMM-yy") ?? "";
                    txtDecisionDate.Text = deal.DecisionDate?.ToString("dd-MMM-yy") ?? "";
                    txtFirstShipmentDate.Text = deal.EstimatedStartDate?.ToString("dd-MMM-yy") ?? "";
                    txtContractEndDate.Text = deal.ContractEndDate?.ToString("dd-MMM-yy") ?? "";
                    txtComments.Text = deal.Comments;

                    // industry
                    var industry = deal.Industry + "";
                    if (ddlIndustry.Items.FindByValue(industry) != null)
                        ddlIndustry.SelectedValue = industry;

                    // won/lost reasons
                    if (deal.SalesStageName == "Won" || deal.SalesStageName == "Lost")
                    {
                        if (!string.IsNullOrEmpty(deal.ReasonWonLost) && !deal.ReasonWonLost.Contains("Select Reason"))
                        {
                            // populate deal reasons
                            var reasons = new List<SelectList>();
                            reasons.Add(new SelectList { SelectText = "Select Reason", SelectValue = "" });
                            if (deal.SalesStageName == "Won")
                                // won
                                reasons.AddRange(new DropdownHelper().GetWonReasons(int.Parse(lblSubscriberId.Text)));
                            else
                                // lost
                                reasons.AddRange(new DropdownHelper().GetLostReasons(int.Parse(lblSubscriberId.Text)));
                            foreach (var reason in reasons)
                            {
                                ddlWonLostReason.Items.Add(new ListItem(reason.SelectText, reason.SelectValue));
                            }
                            ddlWonLostReason.SelectedValue = deal.ReasonWonLost;
                        }
                    }
                }
            }
        }

    }
}
