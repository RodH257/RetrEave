using System;
using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// An index, could be a twitter stream, hash tag, facebook stream etc. 
    /// </summary>
    [PetaPoco.TableName("Indexes")]
    [PetaPoco.PrimaryKey("IndexId", autoIncrement = true)]
    public class Index
    {
  
        public IndexType IndexType { get; set; }
        public string Name { get; set; }
        public int IndexId { get; set; }
        public bool Active { get; set; }
        public DateTime LastProcessed { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
