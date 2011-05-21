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

        public void Delete(RegisteredUser entity)
        {
            _database.Delete(entity);
        }

        public RegisteredUser GetUserByUserName(string userName)
        {
            var sql = PetaPoco.Sql.Builder
                .Append("Select Users.*, Auth.* ")
                .Append("FROM Users")
                .Append("LEFT JOIN AuthenticationDetails Auth")
                .Append("On Users.AuthenticationDetails = Auth.AuthenticationDetailsId")
                .Append("Where Users.UserName = @0", userName);
            

            var results = _database.Query<RegisteredUser, TwitterAuthentication, RegisteredUser>
                (
                    (u, auth) =>
                    {
                        u.AuthDetails = auth;

                        return u;
                    }, sql

                );

            RegisteredUser user = results.FirstOrDefault();

            if (user == null)
                return null;



            return user;
        }
    }
}
