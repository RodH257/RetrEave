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
    }
}
