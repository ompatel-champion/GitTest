using System.Linq;
using Models;
using System.Collections.Generic;
using Crm6.App_Code;

namespace Helpers
{

    public class ExchangeSyncLogs
    {

        public ExchangeSyncLogResponse GetExchangeSyncLog(ExchangeSyncLogFilter filter)
        {
            var response = new ExchangeSyncLogResponse
            {
                SyncLogEntries = new List<ExchangeSyncLog>()
            };

            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get exchange sync log
            var syncLogs = (from exchangeSyncLog in context.ExchangeSyncLogs
                where exchangeSyncLog.UserId == filter.UserId
                orderby exchangeSyncLog.SyncDateTime descending
                select exchangeSyncLog).ToList();

            if (filter.SubscriberId > 0)
            {
                syncLogs = syncLogs.Where(t => t.SubscriberId == filter.SubscriberId).ToList();
            }

            var recordCount = syncLogs.Count();
            var totalPages = 0;

            // apply paging
            if (filter.RecordsPerPage > 0 && filter.CurrentPage > 0)
            {
                syncLogs = syncLogs.Skip((filter.CurrentPage - 1) * filter.RecordsPerPage)
                    .Take(filter.RecordsPerPage).ToList();
                    totalPages = recordCount % filter.RecordsPerPage == 0 ?
                    (recordCount / filter.RecordsPerPage) :
                    ((recordCount / filter.RecordsPerPage) + 1);
            }

            response.SyncLogEntries =  syncLogs.ToList();
            return response;
        }

    }

}