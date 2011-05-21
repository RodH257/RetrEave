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
            RetreaveIndex streamIndex =  new RetreaveIndex()
                              {
                                  IndexType = IndexType.TwitterStreamIndex,
                                  Name = user.UserName,
                                  Active = true,
                                  DateAdded = DateTime.Now
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

            if (unprocessedIndexes.Count() > 0)
                return unprocessedIndexes.First();

            //get the most outdated index.
            //this may one day get updated with some sort of weighting on indexes,
            // ie most frequent visitors or something
            //Make sure the index is at least 5 minutes old
            DateTime fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            return _indexDao.GetMostOutdatedIndex(fiveMinutesAgo);
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
