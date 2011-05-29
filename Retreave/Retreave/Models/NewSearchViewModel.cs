using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Retreave.Domain.Models;

namespace Retreave.Models
{
    public class NewSearchViewModel
    {
        public IEnumerable<RetreaveIndex> SearchableIndexes { get; set; }
        public RegisteredUser User { get; set; }
        public string SearchText { get; set; }
    }
}