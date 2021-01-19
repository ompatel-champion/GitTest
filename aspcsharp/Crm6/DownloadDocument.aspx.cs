using System;
using System.Linq;
using System.Net;
using System.Web.UI;

namespace Crm6
{
    public partial class DownloadDocument : Page
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    var strRequest = Request.QueryString["file"];
        //    var strFileName = Request.QueryString["fName"];
        //    var invalidExt = new[] { ".cs", ".aspx", ".asp", ".vb", ".ashx", ".config", ".asax", ".dll", ".dbml" };

        //    // get absolute path of the file
        //    if (!string.IsNullOrEmpty(strRequest))
        //    {
        //        var path = strRequest;
        //        var ext = System.IO.Path.GetExtension(path).ToLower();

        //        if (!string.IsNullOrEmpty(ext) && !invalidExt.Contains(ext) && !string.IsNullOrEmpty(strFileName))
        //        {
        //            // get file object as FileInfo
        //            var file = new System.IO.FileInfo(path);
        //            // if the file exists on the server
        //            if (file.Exists)
        //            {
        //                //clear the response
        //                Response.Clear();
        //                if (!string.IsNullOrEmpty(strFileName))
        //                    // rename the file
        //                    Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName.Replace(" ", "") + System.IO.Path.GetExtension(path));
        //                else
        //                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name.Replace(" ", "_").Replace(",", "_"));
        //                // buffer the file with appropriate headers
        //                Response.AddHeader("Content-Length", file.Length.ToString());
        //                Response.ContentType = "application/octet-stream";
        //                Response.WriteFile(file.FullName);
        //                Response.End();
        //            }
        //            else
        //                // if file does not exist
        //                Response.Write("<pre>This file does not exist.</pre>");
        //        }
        //    }
        //    else
        //        // nothing in the URL as HTTP GET
        //        Response.Write("<pre>Please provide a file to download.</pre>");
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            var strRequest = Request.QueryString["file"];
            var strFileName = Request.QueryString["fName"];
            var invalidExt = new[] { ".cs", ".aspx", ".asp", ".vb", ".ashx", ".config", ".asax", ".dll", ".dbml" };

            // get absolute path of the file
            if (!string.IsNullOrEmpty(strRequest))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadDataTaskAsync(new Uri(strRequest));
                }
                 
            }
            else
                // nothing in the URL as HTTP GET
                Response.Write("<pre>Please provide a file to download.</pre>");
        }
    }
}
