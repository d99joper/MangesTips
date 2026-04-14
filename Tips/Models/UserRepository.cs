using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace VMTips_2022.Models
{
    public class UserRepository : IUserRepository
    {
        private Tips_Entities db = new Tips_Entities();

        public IQueryable<User> GetAllUsers()
        {
            return db.Users;    
        }

        public IQueryable<User> GetAllConfirmedUsers()
        {
            return GetAllUsers().Where(u => u.IsConfirmed);
        }

        

        public IQueryable<User> GetAllActiveUsers()
        {
            return GetAllUsers().Where(u => u.HasPaid);
        }

        public IQueryable<Standing> GetStandings()
        { 
            DateTime maxDate = (from s in db.Standings
                                select s.UpdateDate).Max();

            return from s in db.Standings
                   where s.UpdateDate == maxDate && s.User.HasPaid
                   orderby s.TotalPoints descending
                   select s;

        }

        public IQueryable<Standing> GetStandings(Guid guid)
        {
            return from s in db.Standings
                   where s.Guid == guid && s.User.HasPaid
                   orderby s.TotalPoints descending
                   select s;
        }

        public User GetUser(int id)
        {
            return db.Users.SingleOrDefault(u => u.ID == id);
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
        }

        internal object GetStandingDates()
        {
            return (from s in db.Standings orderby s.UpdateDate descending select new { guid = s.Guid, UpdateDate = s.UpdateDate }).Distinct().OrderBy(s => s.UpdateDate).ToList();            
        }

        public void ResetAllBonusPoints()
        {
            IQueryable<BonusPoints> bps = db.BonusPoints;
            foreach (BonusPoints bp in bps)
            {
                bp.Point = 0;
                bp.HalfPoint = false;
            }

            Save();
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

        internal User_2010 GetVM2010User(string strDisplayName)
        {
            return db.User_2010.Where(u => u.DisplayName == strDisplayName && u.HasPaid).FirstOrDefault();
        }

        internal User_2012 GetEM2012User(string strDisplayName)
        {
            return db.User_2012.Where(u => u.DisplayName == strDisplayName && u.HasPaid).FirstOrDefault();
        }

        internal User_2014 GetVM2014User(string strDisplayName)
        {
            return db.User_2014.Where(u => u.DisplayName == strDisplayName && u.HasPaid).FirstOrDefault();
        }

        internal User_2016 GetEM2016User(string strDisplayName)
        {
            return db.User_2016.Where(u => u.DisplayName == strDisplayName && u.HasPaid).FirstOrDefault();
        }

        internal double CountUserMatchResult(int matchID, String resultMark)
        {
            return db.UserMatches.Where(um => um.MatchID == matchID && um.ResultMark == resultMark && um.User.HasPaid).Count();
        }

        internal static List<UserMatch> GetAllUserMatches(int matchID, string resultMark)
        {
            using (Tips_Entities db = new Tips_Entities())
            {
                return db.UserMatches
                    .Include(um => um.User)
                    .Include(um => um.Match)
                    .Where(um => um.MatchID == matchID && um.ResultMark == resultMark && um.User.HasPaid)
                    .OrderBy(um => um.User.DisplayName)
                    .ToList();
            }
        }

        internal static List<User> GetUserPlayoffTeams(string stage, int teamid)
        {
            using(Tips_Entities db = new Tips_Entities())
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
        }

        internal static List<User> GetUsersForTopscorer(int topscorerID)
        {
            using (Tips_Entities db = new Tips_Entities())
            {

                return db.Users
                    .Where(u => u.TopScorerID == topscorerID && u.HasPaid)
                    .OrderBy(u => u.DisplayName)
                    .ToList();
            }
        }

        internal static bool CorrectTeamInStage(int teamID, string stage)
        {
            using (Tips_Entities db = new Tips_Entities())
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
}
