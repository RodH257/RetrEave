using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A query to the database
    /// </summary>
    public class SearchQuery
    {
        public IList<RetreaveIndex> IndexesToSearch { get; set; }
        public string QueryText { get; set; }
        public RegisteredUser Querier { get; set; }
    }
}
