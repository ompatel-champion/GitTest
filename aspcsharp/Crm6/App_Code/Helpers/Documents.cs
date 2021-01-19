using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Helpers
{
    public class Documents
    {

        public bool SaveDocuments(IEnumerable<DocumentModel> documentList)
        {
            foreach (var doc in documentList)
            {
                SaveDocument(doc);
            }
            return true;
        }


        public int SaveDocument(DocumentModel doc)
        {
            try
            {
                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == doc.SubscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
                var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
              //  var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
 
                if (doc.SubscriberId > 0)
                {
                    context = new DbFirstFreightDataContext(LoginUser.GetConnection());
                }
                var document = context.Documents.FirstOrDefault(t => t.DocumentId == doc.DocumentId) ?? new Document();
                var deleteExisitngDocTypeIds = context.DocumentTypes.Where(t => t.DeleteExisting).Select(t => t.DocumentTypeId).ToList();

                // delete existing file
                if (deleteExisitngDocTypeIds.Contains(doc.DocumentTypeId))
                {
                    DeleteFilesForDocType(doc);
                    document.DocumentId = 0;
                }

                document.SubscriberId = doc.SubscriberId;
                document.DocumentType = doc.DocumentTypeId;
                document.FileName = doc.FileName;
                document.Title = doc.Title;
                document.Description = doc.Description;
                document.DealId = doc.DealId;
                document.CompanyId = doc.CompanyId;
                document.CompanyIdGlobal = doc.CompanyIdGlobal;
                document.ContactId = doc.ContactId;
                document.EmailId = doc.EmailId;
                document.UserId = doc.UserId;
                document.CreatedDate = DateTime.UtcNow;
                document.UploadedUserId = doc.UploadedBy;
                document.UploadedByName = new Users().GetUserFullNameById(doc.UploadedBy, doc.SubscriberId);
                document.CalendarEventId = doc.CalendarEventId;

                using (var wc = new WebClient())
                {
                    if (!string.IsNullOrEmpty(doc.DocumentBlobReference) && !string.IsNullOrEmpty(doc.DocumentContainerReference))
                    {
                        // upload file
                        // get the blob container to store the blob
                        var targetContainerReference = "";
                        if (document.DealId > 0)
                            targetContainerReference = "deals";
                        else if (document.ContactId > 0)
                            targetContainerReference = "contacts";
                        else if (document.CompanyId > 0)
                            targetContainerReference = "companies";
                        else if (document.EmailId > 0)
                            targetContainerReference = "emails";
                        else if (document.UserId > 0)
                            targetContainerReference = "users";
                        else if (document.CalendarEventId > 0)
                            targetContainerReference = "companies";
                        
                        CloudBlockBlob blob = new BlobStorageHelper().MoveBlob(doc.DocumentContainerReference, targetContainerReference, doc.DocumentBlobReference, true);

                        if (blob != null)
                        {
                            document.DocumentBlobReference = doc.DocumentBlobReference;
                            document.DocumentBlobContainer = targetContainerReference;
                        }
                    }
                }

                if (document.DocumentId < 1)
                {
                    context.Documents.InsertOnSubmit(document);
                }
                context.SubmitChanges();

                // Log Event 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = doc.UploadedBy,
                    CompanyId = doc.CompanyId,
                    ContactId = doc.ContactId,
                    DealId = doc.DealId,
                    UserActivityMessage = "Saved Document: " + document.FileName
                });

                //DONE: intercom Journey Step event
                var eventName = "Saved document";
                var intercomHelper = new IntercomHelper();
                intercomHelper.IntercomTrackEvent(doc.UserId, doc.SubscriberId, eventName);

                return document.DocumentId;
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveDocument",
                    PageCalledFrom = "SaveDocument",
                    SubscriberId = doc.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = doc.UserId
                };
                new Logging().LogWebAppError(error);
                // ignore
                // TODO: alter path column size
            }
            return -1;
        }


        public void DeleteFilesForDocType(DocumentModel doc)
        {
            var docPath = ConfigurationManager.AppSettings["DocPath"];
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == doc.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
           // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            try
            {
                var refId = 0;
                var documenttype = (DocumentTypeEnum)doc.DocumentTypeId;
                switch (documenttype)
                {
                    case DocumentTypeEnum.UserProfilePic:
                        refId = doc.UserId;
                        break;
                    case DocumentTypeEnum.ContactProfilePic:
                        refId = doc.ContactId;
                        break;
                    case DocumentTypeEnum.CompanyLogo:
                        refId = doc.CompanyId;
                        break;
                    case DocumentTypeEnum.DealDocuments:
                        refId = doc.DealId;
                        break;
                    case DocumentTypeEnum.ContactDocuments:
                        refId = doc.ContactId;
                        break;
                    case DocumentTypeEnum.CompanyDocuments:
                        refId = doc.CompanyId;
                        break;
                    case DocumentTypeEnum.CalendarEvents:
                        refId = doc.CalendarEventId;
                        break;
                    case DocumentTypeEnum.LocationPic:
                        break;
                    default:
                        break;
                }

                if (refId > 0)
                {
                    //  delete entries from DB
                    var documents = GetDocumentsByDocType(doc.DocumentTypeId, refId);
                    if (documents.Count > 0)
                    {
                        // delete files  
                        foreach (var docItem in documents)
                        {
                            DeleteDocument(docItem.DocumentId, doc.UploadedBy);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Ignore
            }
        }


        public bool DeleteDocument(int documentId, int userId, int subscriberId = 0)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
           // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            if (subscriberId > 0)
            {
                context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            }
            // get document for the filters
            var document = (from t in context.Documents where t.DocumentId == documentId select t).FirstOrDefault();
            if (document == null) return false;
            try
            {
                // delete document from blob storage#
                if (new BlobStorageHelper().DeleteBlob(document.DocumentBlobContainer, document.DocumentBlobReference))
                {  // delete document
                    document.Deleted = true;
                    document.DeletedUserId = userId;
                    document.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                    context.SubmitChanges();

                    new Logging().LogUserAction(new UserActivity
                    {
                        UserId = userId,
                        CompanyId = document.CompanyId,
                        ContactId = document.ContactId,
                        DealId = document.DealId,
                        UserActivityMessage = "Deleted Document: " + document.FileName
                    });

                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }


        public List<DocumentModel> GetDocumentsByDocType(int docType, int refId, int subscriberId = 0)
        {
            if (subscriberId == 0)
            {
                return new List<DocumentModel>();
            }

            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                          .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                          .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection); 

            // get documents for the filters 
            var documents = (from t in context.Documents
                             join j in context.DocumentTypes on t.DocumentType equals j.DocumentTypeId
                             where docType == t.DocumentType && !t.Deleted
                             orderby t.CreatedDate descending
                             select new DocumentModel
                             {
                                 DocumentId = t.DocumentId,
                                 //  DocumentTypeId = t.DocumentTypeId,
                                 FileName = t.FileName,
                                 DealId = t.DealId,
                                 ContactId = t.ContactId,
                                 CompanyId = t.CompanyId,
                                 UserId = t.UserId,
                                 EmailId = t.EmailId,
                                 Title = t.Title,
                                 CalendarEventId = t.CalendarEventId,
                                 Description = t.Description,
                                 DocType = j.DocumentTypeText,
                                 DocumentBlobReference = t.DocumentBlobReference,
                                 DocumentContainerReference = t.DocumentBlobContainer,
                                 UploadedDate = t.CreatedDate,
                                 DocTypeText = j.DocumentTypeText,
                                 UploadedBy = t.UploadedUserId,
                                 UploadedByName = t.UploadedByName
                             });

            var documenttype = (DocumentTypeEnum)docType;
            switch (documenttype)
            {
                case DocumentTypeEnum.UserProfilePic:
                    documents = documents.Where(t => t.UserId == refId);
                    break;
                case DocumentTypeEnum.ContactProfilePic:
                    documents = documents.Where(t => t.ContactId == refId);
                    break;
                case DocumentTypeEnum.CompanyLogo:
                    documents = documents.Where(t => t.CompanyId == refId);
                    break;
                case DocumentTypeEnum.DealDocuments:
                    documents = documents.Where(t => t.DealId == refId);
                    break;
                case DocumentTypeEnum.ContactDocuments:
                    documents = documents.Where(t => t.ContactId == refId);
                    break;
                case DocumentTypeEnum.CompanyDocuments:
                    documents = documents.Where(t => t.CompanyId == refId);
                    break;
                case DocumentTypeEnum.CalendarEvents:
                    documents = documents.Where(t => t.CalendarEventId == refId);
                    break;
                case DocumentTypeEnum.LocationPic:
                    break;
                default:
                    break;
            }


            var finalDocList = new List<DocumentModel>();
            foreach (var doc in documents)
            {
                // get the blob URI
                var blobUri = new BlobStorageHelper().GetBlob(doc.DocumentContainerReference, doc.DocumentBlobReference);
                if (string.IsNullOrEmpty(blobUri))
                {
                    continue;
                }
                else
                {
                    doc.DocumentUrl = blobUri;
                    finalDocList.Add(doc);
                }
            }
            return finalDocList;
        }


        public List<DocumentModel> GetCompanyDocuments(int globalCompanyId, int subscriberId)
        {
            var docList = new List<DocumentModel>();
            var sharedConnection = LoginUser.GetSharedConnection( );
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
            // get global company
            var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == globalCompanyId);
            if (globalCompany != null)
            {
                // get global company linked subscribers
                var linkedSubscriberIds = new Subscribers().GetLinkedSubscriberIds(globalCompany.SubscriberId);

                // iterate through the linked subscriber id and get the notes filtered by global company id
                foreach (var sid in linkedSubscriberIds)
                {
                    var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == sid)
                                                           .Select(t => t.DataCenter).FirstOrDefault();
                    var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

                    var context = new DbFirstFreightDataContext(connection);

                    var documents = (from t in context.Documents
                                     join j in context.DocumentTypes on t.DocumentType equals j.DocumentTypeId
                                     where t.DocumentType == 6 && t.CompanyIdGlobal == globalCompany.GlobalCompanyId &&
                                            !t.Deleted && t.SubscriberId == sid
                                     orderby t.CreatedDate descending
                                     select new DocumentModel
                                     {
                                         DocumentId = t.DocumentId,
                                         SubscriberId = t.SubscriberId,
                                         //  DocumentTypeId = t.DocumentTypeId,
                                         FileName = t.FileName,
                                         DealId = t.DealId,
                                         ContactId = t.ContactId,
                                         CompanyId = t.CompanyId,
                                         UserId = t.UserId,
                                         EmailId = t.EmailId,
                                         Title = t.Title,
                                         Description = t.Description,
                                         DocType = j.DocumentTypeText,
                                         DocumentBlobReference = t.DocumentBlobReference,
                                         DocumentContainerReference = t.DocumentBlobContainer,
                                         UploadedDate = t.CreatedDate,
                                         DocTypeText = j.DocumentTypeText,
                                         UploadedBy = t.UploadedUserId,
                                         UploadedByName = t.UploadedByName
                                     });


                    docList.AddRange(documents);
                }
            }

            docList = docList.Distinct().ToList();

            var response = new List<DocumentModel>();
            foreach (var doc in docList)
            {
                // get the blob URI
                var blobUri = new BlobStorageHelper().GetBlob(doc.DocumentContainerReference, doc.DocumentBlobReference);
                if (string.IsNullOrEmpty(blobUri))
                {
                    continue;
                }
                else
                {
                    doc.DocumentUrl = blobUri;
                    response.Add(doc);
                }
            }
            return response;
        }



        /// <summary>
        /// this function takes the connection from the subscriber id passed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public DocumentModel GetDocumentById(int id, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            var context = new DbFirstFreightDataContext(connection);

            // get documents for the filters 
            var document = (from t in context.Documents
                            join j in context.DocumentTypes on t.DocumentType equals j.DocumentTypeId
                            where t.DocumentId == id
                            select new DocumentModel
                            {
                                DocumentId = t.DocumentId,
                                DocumentTypeId = t.DocumentType,
                                FileName = t.FileName,
                                DealId = t.DealId,
                                ContactId = t.ContactId,
                                CompanyId = t.CompanyId,
                                CalendarEventId = t.CalendarEventId,
                                UserId = t.UserId,
                                EmailId = t.EmailId,
                                Title = t.Title,
                                Description = t.Description,
                                DocType = j.DocumentTypeText,
                                DocumentBlobReference = t.DocumentBlobReference,
                                DocumentContainerReference = t.DocumentBlobContainer,
                                UploadedDate = t.CreatedDate,
                                DocTypeText = j.DocumentTypeText,
                                UploadedBy = t.UploadedUserId,
                                UploadedByName = t.UploadedByName
                            }).FirstOrDefault();

            // get the blob URI
            var blobUri = new BlobStorageHelper().GetBlob(document.DocumentContainerReference, document.DocumentBlobReference);
            if (!string.IsNullOrEmpty(blobUri))
            {
                document.DocumentUrl = blobUri;
                return document;
            }
            return null;
        }

    }
}
