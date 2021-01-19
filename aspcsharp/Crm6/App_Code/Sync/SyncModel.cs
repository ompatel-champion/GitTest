
using Crm6.App_Code;
using System;
using System.Collections.Generic;

namespace Helpers.Sync
{
    public class GoogleSyncTimings
    {
        public static DateTime? LastGoogleSync = null;
        public static readonly int GoogleSyncIntervalMinutes = 1;
    }

    public class SyncUser
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public string SyncUsername{ get; set; }
        public string SyncEmail { get; set; }
        public string SyncPassword { get; set; }
        public string SyncType { get; set; } 
        public string Connection { get; set; }
        public string SyncState { get; set; }
    }

    public enum ServerVersion
    {
        Ex2007Sp1,
        Ex2010,
        Ex2010Sp1,
        Ex2013Sp1
        // 2016?
    }

    public class SyncHistoryRequest
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public int CurrentPage { get; set; }
        public int RecordsPerPage { get; set; }
    }

    public class SyncHistoryResponse
    {
        public int RecordsCount { get; set; }
        public List<ExchangeSyncLog> Items { get; set; }
    }


    public class SyncErrorItemsRequest
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public int CurrentPage { get; set; }
        public int RecordsPerPage { get; set; }
    }

    public class SyncErrorItemsResponse
    {
        public int RecordsCount { get; set; }
        public List<ExchangeSyncErrorLog> Items { get; set; }
    }
}