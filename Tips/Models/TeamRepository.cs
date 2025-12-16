using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tips.Models
{
    public class TeamRepository : Tips.Models.ITeamRepository 
    {
        public enum TeamInqueryType
        {
            isInPlayoffs = 1,
            isInQuarterFinals = 2, 
            isInSemiFinals = 3,
            isInFinals = 4,
            WonBronze = 5,
            WonSilver = 6,
            WonGold = 7
        }

        private Tips_Entities db = new Tips_Entities();

        public IQueryable<Team> GetAllTeams()
        {
            return db.Teams.OrderBy(t => t.TeamName);
        }

        public IQueryable<Team> GetTeams(string filter)
        {
            return db.Teams.Where(t => t.GroupID == filter).OrderBy(t => t.TeamName);
        }

        public Team GetTeam(int id)
        {
            return db.Teams.SingleOrDefault(t => t.ID == id);
        }

        public IQueryable<Team> GetPlayoffTeams()
        {
            return db.Teams.Where(t => t.IsInPlayOffs).OrderBy(t => t.TeamName);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void ResetAllTeams()
        {
            IQueryable allTeams = GetAllTeams();
            foreach (Team team in allTeams)
            {
                team.IsInPlayOffs = false;
                team.IsInQuarterFinals = false;
                team.IsInSemiFinals = false;
                team.IsInFinal = false;
                team.WonBronze = false;
                team.WonSilver = false;
                team.WonGold = false;
            }

            Save();
        }

        public List<Team> GetTeams(TeamInqueryType filter)
        {
            switch (filter)
            { 
                case TeamInqueryType.isInPlayoffs:
                    return db.Teams.Where(t => t.IsInPlayOffs).ToList();
                case TeamInqueryType.isInQuarterFinals:
                    return db.Teams.Where(t => t.IsInQuarterFinals).ToList();
                case TeamInqueryType.isInSemiFinals:
                    return db.Teams.Where(t => t.IsInSemiFinals).ToList();
                case TeamInqueryType.isInFinals:
                    return db.Teams.Where(t => t.IsInFinal).ToList();
                case TeamInqueryType.WonBronze:
                    return db.Teams.Where(t => t.WonBronze).ToList();
                case TeamInqueryType.WonSilver:
                    return db.Teams.Where(t => t.WonSilver).ToList();
                case TeamInqueryType.WonGold:
                    return db.Teams.Where(t => t.WonGold).ToList();
                default:
                    return null;
            }
        }

        public List<Team> GetTeams(TeamInqueryType filter, String groupFilter)
        {
            List<Team> teamList = GetTeams(filter);

            return teamList.Where(t => t.GroupID == groupFilter).ToList();
        }

        internal Team GetTeam(TeamInqueryType filter, string groupID, byte position)
        {
            Team thisTeam = null;
            switch (filter)
            {
                case TeamInqueryType.isInPlayoffs:
                    thisTeam = db.Teams.SingleOrDefault(t => t.IsInPlayOffs && t.GroupID == groupID && t.PlayOffPos == position);
                    break;
                default:
                    break;
            }

            return thisTeam;
        }

        public void Add(Team t)
        {
            db.Teams.Add(t);
        }

        public void Delete(Team t)
        {
            db.Teams.Remove(t);
        } 
    }
}
