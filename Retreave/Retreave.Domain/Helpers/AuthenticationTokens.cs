using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retreave.Domain.Helpers
{
    /// <summary>
    /// Stores the authentication tokens for oAuth sites.
    /// </summary>
    public class AuthenticationTokens
    {

        /// <summary>
        /// Twitter app consumer key 
        /// </summary>
        public static string TwitterConsumerKey
        {
            get { return "OIRTdpl2BPSGD3qMx1OlQ"; }
        }

        /// <summary>
        /// Twitter app consumer secret
        /// </summary>
        public static string TwitterConsumerSecret
        {
            get { return "2TSEWXQV73iX3Je6h6xoo69nCP4FxKPiOcEkCpIbns"; }
        }

        /// <summary>
        /// Access token for Rodh257
        /// </summary>
        public static string AppOwnerAccessToken
        {
            get { return "32148489-OuUCbiyPB3d8LIbv18eKNpFx7K4oFk5lcm0nX7xHB"; }
        }

        /// <summary>
        /// Access token secret for Rodh257
        /// </summary>
        public static string AppOwnerAccessTokenSecret
        {
            get { return "GGnU5iR8Pf8MGR6OEI1r5ibOT9SkviKwp5IxlVg2Y9I"; }
        }
    }
}
