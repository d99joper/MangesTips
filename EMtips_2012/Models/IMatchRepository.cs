using System;
namespace VMTips_2022.Models
{
    interface IMatchRepository
    {
        System.Linq.IQueryable<Match> GetAllMatches();
        Match GetMatch(int id);
        void Save();
    }
}
