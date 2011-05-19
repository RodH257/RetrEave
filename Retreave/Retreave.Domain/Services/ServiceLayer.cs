using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.DataAccess;
using Retreave.Domain.Infrastructure;

namespace Retreave.Domain.Services
{
    /// <summary>
    /// Service Layer
    /// </summary>
    public static class ServiceLayer
    {
        /// <summary>
        /// Gets the Index Queuing service layer
        /// </summary>
        public static IIndexQueuerService IndexQueuerService
        {
            get { return new IndexQueuerService(new IndexDao()); }
        }
        
        /// <summary>
        /// Gets the User Details service layer
        /// </summary>
        public static IUserDetailsService UserDetailsService
        {
            get { return new UserDetailsService(new RegisteredUserDao()); }
        }



    }
}
