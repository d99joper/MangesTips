using Microsoft.AspNetCore.Mvc;
using Tipset.Models; 

namespace Tipset.Helpers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopScorersController : ControllerBase
    {
        private readonly TopScorerRepository _topScorerRepository;

        public TopScorersController(TopScorerRepository topScorerRepository)
        {
            _topScorerRepository = topScorerRepository;
        }

        [HttpGet]
        public ActionResult<List<string>> GetTopScorers([FromQuery] string? term)
        {
            try
            {
                // jQuery UI passes the typed text in the 'term' query variable
                string? prefixText = term;

                if (string.IsNullOrWhiteSpace(prefixText))
                {
                    return Ok(new List<string>());
                }

                IQueryable<Models.TopScorer> topScorers = _topScorerRepository.GetAllScorers();

                // Split search words
                string[] searchWords = prefixText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (searchWords.Length == 0)
                {
                    return Ok(new List<string>());
                }

                string strFirstName = searchWords[0];

                if (searchWords.Length == 2)
                {
                    string strLastName = searchWords[1];
                    // Simple StartsWith maps cleanly to SQL Server "LIKE 'value%'"
                    topScorers = topScorers.Where(t =>
                        t.FirstName.StartsWith(strFirstName) &&
                        t.LastName.StartsWith(strLastName));
                }
                else
                {
                    // Simple StartsWith maps cleanly to SQL Server "LIKE 'value%'"
                    topScorers = topScorers.Where(t =>
                        t.FirstName.StartsWith(strFirstName) ||
                        t.LastName.StartsWith(strFirstName));
                }

                List<string> listTopscorers = topScorers
                    .Select(ts => ts.DisplayName)
                    .ToList();

                return Ok(listTopscorers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
