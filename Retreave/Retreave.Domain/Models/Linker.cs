namespace Retreave.Domain.Models
{
    /// <summary>
    /// Someone who posted a link
    /// </summary>
    public class Linker
    {
        public string Id { get; set; }
        public string Name { get; set;}
        public Index Index { get; set; }
        public double ReputationScore { get; set; }
    }
}
