using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;

namespace Tipset.Controllers
{
    /// <summary>
    /// Replaces the legacy TopScorer.asmx and TopScorers.ashx web services.
    /// Provides autocomplete data for top scorer name lookups.
    /// </summary>
    [Route("api/topscorers")]
    public class TopScorersApiController : ControllerBase
    {
        private readonly TopScorerRepository _repository;

        public TopScorersApiController(TopScorerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("autocomplete")]
        public IActionResult Autocomplete(string prefixText, int count = 10)
        {
            if (string.IsNullOrWhiteSpace(prefixText))
                return Ok(Array.Empty<string>());

            IQueryable<TopScorer> topScorers = _repository.GetAllScorers();

            string[] searchWords = prefixText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string searchWord in searchWords)
            {
                topScorers = topScorers.Where(t =>
                    t.FirstName.ToLower().StartsWith(searchWord.ToLower()) ||
                    t.LastName.ToLower().StartsWith(searchWord.ToLower()));
            }

            var results = topScorers
                .Take(count)
                .Select(t => t.FirstName + " " + t.LastName)
                .ToArray();

            return Ok(results);
        }
    }
}
