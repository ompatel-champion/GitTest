using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;

public class BasePage : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // check login
        CheckLogin();

        var currentUser = LoginUser.GetLoggedInUser();
        var userId = currentUser.User.UserId.ToString();

        // logrocket
        Page.Header.Controls.Add(new LiteralControl("<script src='https://cdn.logrocket.io/LogRocket.min.js' crossorigin='anonymous'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script>window.LogRocket && window.LogRocket.init('nnz2fs/crm6');</script>"));
        // identify logrocket user
        Page.Header.Controls.Add(new LiteralControl("<script>window.LogRocket && window.LogRocket.identify(" + userId + ", {name: '" + currentUser.User.FullName + "',email: '" + currentUser.User.EmailAddress + "'});</script>"));

        // shared scripts
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/dropzone.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/jquery-3.4.1.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/jquery-ui-1.12.1.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/bootstrap.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/select2.full.min.js' charset='UTF-8'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/jquery.validate.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/intercom.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/spinKit.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/moment.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/js.cookie.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/sweetalert.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/icheck.min.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/sessionAlive.js'></script>")); 
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/languagetranslation-28-mar-2020.js'></script>"));
        Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/bundle/main-07-apr-2020.js'></script>"));
        Regex rx = new Regex(@"\bContactDetail|DealList|DealDetail|CompanyDetail\b", RegexOptions.IgnoreCase);
        if (rx.IsMatch(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath))) Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='/_content/_js/docs-detail-07-apr-2020.js'></script>")); 

        //add styles
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/dropzone.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/bootstrap.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/animate.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/select2.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/select2-bootstrap.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/sweetalert.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/spinner.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/iCheck/custom.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_font-awesome/css/font-awesome.min.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/datepicker3.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/jquery.timepicker.css' rel='stylesheet' />"));
        Page.Header.Controls.Add(new LiteralControl("<link href='/_content/_css/bundle/style-07-apr-2020.css ' rel='stylesheet' />"));
    }


    void CheckLogin()
    {
        // checks for logged-in user session
        var fromPage = HttpContext.Current.Request.RawUrl;
        var user = LoginUser.GetLoggedInUser();
        if (user == null)
        {
            HttpContext.Current.Response.Redirect("/Login.aspx?from=" + HttpContext.Current.Server.UrlEncode(fromPage));
            return;
        }
    }
}
