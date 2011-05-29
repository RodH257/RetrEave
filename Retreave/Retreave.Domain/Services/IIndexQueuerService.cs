using System.Collections.Generic;
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
        RetreaveIndex GetNextIndexToProcess();

        /// <summary>
        /// Marks an index as completely processed up to the current time. 
        /// </summary>
        /// <param name="indexId"></param>
        void MarkIndexComplete(int indexId);


        /// <summary>
        /// Gets the indexes taht a certain user has queued
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<RetreaveIndex> GetIndexesQueuedByUser(int userId);


        /// <summary>
        /// Converts a list of ID's to a list of indexes 
        /// </summary>
        IEnumerable<RetreaveIndex> GetIndexesByIdList(IEnumerable<int> selectedIndexes);


        /// <summary>
        /// Gets the user indexes to stream
        /// </summary>
        /// <returns></returns>
        IEnumerable<RetreaveIndex> GetUserIndexesToStream();

        /// <summary>
        /// Gets indexes by a list of their twitter specific ids
        /// </summary>
        /// <param name="indexIds"></param>
        /// <returns></returns>
        IEnumerable<RetreaveIndex> GetIndexesByUniqueIdList(List<string> indexIds);
    }
}