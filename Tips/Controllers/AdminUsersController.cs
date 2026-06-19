using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Users")]
    public class AdminUsersController : BaseController
    {
        private readonly UserRepository _userRepo;

        public AdminUsersController(UserRepository userRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = new AdminUsersViewModel
            {
                Users = _userRepo.GetAllUsers().ToList()
            };

            if (TempData["UsersMessage"] is string msg)
                vm.UsersMessage = msg;

            return View("~/Views/Admin/Users.cshtml", vm);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminSaveUsersInput input)
        {
            string message;
            try
            {
                var ids = input.Users.Select(r => r.UserID).ToList();
                var userMap = _userRepo.GetAllUsers()
                                       .Where(u => ids.Contains(u.ID))
                                       .ToDictionary(u => u.ID);
                foreach (var row in input.Users)
                {
                    if (!userMap.TryGetValue(row.UserID, out var user)) continue;
                    user.HasPaid = row.HasPaid;
                    user.IsConfirmed = row.IsConfirmed;
                    user.IsWinner = row.IsWinner;
                }
                _userRepo.Save();
                message = "Användarna sparades.";
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
            }

            TempData["UsersMessage"] = message;
            return RedirectToAction("Index");
        }
    }
}