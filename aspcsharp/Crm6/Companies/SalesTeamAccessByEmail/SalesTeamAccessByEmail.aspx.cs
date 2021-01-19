using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Crm6.Companies.SalesTeamAccessByEmail
{
    public partial class SalesTeamAccessByEmail :  BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["guid"] != null)
            {
                var guid = Request.QueryString["guid"];

                var sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForSubscriberId(0);
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

                var request = sharedWriteableContext.GlobalCompanyAccessRequests.FirstOrDefault(t => t.Guid == guid);

                if (request == null)
                {
                    var dataCenter = "SINOTRANS";
                    sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForDataCenter(dataCenter);
                    sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);
                    request = sharedWriteableContext.GlobalCompanyAccessRequests.FirstOrDefault(t => t.Guid == guid);
                }
                 
                if (request != null)
                {
                    var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == request.GlobalCompanyId);
                    if (globalCompany != null)
                    {
                        // company owner user id
                        var loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                        var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                        var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == request.CompanyOwnerId);
                        if (globalUser != null)
                        {
                            var connection = LoginUser.GetConnectionForDataCenter(globalUser.DataCenter);
                            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(globalUser.DataCenter);

                            var context = new DbFirstFreightDataContext(connection);
                            var user = context.Users.FirstOrDefault(u => u.UserId == globalUser.UserId && u.SubscriberId == globalUser.SubscriberId && !u.Deleted);

                            if (user != null)
                            {
                                // valid user - return user model to store in the session
                                var userModel = context.Users.Where(u => u.UserId == user.UserId)
                                                             .Select(u => new UserModel
                                                             {
                                                                 User = u,
                                                                 DataCenterConnection = connection,
                                                                 SharedConnection = sharedConnection,
                                                                 LoginConnection = loginConnection,
                                                                 DataCenter = user.DataCenter
                                                             }).FirstOrDefault();

                                if (userModel != null)
                                {
                                    // create user session
                                    LoginUser.CreateUserSession(userModel);

                                    // get subscriber
                                    userModel.Subscriber = context.Subscribers.FirstOrDefault(s => s.SubscriberId == user.SubscriberId);
                                    var docs = new Documents().GetDocumentsByDocType(1, user.UserId, user.SubscriberId);
                                    if (docs.Count > 0) userModel.ProfilePicture = docs.FirstOrDefault();

                                    // save user session with profile pic again
                                    LoginUser.CreateUserSession(userModel);


                                    var requestedGlobalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == request.RequestedGlobalUserId);
                                    if (requestedGlobalUser != null)
                                    {
                                        var companyUser = sharedWriteableContext.LinkGlobalCompanyGlobalUsers.FirstOrDefault(t =>
                                                                        t.GlobalCompanyId == globalCompany.GlobalCompanyId
                                                                        && t.GlobalUserId == requestedGlobalUser.GlobalUserId
                                                                        && !t.Deleted) ?? new LinkGlobalCompanyGlobalUser();

                                        companyUser.GlobalUserName = requestedGlobalUser.FullName;
                                        companyUser.UserSubscriberId = requestedGlobalUser.SubscriberId;
                                        companyUser.GlobalUserId = requestedGlobalUser.GlobalUserId;
                                        companyUser.CreatedBy = globalUser.GlobalUserId;
                                        companyUser.CreatedByName = globalUser.FullName;
                                        companyUser.CreatedDate = DateTime.UtcNow;
                                        companyUser.LinkType = "";
                                        companyUser.GlobalCompanyName = globalCompany.CompanyName;
                                        companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                                        companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                                        companyUser.LastUpdate = DateTime.UtcNow;
                                        companyUser.UpdateUserId = globalUser.GlobalUserId;
                                        companyUser.UpdateUserName = globalUser.FullName;
                                        if (companyUser.Id < 1)
                                        {
                                            sharedWriteableContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                                        }
                                        sharedWriteableContext.SubmitChanges();

                                        var usernames = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                                    .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                                    .Select(t => t.GlobalUserName).ToList();
                                        globalCompany.SalesTeam = string.Join(",", usernames);
                                        sharedWriteableContext.SubmitChanges();

                                        var company = context.Companies.Where(t => t.CompanyId == globalCompany.CompanyId).FirstOrDefault();
                                        if (company != null)
                                        {
                                            company.SalesTeam = string.Join(",", usernames);
                                            context.SubmitChanges();
                                        }

                                        successMessage.InnerHtml = requestedGlobalUser.FullName + " has been added to the sales team of company '" + globalCompany.CompanyName + "'";
                                        divSuccessBox.Visible = true;
                                        divErrorBox.Visible = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                divSuccessBox.Visible = false;
                divErrorBox.Visible = true;
            }
        }
    }
}
