using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// Stores Oauth authentication details from twitter.
    /// </summary>
    public class TwitterAuthentication
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
