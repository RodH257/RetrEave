using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Retreave.Domain.Models;
using PetaPoco;
namespace Retreave.Domain.DataAccess
{
    /// <summary>
    /// Data access for registered users
    /// </summary>
    public class RegisteredUserDao : IRegisteredUserDao
    {
        private PetaPoco.Database _database;

        public RegisteredUserDao()
        {
            _database = new Database("RetreaveSql");
        }

        /// <summary>
        /// Gets the user by their ID
        /// </summary>
        public RegisteredUser GetById(int id)
        {
            return _database.SingleOrDefault<RegisteredUser>("where userId=@0", id);
        }

        /// <summary>
        /// UpSerts the user
        /// </summary>
        public RegisteredUser SaveOrUpdate(RegisteredUser entity)
        {
            if (_database.IsNew(entity))
            {
                _database.Insert(entity);
                //save the authentication details
                _database.Insert(entity.AuthDetails);
            }
            else
            {
                _database.Update(entity);
            }

            _database.Update<RegisteredUser>("SET AuthenticationDetails = @0 where UserId=@1",
                                             entity.AuthDetails.AuthenticationDetailsId, entity.UserId);
            return entity;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(RegisteredUser entity)
        {
            _database.Delete(entity);
        }

        /// <summary>
        /// Gets the user by their user name 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public RegisteredUser GetUserByUserName(string userName)
        {
            var sql = Sql.Builder
                .Append("Select Users.*, Auth.* ")
                .Append("FROM Users")
                .Append("LEFT JOIN AuthenticationDetails Auth")
                .Append("On Users.AuthenticationDetails = Auth.AuthenticationDetailsId")
                .Append("Where Users.UserName = @0", userName);

            return GetUserByQuery(sql);
        }


        /// <summary>
        /// Gets a user by the query
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        private RegisteredUser GetUserByQuery(Sql query)
        {
            var results = _database.Query<RegisteredUser, TwitterAuthentication, RegisteredUser>
               (
                   (u, auth) =>
                   {
                       u.AuthDetails = auth;

                       return u;
                   }, query

               );

            RegisteredUser user = results.FirstOrDefault();
            if (user == null)
                return null;
            return user;
        }


        /// <summary>
        /// Helper method to retrieve the joined authentication and registered user 
        /// </summary>
        /// <param name="sqlQuery">pre formed SQL</param>
        /// <returns></returns>
        private IEnumerable<RegisteredUser> GetUsersByQuery(Sql query)
        {
            var results = _database.Query<RegisteredUser, TwitterAuthentication, RegisteredUser>
               (
                   (u, auth) =>
                   {
                       u.AuthDetails = auth;

                       return u;
                   }, query

               );

            return results;
        }


        /// <summary>
        /// Gets the user by their index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal IEnumerable<RegisteredUser> GetUsersByIndex(RetreaveIndex index)
        {
           var sql =  Sql.Builder
                .Append(" Select U.*, AUTH.*")
                .Append("From [RetrEave].[dbo].AuthenticationDetails AUTH ")
                .Append("JOIN [RetrEave].[dbo].[Users] U on AUTH.AuthenticationDetailsId = U.AuthenticationDetails")
                .Append("JOIN [RetrEave].[dbo].Users_Indexes")
                .Append("ON U.UserId = Users_Indexes.UserId ")
                .Where("Users_Indexes.IndexId = @0", index.IndexId);

            return GetUsersByQuery(sql);
        }
    }
}
