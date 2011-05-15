using System;
using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A result to a SearchQuery
    /// </summary>
    public class Result
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public IList<Index> OriginatingIndexes { get; set; }
        public IList<Linker> Linkers { get; set; }
    }
}
