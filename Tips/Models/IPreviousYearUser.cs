using System;
using System.Collections.Generic;

namespace Tipset.Models
{
    public interface IPreviousYearStanding
    {
        int UserID { get; }
        short TotalPoints { get; }
        Nullable<short> Position { get; }
        DateTime UpdateDate { get; }
    }

    public interface IPreviousYearUser
    {
        int ID { get; }
        string DisplayName { get; }
        bool HasPaid { get; }
        IEnumerable<IPreviousYearStanding> Standings { get; }
    }
}