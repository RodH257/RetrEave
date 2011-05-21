using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// Stores Oauth authentication details from twitter.
    /// </summary>
    [PetaPoco.TableName("AuthenticationDetails")]
    [PetaPoco.PrimaryKey("AuthenticationDetailsId", autoIncrement = true)]
    public class TwitterAuthentication
    {
        public int AuthenticationDetailsId { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
