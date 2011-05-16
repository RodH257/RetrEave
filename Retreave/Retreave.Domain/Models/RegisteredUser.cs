using System.Collections.Generic;
using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A registered user
    /// </summary>
    public class RegisteredUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public IList<Index> ActiveIndexes { get; set; }
        public TwitterAuthentication AuthDetails { get; set; }
        
        public RegisteredUser()
        {
            ActiveIndexes = new List<Index>();
        }

        /// <summary>
        /// Add an index
        /// </summary>
        public Index AddStreamIndex()
        {
            Index index = new Index()
                              {
                                  IndexType = IndexType.TwitterStreamIndex,
                                  Name = UserName

                              };
            ActiveIndexes.Add(index);
            return index;
        }
    }
}
 