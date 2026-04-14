
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserSilverTeam
    {
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public Nullable<byte> Points { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
    }
}
