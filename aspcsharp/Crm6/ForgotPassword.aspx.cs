using System;
using System.Web.UI;

namespace Crm6
{
    public partial class ForgotPassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendResetLink_Click(object sender, EventArgs e)
        {
            divError.Visible = false;
            divSuccess.Visible = false;

            var result = new Helpers.ResetPassword().SendForgotPasswordEmail(txtEmailAddress.Text.Trim());
            if (result.IsError)
            {
                divError.Visible = true;
                divSuccess.Visible = false;
            }
            else {
                divError.Visible = false;
                divSuccess.Visible = true;
               // divContent.Visible = false;
            }
        }

    }
}
