using System;

namespace Crm6.App_Code
{

    public partial class EventCategory : IInsertable, IDeletable
    {
        public DateTime? InterimCreatedDate
        {
            get { return CreatedDate; }
            set { CreatedDate = value ?? DateTime.MinValue; }
        }

        public DateTime? InterimLastUpdate
        {
            get { return this.LastUpdate; }
            set { this.LastUpdate = value ?? DateTime.MinValue; }
        }
    }

}

namespace Crm6.App_Code.Shared
{

    public partial class LinkCountryRegion : IInsertable, IDeletable
    {
        public int CreatedUserId
        {
            get { return CreatedUserIdGlobal; }
            set { CreatedUserIdGlobal = value; }
        }
        public int UpdateUserId
        {
            get { return UpdateUserIdGlobal; }
            set { UpdateUserIdGlobal = value; }
        }
        public int? DeletedUserId
        {
            get { return DeletedUserIdGlobal; }
            set { DeletedUserIdGlobal = value ?? -1; }
        }

        public DateTime? InterimCreatedDate
        {
            get { return CreatedDate; }
            set { CreatedDate = value; }
        }

        public DateTime? InterimLastUpdate
        {
            get { return LastUpdate; }
            set { LastUpdate = value; }
        }
    }
}