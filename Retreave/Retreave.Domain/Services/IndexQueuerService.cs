using Retreave.Domain.DataAccess;
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
            Index streamIndex =  user.AddTwitterStreamIndex();
            _indexDao.SaveOrUpdate(streamIndex);
        }
    }
}
