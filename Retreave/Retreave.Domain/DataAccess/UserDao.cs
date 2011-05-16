using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.Models;

namespace Retreave.Domain.DataAccess
{
   public class UserDao: IDao<RegisteredUser, int> 
    {
        public RegisteredUser GetById(int id)
        {
            throw new NotImplementedException();
        }

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
