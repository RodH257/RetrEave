using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.DataAccess;
using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public class UserDetailsService : IUserDetailsService
    {
        private RegisteredUserDao _userDao;

        public UserDetailsService(RegisteredUserDao userDao)
        {
            _userDao = userDao;
        }

        public void CreateUser(RegisteredUser user)
        {
            _userDao.SaveOrUpdate(user);
        }
    }
}
