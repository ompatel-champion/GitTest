﻿using System;
using System.Web.UI;

namespace Crm6.Activities.DetailViews
{
    public partial class Notes : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            { 
            }
        }
         
    }
}