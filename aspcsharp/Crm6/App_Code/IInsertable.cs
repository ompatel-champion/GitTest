using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm6.App_Code
{
    public interface IInsertable : IDeletable, IUpdatable
    {
        int CreatedUserId { get; set; }
        string CreatedUserName { get; set; }

        DateTime? InterimCreatedDate { get; set; }
    }
}
