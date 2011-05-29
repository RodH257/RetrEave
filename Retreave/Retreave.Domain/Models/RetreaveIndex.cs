using System;
using System.Collections.Generic;
using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// An index, could be a twitter stream, hash tag, facebook stream etc. 
    /// </summary>
    [PetaPoco.TableName("Indexes")]
    [PetaPoco.PrimaryKey("IndexId", autoIncrement = true)]
    public class RetreaveIndex
    {
        public IndexType IndexType { get; set; }
        public string Name { get; set; }
        public string IndexStreamIdentifier { get; set; }
        public int IndexId { get; set; }
        public bool Active { get; set; }
        public DateTime? LastProcessed { get; set; }
        public DateTime DateAdded { get; set; }
        [PetaPoco.Ignore]
        public IList<RegisteredUser> AssociatedUsers { get; set; }

        public RetreaveIndex()
        {
            this.AssociatedUsers = new List<RegisteredUser>();
        }

       
    }
}
