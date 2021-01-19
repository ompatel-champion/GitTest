using System;
using System.Web;

namespace Models
{
    public class DocumentModel
    {
        public int SubscriberId { get; set; }
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string DocumentUrl { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocType { get; set; }
        public string DocTypeText { get; set; }
        public int DealId { get; set; }
        public int ContactId { get; set; }
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public int CalendarEventId { get; set; }
        public int EmailId { get; set; }
        public string DocumentBlobReference { get; set; }
        public string DocumentContainerReference { get; set; }
        public DateTime UploadedDate { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; }
    }


    public class DocumentType
    {
        public int DocTypeValue { get; set; }
        public string DocType { get; set; }
        public string DocTypeText { get; set; }
        public string PrimaryType { get; set; }
    }


    public enum DocumentTypeEnum
    {
        UserProfilePic = 1,
        ContactProfilePic = 2,
        CompanyLogo = 3,
        DealDocuments = 4,
        ContactDocuments = 5,
        CompanyDocuments = 6,
        LocationPic = 7,
        CalendarEvents = 8
    }


    public class DocumentFilter
    {
        public int RefId { get; set; }
        public int DocTypeId { get; set; }
        public int SubscriberId { get; set; }
    }

}