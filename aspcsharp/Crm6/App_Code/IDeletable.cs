using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm6.App_Code
{
    public interface IDeletable : ISubscribable
    {
        bool Deleted { get; set; }
        int? DeletedUserId { get; set; }
        DateTime? DeletedDate { get; set; }
        string DeletedUserName { get; set; }
    }
}
