 <%@ WebHandler Language="C#" Class="TempFileUploadHandler" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using System.Web.Script.Serialization;
using System.Text;

public class TempFileUploadHandler : IHttpHandler
{
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.Files.Count > 0)
        {
            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            var containerReference = "temp";
            var container = blobClient.GetContainerReference(containerReference);

            var tempFiles = new List<TempBlobFile>();
            foreach (string fileName in context.Request.Files)
            {
                //get the file and save
                var file = context.Request.Files[fileName];
                if (file != null)
                {
                    var blobReference = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var blockBlob = container.GetBlockBlobReference(blobReference);
                    using (var s = file.InputStream)
                    {
                        blockBlob.UploadFromStream(s);
                        tempFiles.Add(new TempBlobFile
                        {
                            FileName = Path.GetFileName(file.FileName),
                            BlobReference = blobReference,
                            ContainerReference = containerReference,
                            Uri = blockBlob.Uri.ToString()
                        });
                    }
                }
            }

            // serialize and send..
            var sbJsonResults = new StringBuilder();
            new JavaScriptSerializer().Serialize(tempFiles, sbJsonResults);
            context.Response.Clear();
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Write(sbJsonResults.ToString());
            context.Response.End();
        }else{

 var tempFiles = new List<TempBlobFile>();
tempFiles.Add(new TempBlobFile
                        { FileName = "asdasdasdad",
                            BlobReference = "zxczxc",
                            ContainerReference = "asdasd",
                            Uri = "zxczxczxc"
                        });
 var sbJsonResults = new StringBuilder();
            new JavaScriptSerializer().Serialize(tempFiles, sbJsonResults);
            context.Response.Clear();
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Write(sbJsonResults.ToString());
            context.Response.End();
}
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}

public class TempBlobFile
{
    public string FileName { get; set; }
    public string BlobReference { get; set; }
    public string ContainerReference { get; set; }
    public string Uri { get; set; }
}
