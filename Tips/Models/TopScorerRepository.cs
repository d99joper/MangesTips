

namespace Tipset.Models
{
    public class TopScorerRepository
    {
        private Tips_Entities db;

        public TopScorerRepository(Tips_Entities db)
        {
            this.db = db;
        }

        public IQueryable<TopScorer> GetAllScorers()
        {
            return db.TopScorers;
        }

        public TopScorer GetTopScorer(int id)
        {
            return db.TopScorers.SingleOrDefault(t => t.ID == id);
        }

        public TopScorer GetTopScorer(string strDisplayName)
        {
            return db.TopScorers.SingleOrDefault(t => t.DisplayName.ToLower() == strDisplayName.ToLower());
        }

        public List<TopScorer> GetWinner()
        {
            return db.TopScorers.Where(t => t.IsWinner == true).ToList<TopScorer>();
        }

        public void ResetWinner()
        {
            List<TopScorer> ts = GetWinner();
            foreach (TopScorer t in ts)
            {
                if (t != null)
                    t.IsWinner = false;
            }
        }

        public void Add(TopScorer ts)
        {
            db.TopScorers.Add(ts);
        }

        public void Delete(TopScorer ts)
        {
            db.TopScorers.Remove(ts);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
