using System;
using System.Collections.Generic;

namespace Tipset.Models
{
    public class User_2021 : IPreviousYearUser
    {
        public User_2021()
        {
            this.Standings_2021 = new HashSet<Standings_2021>();
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

        public virtual ICollection<Standings_2021> Standings_2021 { get; set; }
        public IEnumerable<IPreviousYearStanding> Standings => Standings_2021;
    }
}