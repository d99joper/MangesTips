
namespace Tipset.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Standings_2021
    {
        public int UserID { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public short TotalPoints { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<short> Position { get; set; }
    
        public virtual User_2021 User_2021 { get; set; }
    }
}
