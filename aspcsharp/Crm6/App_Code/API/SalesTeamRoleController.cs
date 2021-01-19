using Crm6.App_Code.Shared;
using Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class SalesTeamRoleController : ApiController
    {

        [AcceptVerbs("GET")]
        public bool UpdateSalesTeam()
        {
            // TODO: ???? is this used ????
            return new Admin().UpdateSalesTeam();
        }


        [AcceptVerbs("GET")]
        public bool DeleteSalesTeamRole([FromUri]int id, int userId, int subscriberId)
        {
            return new SalesTeamRoles().DeleteSalesTeamRole(id, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SalesTeamRole> GetSalesTeamRoles([FromUri]int subscriberId)
        {
            return new SalesTeamRoles().GetSalesTeamRoles(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveSalesTeamRole([FromBody]SalesTeamRole salesTeamRole)
        {
            return new SalesTeamRoles().SaveSalesTeamRole(salesTeamRole);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new SalesTeamRoles().ChangeOrder(ids, subscriberId);
        }

    }
}
