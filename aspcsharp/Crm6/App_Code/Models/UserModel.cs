using Crm6.App_Code;
using System.Collections.Generic;

namespace Models
{


    public class UserModel
    {
        public User User { get; set; }
        public DocumentModel ProfilePicture { get; set; }
        public Subscriber Subscriber { get; set; }
        public UserTimeZone TimeZone { get; set; }
        public string DataCenter { get; set; }
        public string DataCenterConnection { get; set; }
        public string SharedConnection { get; set; }
        public string WritableSharedConnection { get; set; }
        public string LoginConnection { get; set; }
    }

    public class UserBasic
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
    }

    public class UserSaveRequest
    {
        public User User { get; set; }
        public DocumentModel ProfilePic { get; set; }
        public List<int> ManagerUserIds { get; set; }
    }

    public class UserSaveResponse
    {
        public bool IsError { get; set; }
        public string Error { get; set; }
        public int UserId { get; set; }
        public string ActualError { get; set; }
    }


    public class UserListResponse
    {
        public List<User> Users { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }


    public class UserFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public string UserIdsIn { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
        public bool? LoginEnabled { get; set; }
        public bool? ShowAdmin { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string LocationName { get; set; }

        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }

    public class ReassignUserRequest
    {
        public int DepartingUserId { get; set; }
        public int NewUserId { get; set; }
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
    }


    public class PasswordChangeRequest
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class LoginDetailsSaveRequest
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string ScreenResolution { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
    }
}