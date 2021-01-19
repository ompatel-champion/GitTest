using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Helpers; 
using Crm6.App_Code;

namespace Crm6.Companies
{
    public partial class PageName : BasePage
    {
       
      protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
           
            if (!Page.IsPostBack)
            {
                
            }
        }

    }
}
