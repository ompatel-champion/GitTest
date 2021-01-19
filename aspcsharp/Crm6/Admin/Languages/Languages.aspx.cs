using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Crm6.App_Code.Shared;

namespace Crm6.Admin.Languages
{
    public partial class Languages : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadLanguages();
                LoadLanguagesForCombo(); 

                if (Request.QueryString["status"] != null)
                {
                    ddlStatus.SelectedValue = Request.QueryString["status"];
                }
            }
        }


     

        private void LoadLanguages()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var languages = new Helpers.Languages().GetLanguages();
            rptLanguages.DataSource = languages.OrderBy(x => x.LanguageName);
            rptLanguages.DataBind();
        }




        private void LoadLanguagesForCombo()
        {
            var languages = new Helpers.DropdownHelper().GetLanguages(0); 
            foreach (var language in languages)
            {
                ddlLanguages.Items.Add(new ListItem(language.SelectText, language.SelectValue.ToString()));
            } 
        }


        private void ActiveLanguagesTranslation()
        {
            string retrunTranslation = "<script>$('.border-tabs .btab').removeClass('active');" +
                "$('#divTranslationsTabHeader').addClass('active');" +
                "$('.btab-content').removeClass('active');" +
                "$('#divTranslationsTab').addClass('active');</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "print", retrunTranslation);
        }

         

        protected void btnAddLanguage_Click(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var subscriberId = currentUser.Subscriber.SubscriberId;
            var lt = new Language
            {
                LanguageCode = txtLanguageCode.Text,
                LanguageName = txtLanguageName.Text
            };
            new Helpers.Languages().SaveLanguage(new Helpers.LanguageSaveRequest { Language = lt, SubscriberId = subscriberId });
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

 

    }
}
