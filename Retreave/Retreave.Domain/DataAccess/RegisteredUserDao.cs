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
            throw new NotImplementedException();
        }

        public void Delete(RegisteredUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
