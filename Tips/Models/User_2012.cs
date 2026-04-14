
namespace VMTips_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_2012
    {
        public User_2012()
        {
            this.Standings_2012 = new HashSet<Standings_2012>();
        }
    
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool HasPaid { get; set; }
        public System.DateTime PostedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public System.Guid Guid { get; set; }
        public string PayCode { get; set; }
        public Nullable<int> TopScorerID { get; set; }
        public bool IsWinner { get; set; }
    
        public virtual ICollection<Standings_2012> Standings_2012 { get; set; }
    }
}
