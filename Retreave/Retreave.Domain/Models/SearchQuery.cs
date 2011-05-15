﻿using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A query to the database
    /// </summary>
    public class SearchQuery
    {
        public IList<Index> IndexesToSearch { get; set; }
        public string QueryText { get; set; }
        public User Querier { get; set; }
    }
}