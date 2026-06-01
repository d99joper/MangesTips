using System;
using System.Collections.Generic;

namespace Tipset.Models
{
    public class User_2022 : IPreviousYearUser
    {
        public User_2022()
        {
            this.Standings_2022 = new HashSet<Standings_2022>();
        }

        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool HasPaid { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public Guid Guid { get; set; }
        public string PayCode { get; set; }
        public int? TopScorerID { get; set; }
        public bool IsWinner { get; set; }

        public virtual ICollection<Standings_2022> Standings_2022 { get; set; }
        public IEnumerable<IPreviousYearStanding> Standings => Standings_2022;
    }
}