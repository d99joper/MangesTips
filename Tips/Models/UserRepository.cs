using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace Tipset.Models
{
    public class UserRepository : IUserRepository
    {
        private Tips_Entities db;

        public UserRepository(Tips_Entities db)
        {
            this.db = db;
        }

        public IQueryable<User> GetAllUsers()
        {
            return db.Users.AsNoTracking();    
        }

        public IQueryable<User> GetAllConfirmedUsers()
        {
            return GetAllUsers().Where(u => u.IsConfirmed);
        }

        

        public IQueryable<User> GetAllActiveUsers()
        {
            return GetAllUsers().Where(u => u.HasPaid);
        }

        // Eager-loads all collections needed by UpdateUsers to avoid N+1 queries.
        public List<User> GetAllActiveUsersWithDetails()
        {
            return db.Users
                .Where(u => u.HasPaid)
                .Include(u => u.UserMatches)
                .Include(u => u.UserPlayoffTeams).ThenInclude(t => t.Team)
                .Include(u => u.BonusPoints)
                .Include(u => u.UserQFTeams)
                .Include(u => u.UserSFTeams)
                .Include(u => u.UserFinalTeams)
                .Include(u => u.UserBronzeTeam)
                .Include(u => u.UserSilverTeam)
                .Include(u => u.UserGoldTeam)
                .Include(u => u.Standings)
                .AsSplitQuery()
                .ToList();
        }

        public IQueryable<Standing> GetStandings()
        {
            var maxDate = db.Standings.Max(s => s.UpdateDate);

            return from s in db.Standings
                   where s.UpdateDate == maxDate && s.User.HasPaid
                   orderby s.TotalPoints descending
                   select s;
        }

        public IQueryable<Standing> GetStandings(Guid guid)
        {
            return db.Standings
                .Include(s => s.User)
                .Where(s => s.Guid == guid && s.User.HasPaid)
                .OrderByDescending(s => s.TotalPoints);
        }

        public User GetUser(int id)
        {
            return db.Users
                .Include(u => u.Standings)
                .Include(u => u.UserMatches).ThenInclude(um => um.Match).ThenInclude(m => m.HomeTeam)
                .Include(u => u.UserMatches).ThenInclude(um => um.Match).ThenInclude(m => m.AwayTeam)
                .Include(u => u.UserPlayoffTeams).ThenInclude(t => t.Team)
                .Include(u => u.UserQFTeams).ThenInclude(t => t.Team)
                .Include(u => u.UserSFTeams).ThenInclude(t => t.Team)
                .Include(u => u.UserFinalTeams).ThenInclude(t => t.Team)
                .Include(u => u.UserBronzeTeam).ThenInclude(t => t.Team)
                .Include(u => u.UserSilverTeam).ThenInclude(t => t.Team)
                .Include(u => u.UserGoldTeam).ThenInclude(t => t.Team)
                .Include(u => u.TopScorer)
                .AsSplitQuery()
                .SingleOrDefault(u => u.ID == id);
        }

        public User GetUser(Guid guid)
        {
            return db.Users.SingleOrDefault(u => u.Guid == guid);
        }

        public void Add(User user)
        {
            db.Users.Add(user);
            //db.UserMatches.InsertAllOnSubmit(user.UserMatches);
        }

        public void Delete(User user)
        {
            db.Users.Remove(user);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        internal void SortStandings(DateTime dtUpdate)
        {
            // Get all standings for this date. 
            List<Standing> standings = db.Standings.Where(s => s.UpdateDate == dtUpdate).OrderByDescending(s => s.TotalPoints).ToList();

            // Loop through and set the position
            for (Int16 i = 1; i <= standings.Count; i++)
            {
                // Check if it has the same points as last player
                if (i > 1)
                {
                    if (standings[i - 1].TotalPoints == standings[i - 2].TotalPoints)
                        standings[i - 1].Position = standings[i - 2].Position;
                    else
                        standings[i - 1].Position = i;
                }
                else
                    standings[i - 1].Position = i;
            }

            Save();
        }

        internal List<StandingDate> GetStandingDates()
        {
            return db.Standings
                .Where(s => s.Guid.HasValue)
                .Select(s => new StandingDate { Guid = s.Guid.Value, UpdateDate = s.UpdateDate })
                .Distinct()
                .OrderBy(s => s.UpdateDate)
                .ToList();
        }

        public void ResetAllBonusPoints()
        {
            db.Database.ExecuteSqlRaw("UPDATE BonusPoints_2026 SET Point = 0, HalfPoint = 0");
        }

        internal int CountUserPlayOffTeams(int intFilterTeamID)
        {
            return db.UserPlayoffTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count(); 
        }

        internal int CountUserQuarterFinalTeams(int intFilterTeamID)
        {
            return db.UserQFTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count();
        }

        internal int CountUserSemiFinalTeams(int intFilterTeamID)
        {
            return db.UserSFTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count();
        }

        internal int CountUserFinalsTeams(int intFilterTeamID)
        {
            return db.UserFinalTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User .HasPaid).Count();
        }

        internal int CountUserBronzeTeams(int intFilterTeamID)
        {
            return db.UserBronzeTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count();
        }

        internal int CountUserSilverTeams(int intFilterTeamID)
        {
            return db.UserSilverTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count();
        }

        internal int CountUserGoldTeams(int intFilterTeamID)
        {
            return db.UserGoldTeam.Where(ut => ut.TeamID == intFilterTeamID && ut.User.HasPaid).Count();
        }

        internal IPreviousYearUser GetPreviousYearUser(string year, string displayName)
        {
            switch (year)
            {
                case "2010": return db.User_2010.Include(u => u.Standings_2010).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2012": return db.User_2012.Include(u => u.Standings_2012).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2014": return db.User_2014.Include(u => u.Standings_2014).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2016": return db.User_2016.Include(u => u.Standings_2016).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2018": return db.User_2018.Include(u => u.Standings_2018).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2021": return db.User_2021.Include(u => u.Standings_2021).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2022": return db.User_2022.Include(u => u.Standings_2022).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                case "2024": return db.User_2024.Include(u => u.Standings_2024).FirstOrDefault(u => u.DisplayName == displayName && u.HasPaid);
                default:     return null;
            }
        }

        internal double CountUserMatchResult(int matchID, String resultMark)
        {
            return db.UserMatches.Where(um => um.MatchID == matchID && um.ResultMark == resultMark && um.User.HasPaid).Count();
        }

        // Returns counts keyed by "matchId_resultMark" for all paid users
        internal Dictionary<string, double> GetMatchResultCounts()
        {
            return db.UserMatches
                .Where(um => um.User.HasPaid)
                .GroupBy(um => new { um.MatchID, um.ResultMark })
                .Select(g => new { g.Key.MatchID, g.Key.ResultMark, Count = (double)g.Count() })
                .ToList()
                .ToDictionary(x => x.MatchID + "_" + x.ResultMark, x => x.Count);
        }

        internal Dictionary<int, int> GetPlayoffTeamCounts()
        {
            return db.UserPlayoffTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetQFTeamCounts()
        {
            return db.UserQFTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetSFTeamCounts()
        {
            return db.UserSFTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetFinalTeamCounts()
        {
            return db.UserFinalTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetBronzeTeamCounts()
        {
            return db.UserBronzeTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetSilverTeamCounts()
        {
            return db.UserSilverTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal Dictionary<int, int> GetGoldTeamCounts()
        {
            return db.UserGoldTeam.Where(ut => ut.User.HasPaid)
                .GroupBy(ut => ut.TeamID).Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);
        }

        internal List<UserMatch> GetAllUserMatches(int matchID, string resultMark)
        {
            return db.UserMatches
                .Include(um => um.User)
                .Include(um => um.Match)
                .Where(um => um.MatchID == matchID && um.ResultMark == resultMark && um.User.HasPaid)
                .OrderBy(um => um.User.DisplayName)
                .ToList();
        }

        internal List<User> GetUserPlayoffTeams(string stage, int teamid)
        {
                switch (stage)
                {
                    case "playoff":
                        return db.UserPlayoffTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "quarterfinals":
                        return db.UserQFTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "semifinals":
                        return db.UserSFTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "final":
                        return db.UserFinalTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "silver":
                        return db.UserSilverTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "bronze":
                        return db.UserBronzeTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    case "gold":
                        return db.UserGoldTeam
                            .Where(ut => ut.TeamID == teamid && ut.User.HasPaid)
                            .Select(ut => ut.User)
                            .OrderBy(u => u.DisplayName)
                            .ToList();
                    default:
                        return null;
                }
        }

        internal List<User> GetUsersForTopscorer(int topscorerID)
        {
            return db.Users
                .Where(u => u.TopScorerID == topscorerID && u.HasPaid)
                .OrderBy(u => u.DisplayName)
                .ToList();
        }

        internal bool CorrectTeamInStage(int teamID, string stage)
        {
                switch (stage)
                {
                    case "playoff":
                        return db.Teams.Where(t => t.IsInPlayOffs && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    case "semifinals":
                        return db.Teams.Where(t => t.IsInSemiFinals && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    case "final":
                        return db.Teams.Where(t => t.IsInFinal && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    case "bronze":
                        return db.Teams.Where(t => t.WonBronze && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    case "silver":
                        return db.Teams.Where(t => t.WonSilver && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    case "gold":
                        return db.Teams.Where(t => t.WonGold && t.ID == teamID).SingleOrDefault() == null ? false : true;
                    default:
                        return false;
                }
        }
    }
}


