using System;
namespace Tipset.Models
{
    interface ITeamRepository
    {
        System.Linq.IQueryable<Team> GetAllTeams();
        System.Linq.IQueryable<Team> GetTeams(string filter);
        Team GetTeam(int id);
        void Save();
    }
}
