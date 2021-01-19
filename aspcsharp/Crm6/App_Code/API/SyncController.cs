using System.Threading.Tasks;
using System.Web.Http;
using Helpers.Sync;

namespace API
{
    public class SyncController : ApiController
    {

        [AcceptVerbs("GET")]
        public async Task<bool> DoSync([FromUri]int userId, int subscriberId)
        {
            return await new Helpers.Sync.SyncInitializer().SyncExchangeForUser(userId, subscriberId); 
        }

        [AcceptVerbs("POST")]
        public SyncHistoryResponse GetSyncHistory([FromBody]SyncHistoryRequest request) {
            return new Helpers.Sync.SyncInitializer().GetSyncHistory(request);
        }

        [AcceptVerbs("POST")]
        public SyncErrorItemsResponse GetSyncErrors([FromBody]SyncErrorItemsRequest request)
        {
            return new Helpers.Sync.SyncInitializer().GetSyncErrors(request);
        }

    }

}
