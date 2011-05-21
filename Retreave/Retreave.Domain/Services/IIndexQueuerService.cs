using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public interface IIndexQueuerService
    {
        void QueueUserStreamIndex(RegisteredUser user);

        /// <summary>
        /// Gets the next index to process in lucene
        /// </summary>
        /// <returns></returns>
        Index GetNextIndexToProcess();

        /// <summary>
        /// Marks an index as completely processed up to the current time. 
        /// </summary>
        /// <param name="indexId"></param>
        void MarkIndexComplete(int indexId);
    }
}