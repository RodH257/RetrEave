using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.Infrastructure;

namespace Retreave.Domain.Services
{
    /// <summary>
    /// Service Layer
    /// </summary>
    public static class ServiceLayer
    {
        public static IIndexQueuerService IndexQueuerService
        {
            get { return new IndexQueuerService(); }
        }

    }
}
