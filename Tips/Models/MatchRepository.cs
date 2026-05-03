using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tipset.Models
{
    public class MatchRepository : IMatchRepository
    {
        private Tips_Entities db = new Tips_Entities();

        public IQueryable<Match> GetAllMatches()
        {
            return db.Matches;
        }

        public Match GetMatch(int id)
        {
            return db.Matches.SingleOrDefault(m => m.ID == id);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        internal void Delete(Match match)
        {
            db.Matches.Remove(match);
        }

        internal void Add(Match match)
        {
            db.Matches.Add(match);
        }
    }
}
