using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Tipset.Helpers.Sanitization;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Blog")]
    public class AdminBlogController : BaseController
    {
        private readonly BlogRepository _blogRepo;

        public AdminBlogController(BlogRepository blogRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _blogRepo = blogRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = BuildViewModel();

            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;
            if (TempData["BlogMessage"] is string msg)
                vm.BlogMessage = msg;

            return View("~/Views/Admin/Blog.cshtml", vm);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminBlogInput input)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(input.Title))
                errors.Add("Titeln får inte vara tom.");
            if (string.IsNullOrWhiteSpace(input.Text))
                errors.Add("Texten får inte vara tom.");

            string message = null;
            if (errors.Count == 0)
            {
                try
                {
                    BlogEntry entry;
                    if (input.BlogEntryID > 0)
                        entry = _blogRepo.GetBlogEntry(input.BlogEntryID);
                    else
                    {
                        entry = new BlogEntry { PostedDate = DateTime.Now };
                        _blogRepo.Add(entry);
                    }
                    entry.Title = input.Title.Trim();
                    entry.Text  = HtmlSanitizer.Sanitize(input.Text);
                    _blogRepo.Save();

                    message = input.BlogEntryID > 0
                        ? "✅ Inlägget uppdaterades."
                        : "✅ Nytt inlägg publicerades.";
                }
                catch (Exception ex)
                {
                    errors.Add("Ett fel uppstod vid sparandet: " + ex.Message);
                }
            }

            TempData["ErrorMessage"] = errors.Count > 0 ? string.Join(" ", errors) : null;
            TempData["BlogMessage"]  = message;
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entry = _blogRepo.GetBlogEntry(id);
            if (entry != null) { _blogRepo.Delete(entry); _blogRepo.Save(); }
            return RedirectToAction("Index");
        }

        private AdminBlogViewModel BuildViewModel()
        {
            var entries = _blogRepo.GetAllBlogEntries().ToList();
            var vm = new AdminBlogViewModel
            {
                BlogEntries = entries
            };

            vm.BlogEntriesJson = JsonSerializer.Serialize(
                entries.ToDictionary(
                    b => b.ID.ToString(),
                    b => new { title = b.Title, text = b.Text }
                )
            );

            return vm;
        }
    }
}
