using System;
namespace Tipset.Models
{
    interface IUserRepository
    {
        void Add(User user);
        void Delete(User user);
        System.Linq.IQueryable<User> GetAllActiveUsers();
        System.Linq.IQueryable<User> GetAllConfirmedUsers();
        System.Linq.IQueryable<User> GetAllUsers();
        System.Linq.IQueryable<Standing> GetStandings();
        System.Linq.IQueryable<Standing> GetStandings(Guid guid);
        User GetUser(int id);
        User GetUser(Guid guid);
        void Save();
    }
}
