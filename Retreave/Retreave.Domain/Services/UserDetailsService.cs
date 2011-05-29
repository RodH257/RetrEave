using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.DataAccess;
using Retreave.Domain.Helpers;
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

        /// <summary>
        /// Checks or creates a twitter based login.
        /// </summary>
        /// <param name="authentication"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public RegisteredUser AuthenticateTwitterAccount(TwitterAuthentication authentication, string userName, int twitterId)
        {
            //see if there is alraedy an account.
            RegisteredUser returnedUser = _userDao.GetUserByUserName(userName);

            //check if its new 
            if (returnedUser == null)
            {
                //save the user details 
                returnedUser = new RegisteredUser()
                                          {
                                              AuthDetails = authentication,
                                              UserName = userName,
                                              TwitterId = twitterId
                                          };
                IUserDetailsService userDetailsService = ServiceLayer.UserDetailsService;
                userDetailsService.CreateUser(returnedUser);

                //setup the indexing of their tweets
                IIndexQueuerService indexQueuerService = ServiceLayer.IndexQueuerService;
                indexQueuerService.QueueUserStreamIndex(returnedUser);
            }
            else
            {
                //make sure the tokens match
                if (!returnedUser.AuthDetails.AccessToken.Equals(authentication.AccessToken))
                    return null;
            }

            return returnedUser;
        }

        /// <summary>
        /// Gets a user by their user name
        /// </summary>
        public RegisteredUser GetUserByUserName(string userName)
        {
           return  _userDao.GetUserByUserName(userName);
        }


        public void CreateUser(RegisteredUser user)
        {
            _userDao.SaveOrUpdate(user);
        }
    }
}
