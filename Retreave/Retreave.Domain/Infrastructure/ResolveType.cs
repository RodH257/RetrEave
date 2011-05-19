using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Retreave.Domain.DataAccess;
using Retreave.Domain.Services;

namespace Retreave.Domain.Infrastructure
{
    public class ResolveType
    {
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IIndexQueuerService>().To<IndexQueuerService>();
            kernel.Bind<IRegisteredUserDao>().To<RegisteredUserDao>();
            kernel.Bind<IUserDetailsService>().To<UserDetailsService>();
            kernel.Bind<IIndexDao>().To<IndexDao>();

        }


    }
}
