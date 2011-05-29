using System;
using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A result to a SearchQuery
    /// </summary>
    public class Result
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public IList<RetreaveIndex> OriginatingIndexes { get; set; }
        public IList<Linker> Linkers { get; set; }
         
        public Result()
        {
            this.OriginatingIndexes = new List<RetreaveIndex>();
            this.Linkers = new List<Linker>();
        }
    }
}
