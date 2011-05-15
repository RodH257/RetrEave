using System.Collections.Generic;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A registered user
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public IList<Index> ActiveIndexes { get; set; }
    }
}
 