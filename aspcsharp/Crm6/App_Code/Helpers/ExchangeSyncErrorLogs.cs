using System.Linq;
using Models;
using System.Collections.Generic;
using Crm6.App_Code;

namespace Helpers
{

    public class ExchangeSyncErrorLogs
    {

        public ExchangeSyncErrorLogResponse GetExchangeSyncErrorLog(ExchangeSyncErrorLogFilter filter)
        {
            var response = new ExchangeSyncErrorLogResponse
            {
                SyncErrorLogEntries = new List<ExchangeSyncErrorLog>()
            };

            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            // get exchange sync error log
            var syncErrorLogs = (from exchangeSyncErrorLog in context.ExchangeSyncErrorLogs
                where exchangeSyncErrorLog.UserId == filter.UserId
                orderby exchangeSyncErrorLog.ErrorDateTime descending
                select exchangeSyncErrorLog).ToList();

            if (filter.SubscriberId > 0)
            {
                syncErrorLogs = syncErrorLogs.Where(t => t.SubscriberId == filter.SubscriberId).ToList();
            }

            var recordCount = syncErrorLogs.Count();
            var totalPages = 0;

            // apply paging
            if (filter.RecordsPerPage > 0 && filter.CurrentPage > 0)
            {
                syncErrorLogs = syncErrorLogs.Skip((filter.CurrentPage - 1) * filter.RecordsPerPage)
                    .Take(filter.RecordsPerPage).ToList();
                    totalPages = recordCount % filter.RecordsPerPage == 0 ?
                    (recordCount / filter.RecordsPerPage) :
                    ((recordCount / filter.RecordsPerPage) + 1);
            }

            response.SyncErrorLogEntries = syncErrorLogs.ToList();
            return response;
        }

    }

}