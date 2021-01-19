using Crm6.App_Code;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Crm6.Deals.DealDetail
{
    public partial class DealDetail : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentUser = LoginUser.GetLoggedInUser();

                if (currentUser != null)
                {
                    int dealId = 0;
                    lblUserId.Text = currentUser.User.UserId.ToString();
                    lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();
                    lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
                    lblUsername.Text = currentUser.User.FullName;
                    var dealSubscriberId = currentUser.User.SubscriberId;
                    if (Request.QueryString["dealsubscriberid"] != null && Utils.IsNumeric(Request.QueryString["dealsubscriberid"]) && int.Parse(Request.QueryString["dealsubscriberid"]) > 0)
                    {
                        dealSubscriberId = int.Parse(Request.QueryString["dealsubscriberid"]);
                    }
                    lblDealSubscriberId.Text = dealSubscriberId.ToString();

                    // get deal subscriber connection
                    var dealSubscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                        .GlobalSubscribers.Where(t => t.SubscriberId == dealSubscriberId)
                        .Select(t => t.DataCenter).FirstOrDefault();
                    var connection = LoginUser.GetConnectionForDataCenter(dealSubscriberDataCenter);
                    var context = new DbFirstFreightDataContext(connection);

                    int companyId = 0;

                    if (Request.QueryString["dealId"] != null && Utils.IsNumeric(Request.QueryString["dealId"]) && int.Parse(Request.QueryString["dealId"]) > 0)
                    {
                        dealId = int.Parse(Request.QueryString["dealId"]);
                        lblDealId.Text = dealId.ToString();
                        var deal = context.Deals.Where(x => x.DealId == dealId && x.SubscriberId == dealSubscriberId).FirstOrDefault();
                        if (deal != null)
                        {
                            companyId = deal.CompanyId;
                            if (companyId > 0)
                            {
                                lblCompanyId.Text = companyId.ToString();

                                // primary contact
                                var primaryContactId = deal.PrimaryContactId;
                                if (primaryContactId > 0)
                                {
                                    var primaryContact = context.Contacts.FirstOrDefault(t => t.ContactId == primaryContactId);
                                    if (primaryContact != null)
                                    {
                                        lblPrimaryContactName.Text = primaryContact.ContactName;
                                        lblPrimaryContactCountry.Text = primaryContact.BusinessCity + ", " + primaryContact.BusinessCountry;
                                        lblPrimaryContactPosition.Text = primaryContact.Title;
                                        if (!string.IsNullOrWhiteSpace(primaryContact.BusinessPhone))
                                        {
                                            lblPrimaryContactPhone.Text = primaryContact.BusinessPhone;
                                            aPrimaryContactPhone.Attributes["href"] = "tel:" + primaryContact.BusinessPhone;
                                        }
                                        lblPrimaryContactEmail.Text = Utils.AddWordBreakOpportunities(primaryContact.Email);
                                        aPrimaryContactEmail.Attributes["href"] = "mailto:" + primaryContact.Email;

                                        //profile pic / svg initials
                                        var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(primaryContact.ContactId, primaryContact.SubscriberId, "contact");
                                        imgPrimaryContact.Attributes["src"] = profilePicUrl;
                                    }
                                    else
                                    {
                                        // hide primary contact container
                                        divPrimaryContactContainer.Visible = false;
                                    }
                                }
                                else
                                {
                                    // hide primary contact container
                                    divPrimaryContactContainer.Visible = false;
                                }

                                // deal owner
                                if (deal.DealOwnerId > 0)
                                {
                                    // load sales reps
                                    //LoadSalesReps(companyOwnerUserId.Value, currentUser.Subscriber.SubscriberId);
                                    var dealOwner = context.Users.FirstOrDefault(t => t.UserId == deal.DealOwnerId && t.SubscriberId == deal.SubscriberId);
                                    if (dealOwner != null)
                                    {
                                        lblSalesOwnerId.Text = dealOwner.UserId.ToString();
                                        lblSalesRepName.Text = dealOwner.FullName;
                                        lblSalesRepCountry.Text = dealOwner.City + ", " + dealOwner.CountryName;
                                        lblSalesRepPosition.Text = dealOwner.Title;
                                        if (!string.IsNullOrWhiteSpace(dealOwner.Phone))
                                        {
                                            lblSalesRepPhone.Text = dealOwner.Phone;
                                            aSalesRepPhone.Attributes["href"] = "tel:" + dealOwner.Phone;
                                        }
                                        lblSalesRepEmail.Text = Utils.AddWordBreakOpportunities(dealOwner.EmailAddress);
                                        aSalesRepEmail.Attributes["href"] = "mailto:" + dealOwner.EmailAddress;

                                        //profile pic / svg initials
                                        var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(dealOwner.UserId, dealOwner.SubscriberId);
                                        imgProfile.Attributes["src"] = profilePicUrl;
                                    }
                                    else
                                    {
                                        // hide company owner container
                                        divSalesOwnerContainer.Attributes["class"] += " hide";
                                    }
                                }
                                else
                                {
                                    // hide company owner container
                                    divSalesOwnerContainer.Attributes["class"] += " hide";
                                }

                                PopulateQuoteFilterCombos(currentUser.Subscriber.SubscriberId, connection);
                                LoadQuotes(currentUser.Subscriber.SubscriberId, companyId, "", "", "", connection);
                                LoadUserActivity(currentUser.Subscriber.SubscriberId, dealId, connection);
                                LoadCompany();
                                LoadDeal(dealId);
                                LoadLanes(dealId, currentUser.Subscriber.SubscriberId, connection);
                                PopuplateSalesStages(deal.SalesStageId, deal.SalesStageName);
                            }
                        }
                        else
                        {
                            // TO DO: goto invalid page
                            return;
                        }
                    }
                }
                else
                {
                    Response.Redirect("/Login.aspx");
                }
            }
        }


        private void LoadDeal(int dealId)
        {
            if (dealId > 0)
            {
                var dealSubscriberId = int.Parse(lblSubscriberId.Text);
                var deal = new Helpers.Deals().GetDeal(dealId, dealSubscriberId);
                if (deal != null)
                {
                    lblSalesStage.Text = deal.SalesStageName;

                    if (deal.SalesStageName == "Qualifying")
                    {
                        lblSalesStage.CssClass = "border-status lblue";
                    }
                    if (deal.SalesStageName == "Negotiation")
                    {
                        lblSalesStage.CssClass = "border-status mblue";
                    }
                    if (deal.SalesStageName == "Final Negotiation")
                    {
                        lblSalesStage.CssClass = "border-status";
                    }
                    if (deal.SalesStageName == "Trial Shipment")
                    {
                        lblSalesStage.CssClass = "border-status green";
                    }
                    if (deal.SalesStageName == "Lost")
                    {
                        lblSalesStage.CssClass = "border-status red";
                    }
                    if (deal.SalesStageName == "Won")
                    {
                        lblSalesStage.CssClass = "border-status dgreen";
                    }
                    if (deal.SalesStageName == "Stalled")
                    {
                        lblSalesStage.CssClass = "border-status grey";
                    }

                    lblDealNameTop.Text = deal.DealName;

                    lblDealType.Text = deal.DealType;
                    lblIndustry.Text = deal.Industry;
                    lblCompetitors.DataSource = (deal.Competitors + "").Split(',');
                    lblCompetitors.DataBind();
                    lblCommodities.DataSource = (deal.Commodities + "").Split(',');
                    lblCommodities.DataBind();
                    if (!string.IsNullOrEmpty(deal.Comments)) lblComments.Text = deal.Comments;
                    else wrpComments.Visible = false;

                    if (!string.IsNullOrWhiteSpace(deal.Campaign))
                    {
                        lblCampaigns.Text = deal.Campaign.Replace(",", ", ");
                    }
                    else
                    {
                        lblCampaigns.Text = "-";
                    }
                    if (!string.IsNullOrWhiteSpace(deal.Incoterms))
                    {
                        lblIncoterms.Text = deal.Incoterms;
                    }
                    else
                    {
                        wrpIncoterms.Visible = false;
                    }

                    // load key dates
                    LoadKeyDates(deal);

                }
                else
                {
                    // go back to deal list or show error page
                }
            }
        }

        private void LoadKeyDates(Deal deal)
        {
            if (deal.DateProposalDue.HasValue && deal.DateProposalDue != DateTime.MinValue)
                lblProposalDate.Text = deal.DateProposalDue.Value.ToString("dd-MMM-yy");

            if (deal.DecisionDate.HasValue && deal.DecisionDate != DateTime.MinValue)
                lblDecisionDate.Text = deal.DecisionDate.Value.ToString("dd-MMM-yy");

            if (deal.EstimatedStartDate.HasValue && deal.EstimatedStartDate != DateTime.MinValue)
                lblFirstShipmentDate.Text = deal.EstimatedStartDate.Value.ToString("dd-MMM-yy");

            if (deal.ContractEndDate.HasValue && deal.ContractEndDate != DateTime.MinValue)
                lblContractEndDate.Text = deal.ContractEndDate.Value.ToString("dd-MMM-yy");
        }


        private void LoadUserActivity(int subscriberId, int dealId, string connection)
        {
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var activity = (from t in context.UserActivities where t.DealId == dealId && t.SubscriberId == subscriberId select t).OrderByDescending(x => x.UserActivityTimestamp);

            if (activity.Any())
            {
                rptUserActivity.DataSource = activity;
                rptUserActivity.DataBind();
            }
        }


        private void LoadLanes(int dealId, int subscriberId, string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var lanes = context.Lanes.Where(x => x.DealId == dealId && x.SubscriberId == subscriberId && !x.Deleted);
            if (lanes.Any())
            {
                var lanesList = lanes.ToList();

                for (int i = 0; i < lanesList.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(lanesList[i].CurrencyCode))
                    {
                        if (lanesList[i].CurrencyCode.ToLower().Equals("usd"))
                        {
                            lanesList[i].CurrencyCode = "$";
                        }
                        else if (lanesList[i].CurrencyCode.ToLower().Equals("eur"))
                        {
                            lanesList[i].CurrencyCode = "€";
                        }
                        else if (lanesList[i].CurrencyCode.ToLower().Equals("gbp"))
                        {
                            lanesList[i].CurrencyCode = "£";
                        }
                        else if (!string.IsNullOrEmpty(lanesList[i].CurrencyCode))
                        {
                            // get currency symbol
                            var currency = new Currencies().GetCurrencySymbolFromCode(lanesList[i].CurrencyCode);
                            if (!string.IsNullOrEmpty(currency))
                            {
                                lanesList[i].CurrencyCode = currency;
                            }
                        }
                    }

                    if (lanesList[i].Service.Contains("Logistics"))
                    {
                        if (!string.IsNullOrWhiteSpace(lanesList[i].ServiceLocation))
                        {
                            int locationId = 0;
                            int.TryParse(lanesList[i].ServiceLocation, out locationId);

                            if (locationId > 0)
                                lanesList[i].ServiceLocation = context.Locations.Where(x => x.LocationId == locationId).Select(x => x.LocationName).FirstOrDefault();
                        }
                    }
                }

                rptLanes.DataSource = lanes;
                rptLanes.DataBind();
                noLanes.Visible = false;
            }
            else
            {
                noLanes.Visible = true;
            }
        }

        private void PopuplateSalesStages(int salesStageId, string salesStageName)
        {
            var subscriberId = int.Parse(lblDealSubscriberId.Text);
            var salesStages = new SalesStages().GetSalesStages(subscriberId).Where(t => !t.Won && !t.Lost && t.SalesStageName != "Stalled").Select(t => new
            {
                t.SalesStageName,
                t.SalesStageId,
                Selected = t.SalesStageName.ToLower() == salesStageName.ToLower()
            }).ToList();
            //rbtSalesStageTimeline.DataSource = salesStages;
            //rbtSalesStageTimeline.DataBind();
        }

        private void PopulateQuoteFilterCombos(int subscriberId, string connection)
        {
            var context = new DbFirstFreightDataContext(connection);

            var quotes = (from t in context.Quotes where t.SubscriberId == subscriberId select t);

            if (quotes.Any())
            {
                //  ddlSalesRepFilter.DataSource = quotes;
                //   ddlSalesRepFilter.DataTextField = "CustomerName";
                //   ddlSalesRepFilter.DataValueField = "CustomerName";
                //   ddlSalesRepFilter.DataBind();

                //   ddlSalesRepFilter.Items.Add("Sales Rep");
                //   ddlSalesRepFilter.SelectedIndex = ddlSalesRepFilter.Items.Count - 1;

                //   ddlBranch.DataSource = quotes;
                //   ddlBranch.DataTextField = "BranchName";
                //   ddlBranch.DataValueField = "BranchName";
                //   ddlBranch.DataBind();

                //   ddlBranch.Items.Add("Branch");
                //  ddlBranch.SelectedIndex = ddlBranch.Items.Count - 1;

                //   ddlStatus.DataSource = quotes;
                //   ddlStatus.DataTextField = "QuoteStatus";
                //  ddlStatus.DataValueField = "QuoteStatus";
                //   ddlStatus.DataBind();

                //    ddlStatus.Items.Add("Status");
                //   ddlStatus.SelectedIndex = ddlStatus.Items.Count - 1;
            }
        }

        private void LoadQuotes(int subscriberId, int companyId, string branch, string status, string salesRep, string connection)
        {
            if (companyId > 0)
            {
                // var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);

                var quotes = (from t in context.Quotes where t.CompanyId == companyId && t.SubscriberId == subscriberId select t);

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
                    // rptQuotes.DataSource = quotes;
                    // rptQuotes.DataBind();
                }
            }
        }


        private void LoadCompany()
        {
            var companyId = int.Parse(lblCompanyId.Text);
            var subscriberId = int.Parse(lblDealSubscriberId.Text);
            var company = new Helpers.Companies().GetCompany(companyId, subscriberId);
            if (company != null)
            {
                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                // user data context  
                var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
                var currentUser = userContext.Users.Where(t => t.UserId == LoginUser.GetLoggedInUserId()).FirstOrDefault();
                var userTimeZone = currentUser.TimeZone;

                DateTime lastUpdateUserTimezone = company.LastUpdate;
                DateTime lastCreatedUserTimezone = company.CreatedDate;
                TimeZoneInfo cstZone = null;

                var utcOffsetDefault = "";
                var timezone = sharedContext.TimeZones.FirstOrDefault(t => t.TimeZoneName == userTimeZone);
                if (timezone != null)
                {
                    if (!string.IsNullOrWhiteSpace(timezone.EnumTimeZoneID))
                    {
                        cstZone = TimeZoneInfo.FindSystemTimeZoneById(timezone.EnumTimeZoneID.Trim());
                    }

                    utcOffsetDefault = timezone.UtcOffset;

                    if (cstZone != null)
                        lastUpdateUserTimezone = TimeZoneInfo.ConvertTimeFromUtc(lastUpdateUserTimezone, cstZone);
                    else
                        lastUpdateUserTimezone = new CalendarEvents().ConvertUtcToUserDateTime(lastUpdateUserTimezone, LoginUser.GetLoggedInUserId(), userTimeZone, utcOffsetDefault);

                    if (cstZone != null)
                        lastCreatedUserTimezone = TimeZoneInfo.ConvertTimeFromUtc(lastCreatedUserTimezone, cstZone);
                    else
                        lastCreatedUserTimezone = new CalendarEvents().ConvertUtcToUserDateTime(lastCreatedUserTimezone, LoginUser.GetLoggedInUserId(), userTimeZone, utcOffsetDefault);
                }

                lblCompanyName.Text = company.CompanyName;
                lblGlobalCompanyId.Text = company.CompanyIdGlobal.ToString();
                lblCompanyOwnerId.Text = company.CompanyOwnerUserId.ToString();
                lblIndustry.Text = string.IsNullOrEmpty(company.Industry) ? " " : company.Industry;
                if (company.LastUpdate != DateTime.MinValue) lblLastUpdatedDate.Text = lastUpdateUserTimezone.ToString("dd-MMM-yy");
                lblLastUpdatedBy.Text = string.IsNullOrEmpty(company.UpdateUserName) ? "" : "by " + company.UpdateUserName;
                if (company.CreatedDate != DateTime.MinValue) lblCreatedDate.Text = lastCreatedUserTimezone.ToString("dd-MMM-yy");
                lblCreatedBy.Text = string.IsNullOrEmpty(company.CreatedUserName) ? "" : "by " + company.CreatedUserName;
                
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
                if (addresses.Count > 0)
                {
                    var address = string.Join("<span>,</span><br>", addresses.ToArray());
                    lblAddress.Text = string.IsNullOrEmpty(address) ? "-" : address;
                }
                else wrpAddress.Visible = false;

                if (!string.IsNullOrEmpty(company.Phone)) lblPhone.Text = string.IsNullOrEmpty(company.Phone) ? "-" : company.Phone;
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
                else wrpWebsite.Visible = false;

                //company logo / svg initials
                var logoUrl = new Helpers.Companies().GetCompanyLogoUrl(company.CompanyId, company.SubscriberId);
                imgCompany.Attributes["src"] = logoUrl;
            }
        }
    }
}
