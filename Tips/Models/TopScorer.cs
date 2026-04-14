
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TopScorer
    {
        public TopScorer()
        {
            this.Users = new HashSet<User>();
        }
    
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> TeamID { get; set; }
        public Nullable<bool> IsWinner { get; set; }
        public Nullable<double> WinPercent { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
