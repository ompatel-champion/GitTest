<%@ WebHandler Language="C#" Class="ping" %>

using System;
using System.Web;

public class ping : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write(DateTime.Now.ToString());
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}