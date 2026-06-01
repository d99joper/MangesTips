using System;
using System.Collections.Generic;

namespace Tipset.Models
{
    public class User_2024 : IPreviousYearUser
    {
        public User_2024()
        {
            this.Standings_2024 = new HashSet<Standings_2024>();
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

        public virtual ICollection<Standings_2024> Standings_2024 { get; set; }
        public IEnumerable<IPreviousYearStanding> Standings => Standings_2024;
    }
}