using System;

namespace Tipset.Models
{
    public class Standings_2022 : IPreviousYearStanding
    {
        public int UserID { get; set; }
        public DateTime UpdateDate { get; set; }
        public short TotalPoints { get; set; }
        public Guid? Guid { get; set; }
        public short? Position { get; set; }

        public virtual User_2022 User_2022 { get; set; }
    }
}