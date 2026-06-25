using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminUsersViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public string UsersMessage { get; set; }
    }
}