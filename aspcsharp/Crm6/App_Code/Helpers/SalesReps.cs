using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Models;

namespace Helpers
{
    public class SalesReps
    {

        public IQueryable<Crm6.App_Code.Login.GlobalUser> GetSalesReps(int subscriberId)
        {
            var loginConnection = LoginUser.GetLoginConnection();
            var db = new DbLoginDataContext(loginConnection);
            return db.GlobalUsers;
        }

        public List<SalesTeamRole> GetSalesTeamRoles(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(sharedConnection);

            return (from i in context.SalesTeamRoles
                    where !i.Deleted
                    select i).OrderBy(t => t.SortOrder).ToList();
        }

        /// <summary>
        /// loads the sales reps for the passed subscriber id  for the dropdown
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public List<SelectList> PopulateSalesRepDropdown(int subscriberId)
        {
            //var context = new dbDataContext();
            //return context.tblSalesReps.Where(obj => !obj.Deleted && obj.SubscriberID == subscriberId && obj.Active)
            //    .OrderBy(obj => obj.SalesRepName).Select(l => new SelectList
            //    {
            //        SelectText = l.SalesRepName,
            //        SelectValue = l.SalesRepCode
            //    }).ToList();
            return new List<SelectList>();
        }


        /// <summary>
        /// gets the user id from sales rep code
        /// </summary>
        /// <param name="salesRepCode"></param>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public int GetUserIdFromSalesRepCode(string salesRepCode, int subscriberId)
        {
            //var context = new DbFirstFreightDataContext();
            //var userId = context.Sale.Where(sr => sr.SubscriberID == subscriberId && sr.SalesRepCode.Equals(salesRepCode) && !sr.Deleted)
            //                                 .Select(sr => sr.UserID).FirstOrDefault();
            //return userId;
            return 0;
        }


        public string GetSalesRepNameFromSalesRepCode(string salesRepCode, int? subscriberId)
        {
            //var context = new DbFirstFreightDataContext();
            //var salesRepName = context.tblSalesReps.Where(sr => sr.SubscriberID == subscriberId && sr.SalesRepCode.Equals(salesRepCode) && !sr.Deleted)
            //                                 .Select(sr => sr.SalesRepName).FirstOrDefault();
            //return salesRepName;
            return "";
        }

    }
}