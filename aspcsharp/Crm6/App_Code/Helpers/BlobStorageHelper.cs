using System;
using System.Web;
using Microsoft.WindowsAzure;           // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage;   // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace Crm6.App_Code.Helpers
{
    // Parse the connection string and return a reference to the storage account

    public class BlobStorageHelper
    {
        //TODO: use separate storage accounts per data center - HKG AMS USA KWE Sinotrans

        //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(LoginUser.GetStorageAccountConnection("dev"));

        
        // AMS
        // HKG
        // USA
        // Sinotrans
        // KWE-China
        // KWE-EMEA
        // KWE-SEA
        // KWE-USA

        public CloudBlobContainer GetContainer(string containerReference)
        {
            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);
            // Create the container if it doesn't already exist
            container.CreateIfNotExists();
            return container;
        }


        public string UploadFile(string containerReference, string blobReference, HttpPostedFileBase file)
        {
            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);
            // Retrieve reference to a blob name
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);
            using (var s = file.InputStream)
            {
                long streamlen = s.Length;
                s.Position = 0;
                blockBlob.UploadFromStream(s);
            }
            return blockBlob.Uri.ToString();
        }


        public CloudBlockBlob MoveBlob(string sourceContainerrReference, string targetContainerReference, string blobReference, bool deleteSourceBlob = true)
        {
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer sourceContainer = cloudBlobClient.GetContainerReference(sourceContainerrReference);
            CloudBlobContainer targetContainer = cloudBlobClient.GetContainerReference(targetContainerReference);
            CloudBlockBlob blockBlob = sourceContainer.GetBlockBlobReference(blobReference);
            var maxRetryCount = 3;
            var blobRequestOptions = new BlobRequestOptions
            {
                ServerTimeout = TimeSpan.FromSeconds(30),
                MaximumExecutionTime = TimeSpan.FromSeconds(120),
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), maxRetryCount),
            };

            if (sourceContainer.Exists() && targetContainer.Exists() && blockBlob != null)
            {
                CloudBlockBlob targetBlob = null;
                targetBlob = targetContainer.GetBlockBlobReference(blobReference);

                // Start the transfer
                try
                {
                    TransferManager.CopyAsync(blockBlob, targetBlob,
                          false /* isServiceCopy */);

                    //if (deleteSourceBlob)
                    //    blockBlob.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine("The transfer is cancelled: {0}", e.Message);
                }
                return targetBlob;
            }
            return null;
        }


        public CloudBlockBlob Move(CloudBlockBlob srcBlob, CloudBlobContainer destContainer)
        {
            CloudBlockBlob destBlob;

            if (srcBlob == null)
            {
                throw new Exception("Source blob cannot be null.");
            }

            if (!destContainer.Exists())
            {
                throw new Exception("Destination container does not exist.");
            }

            // copy source blob to destination container
            using (MemoryStream memoryStream = new MemoryStream())
            {
                srcBlob.DownloadToStream(memoryStream);
                destBlob = destContainer.GetBlockBlobReference(srcBlob.Name);
                destBlob.UploadFromStream(memoryStream);
            }
            // remove source blob after copy is done
            srcBlob.Delete();
            return destBlob;
        }


        public string GetBlob(string containerReference, string blobReference)
        {

            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);

            // Retrieve reference to a blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);

            return blockBlob.Uri.ToString();

        }


        public Stream DownloadBlobStream(string containerReference, string blobReference)
        {
            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);

            // Retrieve reference to a blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);

            Stream stream = new MemoryStream();
            blockBlob.DownloadToStream(stream);
            return stream;

            //var fileName = Guid.NewGuid().ToString() + ".xlsx";
            //Stream fileStream = new FileStream(HttpContext.Current.Server.MapPath("~/excel/" + fileName), FileMode.Create);
            //blockBlob.DownloadToStream(fileStream);
            //fileStream.Close();
            //return File.OpenRead(HttpContext.Current.Server.MapPath("~/excel/" + fileName));
        }


        public bool DeleteBlob(string containerReference, string blobReference)
        {
            // Create the blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);
            // Retrieve reference to a blob named "myblob.txt"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);
            // Delete the blob
            blockBlob.Delete();
            return true;
        }

    }
}
