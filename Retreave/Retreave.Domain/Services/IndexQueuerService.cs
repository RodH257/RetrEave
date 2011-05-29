using System;
using System.Collections.Generic;
using System.Linq;
using Retreave.Domain.DataAccess;
using Retreave.Domain.Enums;
using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    /// <summary>
    /// Service Layer for queuing indexes 
    /// </summary>
    public class IndexQueuerService : IIndexQueuerService
    {
        private IndexDao _indexDao;
        public IndexQueuerService(IndexDao indexDao)
        {
            _indexDao = indexDao;
        }

        /// <summary>
        /// Adds a users personal feed to the queue for indexing
        /// </summary>
        /// <param name="user">the user to queue</param>
        public void QueueUserStreamIndex(RegisteredUser user)
        {
            //add a a new StreamIndex to the users indexes
            RetreaveIndex streamIndex = new RetreaveIndex()
                              {
                                  IndexType = IndexType.TwitterStreamIndex,
                                  Name = "Twitter Stream for " + user.UserName,
                                  Active = true,
                                  DateAdded = DateTime.Now,
                                  IndexStreamIdentifier = user.TwitterId.ToString()
                              };
            streamIndex.AssociatedUsers.Add(user);

            _indexDao.SaveOrUpdate(streamIndex);
        }


        /// <summary>
        /// Gets the next index to process
        /// </summary>
        public RetreaveIndex GetNextIndexToProcess()
        {
            //first, see if there is any indexes that have not ever been processed.
            IEnumerable<RetreaveIndex> unprocessedIndexes = _indexDao.GetUnprocessedIndexes();

            return unprocessedIndexes.FirstOrDefault();

        }

        /// <summary>
        /// Gets the indexes queued by a certain user
        /// </summary>
        public IEnumerable<RetreaveIndex> GetIndexesQueuedByUser(int userId)
        {
            return _indexDao.GetIndexesbyUserId(userId);
        }

        /// <summary>
        /// Gets the indexes by ID list
        /// DEBT: Could be combined into one query 
        /// </summary>
        public IEnumerable<RetreaveIndex> GetIndexesByIdList(IEnumerable<int> selectedIndexes)
        {
            IList<RetreaveIndex> indexes = new List<RetreaveIndex>();

            foreach (int indexId in selectedIndexes)
            {
                indexes.Add(_indexDao.GetById(indexId));
            }

            return indexes;
        }


        /// <summary>
        /// Gets the indexes to stream
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RetreaveIndex> GetUserIndexesToStream()
        {
            return _indexDao.GetActiveUserIndexes();
        }

        public IEnumerable<RetreaveIndex> GetIndexesByUniqueIdList(List<string> indexIds)
        {
            IList<RetreaveIndex> indexes = new List<RetreaveIndex>();

            foreach (string indexId in indexIds)
            {
                indexes.Add(_indexDao.GetByUniqueIdentifier(indexId));
            }

            return indexes;
        }

        /// <summary>
        /// Sets an index complete.
        /// </summary>
        /// <param name="indexId"></param>
        public void MarkIndexComplete(int indexId)
        {
            _indexDao.MarkProcessed(indexId, DateTime.Now);
        }
    }
}
