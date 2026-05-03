
namespace Tipset.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Standings_2010
    {
        public int UserID { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public short TotalPoints { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Nullable<short> Position { get; set; }
    
        public virtual User_2010 User_2010 { get; set; }
    }
}
