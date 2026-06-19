// Tipset/Helpers/DropdownHelper.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using Tipset.Models;

namespace Tipset.Helpers
{
    public static class DropdownHelper
    {
        public static IEnumerable<SelectListItem> ToTeamList(
            IEnumerable<Team> src, int selectedId, string label)
        {
            var items = src.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = t.TeamName,
                Selected = t.ID == selectedId
            }).ToList();

            items.Insert(0, new SelectListItem { Value = "-1", Text = label, Selected = selectedId <= 0 });
            return items;
        }
    }
}