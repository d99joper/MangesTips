
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserMatch
    {
        public int UserID { get; set; }
        public int MatchID { get; set; }
        public Nullable<byte> HomeGoals { get; set; }
        public Nullable<byte> AwayGoals { get; set; }
        public string ResultMark { get; set; }
        public Nullable<byte> Points { get; set; }
    
        public virtual Match Match { get; set; }
        public virtual User User { get; set; }
    }
}
