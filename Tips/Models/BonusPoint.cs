
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BonusPoint
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string GroupID { get; set; }
        public byte Point { get; set; }
        public bool HalfPoint { get; set; }
    
        public virtual User User { get; set; }
    }
}
