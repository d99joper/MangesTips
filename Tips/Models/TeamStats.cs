
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TeamStats
    {
        public int TeamID { get; set; }
        public Nullable<double> PlayoffPercent { get; set; }
        public Nullable<double> QuarterFinalPercent { get; set; }
        public Nullable<double> SemiFinalPercent { get; set; }
        public Nullable<double> FinalPercent { get; set; }
        public Nullable<double> BronzePercent { get; set; }
        public Nullable<double> SilverPercent { get; set; }
        public Nullable<double> GoldPercent { get; set; }
        public Nullable<byte> ScoredGoals { get; set; }
        public Nullable<byte> ConcededGoals { get; set; }
    
        public virtual Team Team { get; set; }
    }
}
