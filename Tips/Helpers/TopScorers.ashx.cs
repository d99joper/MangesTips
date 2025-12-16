using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Tips.Helpers
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TopScorers : IHttpHandler
    {
         
        public void ProcessRequest(HttpContext context)
        {
            string prefixText = context.Request.QueryString[0];
            Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();

            IQueryable<Models.TopScorer> topScorers = topScorerRepository.GetAllScorers();

            // Get all the words in the search
            string[] searchWords = prefixText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Create the retun string container
            List<string> listTopscorers = new List<string>();

            String strFirstName = searchWords[0].ToLower();
            if (searchWords.Count() == 2)
            {
                String strLastName = searchWords[1].ToLower();

                // For each searchword, filter the result list
                topScorers = topScorers.Where(t => t.FirstName.ToLower().StartsWith(strFirstName) &&
                                                   t.LastName.ToLower().StartsWith(strLastName));
            }
            else
                topScorers = topScorers.Where(t => t.FirstName.ToLower().StartsWith(strFirstName) |
                                                   t.LastName.ToLower().StartsWith(strFirstName));
            
            foreach (Models.TopScorer ts in topScorers)
                listTopscorers.Add(ts.DisplayName);

            context.Response.Write("[\"" + string.Join("\",\"", listTopscorers) + "\"]");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
