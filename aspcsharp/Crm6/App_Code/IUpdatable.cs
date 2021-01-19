using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crm6.App_Code;

namespace Crm6
{

    public interface IUpdatable : ISubscribable
    {
        string UpdateUserName { get; set; }
        int UpdateUserId { get; set; }
        DateTime? InterimLastUpdate { get; set; }
    }

}
