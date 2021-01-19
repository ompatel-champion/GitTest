using Helpers;
using System;
using System.Web;

namespace Crm6.Document
{
    public partial class AddDocument : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["documentId"] != null && int.Parse(Request.QueryString["documentId"]) > 0)
                {
                    lblDocumentId.Text = Request.QueryString["documentId"];
                    LoadDocument();
                }
                else if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                {
                    lblDealId.Text = Request.QueryString["dealId"];
                }
                else if (Request.QueryString["contactId"] != null && int.Parse(Request.QueryString["contactId"]) > 0)
                {
                    lblContactId.Text = Request.QueryString["contactId"];
                }
                else if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    lblCompanyId.Text = Request.QueryString["companyId"];
                }
                
                if (Request.QueryString["globalCompanyId"] != null && int.Parse(Request.QueryString["globalCompanyId"]) > 0)
                {
                    lblGlobalCompanyId.Text = Request.QueryString["globalCompanyId"];
                }
            }
        }


        /// <summary>
        /// load document
        /// </summary>
        private void LoadDocument()
        {
            var subscriberId = HttpContext.Current.Session["subscriberId"] as int? ?? 0;

            int documentId = int.Parse(lblDocumentId.Text);
            var document = new Documents().GetDocumentById(documentId, subscriberId);
            if (document != null)
            { 
                txtDocumentTitle.Text = document.Title;
                txtDocumentDescription.Text = document.Description;
                switch (document.DocumentTypeId)
                {
                    case 4:
                        lblDealId.Text = document.DealId.ToString(); break;
                    case 5:
                        lblContactId.Text = document.ContactId.ToString(); break;
                    case 6:
                        lblCompanyId.Text = document.CompanyId.ToString();
                        lblGlobalCompanyId.Text = document.CompanyIdGlobal.ToString();
                        break;
                }

                // show the document
                docLink.InnerHtml = document.FileName;
                docLink.Attributes["href"] = document.DocumentUrl;
                docPath.InnerHtml = document.DocumentUrl;
            }
        }
    }
}

