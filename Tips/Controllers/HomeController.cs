using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class HomeController : BaseController
    {
        private readonly UserRepository _userRepository;

        public HomeController(UserRepository userRepository, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _userRepository = userRepository;
        }

        public ActionResult Index(Guid? date, string sort, string dir)
        {
            var vm = new HomeViewModel();

            vm.EnableNewEntries = ViewBag.EnableNewEntries;

            vm.StandingDates = _userRepository.GetStandingDates();

            if (!date.HasValue && vm.StandingDates.Count > 0)
                date = vm.StandingDates.Last().Guid;

            vm.SelectedDate = date;
            vm.SortColumn = sort ?? "Position";
            vm.SortDir = dir ?? "asc";

            if (date.HasValue)
            {
                IQueryable<Standing> standings = _userRepository.GetStandings(date.Value);
                bool asc = vm.SortDir == "asc";

                switch (vm.SortColumn)
                {
                    case "FirstName":
                        standings = asc ? standings.OrderBy(s => s.User.FirstName) : standings.OrderByDescending(s => s.User.FirstName);
                        break;
                    case "LastName":
                        standings = asc ? standings.OrderBy(s => s.User.LastName) : standings.OrderByDescending(s => s.User.LastName);
                        break;
                    case "TotalPoints":
                        standings = asc ? standings.OrderBy(s => s.TotalPoints) : standings.OrderByDescending(s => s.TotalPoints);
                        break;
                    default:
                        standings = asc ? standings.OrderBy(s => s.Position) : standings.OrderByDescending(s => s.Position);
                        break;
                }

                vm.Standings = standings.ToList();
            }

            return View(vm);
        }
    }
}
