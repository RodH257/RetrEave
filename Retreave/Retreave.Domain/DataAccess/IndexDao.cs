using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Retreave.Domain.Models;

namespace Retreave.Domain.DataAccess
{
    /// <summary>
    /// Data access for Index
    /// </summary>
    public class IndexDao: IIndexDao
    {
        private PetaPoco.Database _database;
        private RegisteredUserDao _registeredUserDao;

        /// <summary>
        /// Dependencies on the web config string
        /// and also the registered user dao
        /// </summary>
        public IndexDao()
        {
            _database = new Database("RetreaveSql");
            _registeredUserDao = new RegisteredUserDao();
        }

        public RetreaveIndex GetById(int id)
        {
            var result= _database.SingleOrDefault<RetreaveIndex>("where indexId=@0", id);
            return FillResult(result);
        }


        /// <summary>
        /// Craetes/updates an index
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RetreaveIndex SaveOrUpdate(RetreaveIndex entity)
        {
            if (_database.IsNew(entity))
                _database.Insert(entity);
            else
                _database.Update(entity);

            //update the associated users 
            _database.Execute("Delete from Users_Indexes where indexId=@0", entity.IndexId);
            
            foreach (RegisteredUser user in entity.AssociatedUsers)
            {
                if (_database.IsNew(user))
                    throw new Exception("You must save the User entity first");

                _database.Execute("Insert into Users_indexes (UserId, IndexId) values (@0, @1)", user.UserId,
                                  entity.IndexId);
            }
            return entity;
        }


        /// <summary>
        /// Deletes the index
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(RetreaveIndex entity)
        {
            _database.Delete(entity);
        }


        /// <summary>
        /// Updates the last summary date
        /// </summary>
        internal void MarkProcessed(int indexId, DateTime dateTime)
        {
            _database.Execute("Update Indexes Set LastProcessed=@0 where IndexId=@1", dateTime, indexId);
        }

        /// <summary>
        /// Gets any indexes with a null LastProcessed
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<RetreaveIndex> GetUnprocessedIndexes()
        {
            var results= _database.Fetch<RetreaveIndex>("WHERE LastProcessed IS NULL AND Active = 1");
            return FillResults(results);
        }


        /// <summary>
        /// Gets the oldest index of at least a certain minimum age
        /// </summary>
        /// <param name="minumumAge"></param>
        /// <returns></returns>
        public RetreaveIndex GetMostOutdatedIndex(DateTime minumumAge)
        {
            var result = _database.FirstOrDefault<RetreaveIndex>("WHERE LastProcessed < @0 AND Active = 1 ORDER BY LastProcessed ASC ", minumumAge);
            return FillResult(result);
        }

        /// <summary>
        /// Fills out each of the objects in a result set.
        /// </summary>
        /// <param name="results">an number of indexes returned in their basic form</param>
        /// <returns></returns>
        public IEnumerable<RetreaveIndex> FillResults(IEnumerable<RetreaveIndex> results)
        {

            foreach (RetreaveIndex index in results)
            {
                index.AssociatedUsers = _registeredUserDao.GetUsersByIndex(index).ToList();
            }


            return results;
        }

        /// <summary>
        /// Fills out the rest of the properties on an index object
        /// by executing more queries
        /// </summary>
        /// <param name="returnedIndex">an index returned in its basic form</param>
        /// <returns></returns>
        public RetreaveIndex FillResult(RetreaveIndex returnedIndex)
        {
            if (returnedIndex == null)
                return null;
            returnedIndex.AssociatedUsers = _registeredUserDao.GetUsersByIndex(returnedIndex).ToList();
            return returnedIndex;
        }

        /// <summary>
        /// Gets the indexes a user is associated with
        /// </summary>
        internal IEnumerable<RetreaveIndex> GetIndexesbyUserId(int userId)
        {
            var sql = Sql.Builder
                     .Append(" Select I.*")
                     .Append("From [RetrEave].[dbo].Indexes I ")
                     .Append("JOIN [RetrEave].[dbo].Users_Indexes")
                     .Append("ON I.IndexId = Users_Indexes.IndexId ")
                     .Where("Users_Indexes.UserId = @0", userId);


            var results = _database.Fetch<RetreaveIndex>(sql);
            return FillResults(results);
        }

        /// <summary>
        /// Gets the user indexes taht are active
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<RetreaveIndex> GetActiveUserIndexes()
        {
            var sql = Sql.Builder
            .Append(" Select I.*")
            .Append("From [RetrEave].[dbo].Indexes I ")
            .Append("JOIN [RetrEave].[dbo].Users_Indexes")
            .Append("ON I.IndexId = Users_Indexes.IndexId ")
            .Where("I.Active = 1");


            var results = _database.Fetch<RetreaveIndex>(sql);
            return FillResults(results);
        }

        internal RetreaveIndex GetByUniqueIdentifier(string indexStreamIdentifier)
        {
            var result = _database.SingleOrDefault<RetreaveIndex>("where IndexStreamIdentifier=@0", indexStreamIdentifier);
            return FillResult(result);
        }
    }
}
