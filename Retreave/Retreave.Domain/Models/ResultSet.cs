using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A set of Results
    /// </summary>
    public class ResultSet
    {
        public SearchQuery OriginalQuery { get; set; }
        public IList<Result> Results { get; set; }
        public ResultSet(SearchQuery originalQuery)
        {
            this.OriginalQuery = originalQuery;
            this.Results = new List<Result>();

        }
    }
}
