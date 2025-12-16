using System;
namespace Tips.Models
{
    interface IMatchRepository
    {
        System.Linq.IQueryable<Match> GetAllMatches();
        Match GetMatch(int id);
        void Save();
    }
}
