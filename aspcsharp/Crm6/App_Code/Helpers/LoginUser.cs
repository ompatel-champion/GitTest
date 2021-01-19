using System.Web;
using System.Web.SessionState;
using Models;
using System.Linq;
using Crm6.App_Code;
using Helpers;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using System;

public class LoginUser : IRequiresSessionState
{

    #region *** connection strings ***


    /// <summary>
    /// get logged in user data center connection
    /// </summary>
    /// <returns></returns>
    public static string GetConnection()
    {
        if (HttpContext.Current.Session != null && HttpContext.Current.Session["DataCenterConnectionstring"] != null)
        {
            return HttpContext.Current.Session["DataCenterConnectionstring"].ToString();
        }
        return "";
    }


    /// <summary>
    /// get shared connection for logged in user
    /// </summary>
    /// <returns></returns>
    public static string GetSharedConnection()
    {
        if (HttpContext.Current.Session != null && HttpContext.Current.Session["SharedCenterConnectionstring"] != null)
        {
            return HttpContext.Current.Session["SharedCenterConnectionstring"].ToString();
        }
        return "";
    }


    /// <summary>
    /// get logged in user login connection string
    /// </summary> 
    /// <returns></returns>
    public static string GetLoginConnection()
    {
        if (HttpContext.Current.Session != null && HttpContext.Current.Session["LoginCenterConnectionstring"] != null)
        {
            return HttpContext.Current.Session["LoginCenterConnectionstring"].ToString();
        }
        return "";
    }


    /// <summary>
    /// get shared connection for passed data center
    /// </summary>
    /// <param name="dataCenter"></param>
    /// <returns></returns>
    public static string GetSharedConnectionForDataCenter(string dataCenter = "")
    {
        // if datacenter is not passed, get it from user session
        if (string.IsNullOrEmpty(dataCenter))
        {
            dataCenter = HttpContext.Current.Session["DataCenter"].ToString();
        }
        var sharedConnectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
        switch (dataCenter.ToLower().Trim())
        {
            case "dev":
                sharedConnectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                break;
            case "emea":
                sharedConnectionString = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "hkg":
                sharedConnectionString = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "kweus":
                sharedConnectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "kweusa":
                sharedConnectionString = "Data Source=hkg.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "kwecn":
                sharedConnectionString = "Data Source=hkg.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "sinotrans":
                sharedConnectionString = "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
                break;
            case "usa":
                sharedConnectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
        }
        return sharedConnectionString;
    }


    /// <summary>
    /// get writable connection for data center
    /// </summary>
    /// <param name="dataCenter"></param>
    /// <returns></returns>
    public static string GetWritableSharedConnectionForDataCenter(string dataCenter)
    {
        string sharedConnection;
        switch (dataCenter.ToLower().Trim())
        {
            case "sinotrans":
                sharedConnection = "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
                break;
            case "dev":
                sharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                break;
            default:
                // all active users
                sharedConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
        }
        return sharedConnection;
    }


    /// <summary>
    /// get writable shared connection id
    /// </summary>
    /// <param name="subscriberId"></param>
    /// <returns></returns>
    public static string GetWritableSharedConnectionForSubscriberId(int subscriberId)
    {
        string sharedConnection;
        var dataCenter = "";

        try
        {
            dataCenter = new Subscribers().GetDataCenter(subscriberId) ?? "";
        }
        catch (Exception) { }
        
        switch (dataCenter.ToLower().Trim())
        {
            case "sinotrans":
                sharedConnection = "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
                break;
            case "dev":
                sharedConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Shared;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                break;
            default:
                // all active users
                sharedConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
        }

        return sharedConnection;
    }



    /// <summary>
    /// returns the connection for passed data center
    /// </summary>
    /// <param name="dataCenter"></param>
    /// <returns></returns>
    public static string GetConnectionForDataCenter(string dataCenter = "")
    {
        // if datacenter is not passed, get it from user session
        if (string.IsNullOrEmpty(dataCenter))
        {
            dataCenter = HttpContext.Current.Session["DataCenter"].ToString();
        }
        // default connection 
        var connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
        switch (dataCenter.ToLower().Trim())
        {
            case "dev":
                connectionString = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                break;
            case "emea":
                connectionString = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "hkg":
                connectionString = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "usa":
                connectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "kweus":
                connectionString = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_KWEUS;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "kwecn":
                connectionString = "Data Source=hkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
                break;
            case "sinotrans":
                connectionString = "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Sinotrans;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
                break;
        }
        return connectionString;
    }



    /// <summary>
    /// validate user, first look for the global user in live security database, if not found check on test database 
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static UserModel ValidateUser(string emailAddress, string password)
    {
        // look for the user in live security database
        var loginConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
        var loginContext = new DbLoginDataContext(loginConnection);
        GlobalUser globalUser = null;
        try
        {
            // check for the global user
            globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()));

        }
        catch (System.Exception)
        {
        }

        if (globalUser == null)
        {
            // not found in the live database - look in the dev test
            loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
            loginContext = new DbLoginDataContext(loginConnection);
            globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()));
        }

        if (globalUser != null)
        {
            // get data center connection string based on global user 
            var connection = GetConnectionForDataCenter(globalUser.DataCenter);
            var sharedConnection = GetSharedConnectionForDataCenter(globalUser.DataCenter);

            if (!string.IsNullOrEmpty(connection))
            {
                var context = new DbFirstFreightDataContext(connection);
                var user = context.Users.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()) && !u.Deleted);
                if (user != null && (password.Equals(user.Password) || PasswordEncryptor.VerifyPassword(password, user.PasswordHashed)))
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
                        CreateUserSession(userModel);

                        // get subscriber
                        userModel.Subscriber = context.Subscribers.FirstOrDefault(s => s.SubscriberId == user.SubscriberId);
                        var docs = new Documents().GetDocumentsByDocType(1, user.UserId, user.SubscriberId);
                        if (docs.Count > 0) userModel.ProfilePicture = docs.FirstOrDefault();

                        // save user session with profile pic again
                        CreateUserSession(userModel);

                        return userModel;
                    }
                }
            }
        }
        return null;
    }



    #endregion




    #region *** login user ***


    public static void CreateUserSession(UserModel user)
    {
        // convert user model object to JSON string
        HttpContext.Current.Session["ffuser"] = user;
        HttpContext.Current.Session["DataCenter"] = user.DataCenter;
        HttpContext.Current.Session["DataCenterConnectionstring"] = user.DataCenterConnection;
        HttpContext.Current.Session["SharedCenterConnectionstring"] = user.SharedConnection;
        HttpContext.Current.Session["LoginCenterConnectionstring"] = user.LoginConnection;
    }

    // delete/flush all the sessions
    public static void DeleteUserSession()
    {
        HttpContext.Current.Session.RemoveAll();
        HttpContext.Current.Session.Abandon();
    }


    /// <summary>
    /// this function returns the logged in user object
    /// </summary>
    /// <returns></returns>
    public static UserModel GetLoggedInUser()
    {
        if (HttpContext.Current.Session != null && HttpContext.Current.Session["ffuser"] != null && (HttpContext.Current.Session["ffuser"] is UserModel))
        {
            return (UserModel)HttpContext.Current.Session["ffuser"];
        }
        return null;
    }


    /// <summary>
    /// This function returns the logged in user's user Id
    /// </summary>
    /// <returns></returns>
    public static int GetLoggedInUserId()
    {
        if (HttpContext.Current.Session["ffuser"] != null && (HttpContext.Current.Session["ffuser"] is UserModel))
        {
            return ((UserModel)HttpContext.Current.Session["ffuser"]).User?.UserId ?? 0;
        }
        return 0;
    }


    public static string GetStorageAccountConnection(string dataCenter = "")
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=crm6;AccountKey=GcUpwhDKrWE79horaDsrlugOPXAz6CIvP558tHPnJ/StxwbnUOIqGKGAskD4bAw32Ygp5Rz/IIxDEDd5fUKmTQ==;EndpointSuffix=core.windows.net";
        return connectionString;
    }


    public static void PerformSwitch(int userId, int subscriberId)
    {
        // look for the user in live security database
        var loginConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
        var loginContext = new DbLoginDataContext(loginConnection);
        GlobalUser globalUser = null;
        try
        {
            // check for the global user
            globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
        }
        catch (System.Exception)
        {
        }

        if (globalUser == null)
        {
            // not found in the live database - look in the dev test
            loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
            loginContext = new DbLoginDataContext(loginConnection);
            globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
        }


        if (globalUser != null)
        {
            // get data center connection string based on global user 
            var connection = GetConnectionForDataCenter(globalUser.DataCenter);
            var sharedConnection = GetSharedConnectionForDataCenter(globalUser.DataCenter);

            if (!string.IsNullOrEmpty(connection))
            {
                var context = new DbFirstFreightDataContext(connection);
                // valid user - return user model to store in the session
                var userModel = context.Users.Where(u => u.UserId == userId)
                                             .Select(u => new UserModel
                                             {
                                                 User = u,
                                                 DataCenterConnection = connection,
                                                 SharedConnection = sharedConnection,
                                                 LoginConnection = loginConnection,
                                                 DataCenter = globalUser.DataCenter
                                             }).FirstOrDefault();

                if (userModel != null)
                {
                    // create user session
                    CreateUserSession(userModel);

                    // get subscriber
                    userModel.Subscriber = context.Subscribers.FirstOrDefault(s => s.SubscriberId == globalUser.SubscriberId);
                    var docs = new Documents().GetDocumentsByDocType(1, globalUser.UserId, globalUser.SubscriberId);
                    if (docs.Count > 0) userModel.ProfilePicture = docs.FirstOrDefault();

                    // save user session with profile pic again
                    CreateUserSession(userModel);

                }

            }
        }
    }


    #endregion


}