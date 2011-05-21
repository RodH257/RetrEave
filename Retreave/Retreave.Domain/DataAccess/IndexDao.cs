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
        public IndexDao()
        {
            _database = new Database("RetreaveSql");
        }

        public Index GetById(int id)
        {
            return _database.SingleOrDefault<Index>("where indexId=@0", id);
        }


        /// <summary>
        /// Craetes/updates an index
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Index SaveOrUpdate(Index entity)
        {
            if (_database.IsNew(entity))
                _database.Insert(entity);
            else
                _database.Update(entity);

            return entity;
        }


        /// <summary>
        /// Deletes the index
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Index entity)
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
        internal IEnumerable<Index> GetUnprocessedIndexes()
        {
            return _database.Query<Index>("WHERE LastProcessed IS NULL");
        }


        /// <summary>
        /// Gets the oldest index of at least a certain minimum age
        /// </summary>
        /// <param name="minumumAge"></param>
        /// <returns></returns>
        public Index GetMostOutdatedIndex(DateTime minumumAge)
        {
            return _database.SingleOrDefault<Index>("WHERE LastProcessed < @0 ORDER BY LastProcessed ASC");
        }
    }
}
