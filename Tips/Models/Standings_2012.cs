using System;

namespace Tipset.Models
{
    public class Standings_2012 : IPreviousYearStanding
    {
        public int UserID { get; set; }
        public DateTime UpdateDate { get; set; }
        public short TotalPoints { get; set; }
        public Guid? Guid { get; set; }
        public short? Position { get; set; }

        public virtual User_2012 User_2012 { get; set; }
    }
}