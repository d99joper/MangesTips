using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class BlogController : BaseController
    {
        private readonly BlogRepository _blogRepository;
        private static readonly TimeZoneInfo _localZone = TimeZoneInfo.Local;

        public BlogController(BlogRepository blogRepository, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _blogRepository = blogRepository;
        }

        public ActionResult Index()
        {
            var entries = _blogRepository.GetAllBlogEntries().ToList();
            return View(entries);
        }

        public ActionResult Details(int id)
        {
            var entry = _blogRepository.GetBlogEntry(id);
            if (entry == null)
                return NotFound();
            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int id, string name, string control, string text)
        {
            if (!Regex.IsMatch(control ?? "", @"^[aA][bB][cC][dD][eE]$"))
                TempData["CommentError"] = "Felaktigt svar på kontrollfrågan.";
            else if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(text))
                TempData["CommentError"] = "Namn och kommentar krävs.";
            else
            {
                try
                {
                    var entry = _blogRepository.GetBlogEntry(id);
                    var c = new Comment
                    {
                        PostedBy = WebUtility.HtmlEncode(name),
                        PostedDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, _localZone).AddHours(2),
                        Text = WebUtility.HtmlEncode(text).Replace("\n", "<br />")
                    };
                    entry.Comments.Add(c);
                    _blogRepository.Save();
                }
                catch (Exception ex)
                {
                    TempData["CommentError"] = "Kunde inte spara kommentaren. " + ex.Message;
                }
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int commentId, int blogId)
        {
            var c = _blogRepository.GetComment(commentId);
            if (c != null)
            {
                _blogRepository.Delete(c);
                _blogRepository.Save();
            }
            return RedirectToAction("Details", new { id = blogId });
        }
    }
}
