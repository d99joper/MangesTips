using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace VMTips_2022.Helpers
{
    /// <summary>
    /// Summary description for WebServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TopScorer : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod]
        public string[] GetTopScorers(string prefixText, int count, string contextKey)
        {
            Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();

            IQueryable<Models.TopScorer> topScorers = topScorerRepository.GetAllScorers();

            // Get all the words in the search
            string[] searchWords = prefixText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // For each searchword, filter the result list 
            foreach (string searchWord in searchWords)
            {
                topScorers = topScorers.Where(t => t.FirstName.ToLower().StartsWith(searchWord.ToLower()) ||
                                                    t.LastName.ToLower().StartsWith(searchWord.ToLower()));
            }

            return topScorers.Select(p => p.DisplayName).ToArray();
        }
    }

}
