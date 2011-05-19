using Retreave.Domain.Enums;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// An index, could be a twitter stream, hash tag, facebook stream etc. 
    /// </summary>
    public class Index
    {
        public IndexType IndexType { get; set; }
        public string Name { get; set; }
        public int IndexId { get; set; }
        public bool Active { get; set; }

    }
}
