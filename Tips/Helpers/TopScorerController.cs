using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;

namespace Tipset.Helpers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopScorerController : ControllerBase
    {
        private readonly TopScorerRepository _topScorerRepository;

        public TopScorerController(TopScorerRepository topScorerRepository)
        {
            _topScorerRepository = topScorerRepository;
        }

        [HttpGet("GetTopScorers")]
        public ActionResult<string[]> GetTopScorers([FromQuery] string prefixText, [FromQuery] int count, [FromQuery] string? contextKey = null)
        {
            try
            {
                IQueryable<TopScorer> topScorers = _topScorerRepository.GetAllScorers();

                if (string.IsNullOrWhiteSpace(prefixText))
                {
                    return Array.Empty<string>();
                }

                // Split search words
                string[] searchWords = prefixText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Filter the result list
                foreach (string searchWord in searchWords)
                {
                    // .NET Core optimization: string.Equals with OrdinalIgnoreCase avoids .ToLower() allocations
                    topScorers = topScorers.Where(t =>
                        t.FirstName.StartsWith(searchWord) ||
                        t.LastName.StartsWith(searchWord));
                }

                // Apply the count limit requested by the client
                var results = topScorers
                    .Select(p => p.DisplayName)
                    .Take(count)
                    .ToArray();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
