using System.Collections.Generic;
using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// A registered user
    /// </summary>
    [PetaPoco.TableName("Users")]
    [PetaPoco.PrimaryKey("UserId")]
    public class RegisteredUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IList<Index> ActiveIndexes { get; set; }
        public TwitterAuthentication AuthDetails { get; set; }
        
        public RegisteredUser()
        {
            ActiveIndexes = new List<Index>();
        }

        /// <summary>
        /// Adds an index of the users current twitter stream
        /// </summary>
        public Index AddTwitterStreamIndex()
        {
            Index index = new Index()
                              {
                                  IndexType = IndexType.TwitterStreamIndex,
                                  Name = UserName,
                                  Active = true
                              };
            ActiveIndexes.Add(index);
            return index;
        }
    }
}
 