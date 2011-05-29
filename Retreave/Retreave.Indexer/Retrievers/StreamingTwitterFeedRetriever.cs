using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Retreave.Domain.Helpers;
using Retreave.Domain.Models;
using Retreave.Indexer.Indexers;
using TweetSharp;


namespace Retreave.Indexer.Retrievers
{
    internal class StreamingTwitterFeedRetriever
    {
        public bool Running { get; set; }
        TweetIndexer _tweetIndexer = new TweetIndexer();

        public void StreamUsersTweets(IList<RetreaveIndex> indexesToStream)
        {
            Running = true;
            DoUserStreams(indexesToStream);

            return;
            StringBuilder usersToFollow = new StringBuilder();
            bool first = true;
            //foreach uesr get the associated users
            //should only be one anyway.
            foreach (RetreaveIndex index in indexesToStream)
            {
                foreach (RegisteredUser user in index.AssociatedUsers)
                {
                    if (!first)
                        usersToFollow.Append(",");

                    usersToFollow.Append(user.TwitterId);
                    first = false;
                }
            }

            Console.WriteLine("Starting stream for " + usersToFollow.ToString());
            InMemoryCredentials credentials = new InMemoryCredentials();
            //user credentials
            credentials.AccessToken = AuthenticationTokens.AppOwnerAccessTokenSecret;
            credentials.OAuthToken = AuthenticationTokens.AppOwnerAccessToken;

            //app specific credentials
            credentials.ConsumerKey = AuthenticationTokens.TwitterConsumerKey;
            credentials.ConsumerSecret = AuthenticationTokens.TwitterConsumerSecret;

            //save to pin authorizer 
            PinAuthorizer authorizer = new PinAuthorizer();
            authorizer.Credentials = credentials;


            TwitterContext twitterCtx = new TwitterContext(authorizer);

            //DoSiteStream(twitterCtx, usersToFollow.ToString());
        }

        private void DoUserStreams(IList<RetreaveIndex> indexesToStream )
        {
            foreach (RetreaveIndex index in indexesToStream)
            {
                foreach (RegisteredUser user in index.AssociatedUsers)
                {
                    Console.WriteLine("Starting stream for " + user.UserName);
                    InMemoryCredentials credentials = new InMemoryCredentials();
                    //user credentials
                    credentials.AccessToken = user.AuthDetails.AccessTokenSecret;
                    credentials.OAuthToken = user.AuthDetails.AccessToken;

                    //app specific credentials
                    credentials.ConsumerKey = AuthenticationTokens.TwitterConsumerKey;
                    credentials.ConsumerSecret = AuthenticationTokens.TwitterConsumerSecret;

                    //save to pin authorizer 
                    PinAuthorizer authorizer = new PinAuthorizer();
                    authorizer.Credentials = credentials;


                    TwitterContext twitterCtx = new TwitterContext(authorizer);

                    var streaming =
                        (from strm in twitterCtx.UserStream
                         where strm.Type == UserStreamType.User
                         select strm)
                            .StreamingCallback(strm =>
                                                   {

                                                       StringBuilder blockBuilder = new StringBuilder();
                                                       int bracketCount = 0;
                                                       for (int i = 0; i < strm.Content.Length; i++)
                                                       {
                                                           blockBuilder.Append(strm.Content[i]);

                                                           if (!new[] { '{', '}' }.Contains(strm.Content[i]))
                                                           {
                                                               continue;
                                                           }

                                                           if (strm.Content[i] == '{')
                                                           {
                                                               bracketCount++;
                                                           }

                                                           if (strm.Content[i] == '}')
                                                           {
                                                               bracketCount--;
                                                           }

                                                           if (bracketCount == 0)
                                                           {
                                                               //TODO: remove the index identifier when site streaming turned on
                                                               Action<string, string> parseMethod = ParseMessage;
                                                               parseMethod.BeginInvoke(blockBuilder.ToString().Trim('\n'), index.IndexStreamIdentifier,
                                                                   null, null);
                                                               blockBuilder.Clear();
                                                           }
                                                       }

                                                       if (!Running)
                                                           strm.CloseStream();

                                                   })
                            .SingleOrDefault();
                }
            }
        }

        private void DoSiteStream(TwitterContext twitterCtx, string usersToFollow)
        {
            var streaming =
                         (from strm in twitterCtx.UserStream
                          where strm.Type == UserStreamType.Site
                          && strm.Follow == usersToFollow.ToString()
                          select strm)
                         .StreamingCallback(strm =>
                         {
                             Console.WriteLine(strm.Content);
                             //StringBuilder blockBuilder = new StringBuilder();
                             //int bracketCount = 0;
                             //for (int index = 0; index < strm.Content.Length; index++)
                             //{
                             //    blockBuilder.Append(strm.Content[index]);

                             //    if (!new[] { '{', '}' }.Contains(strm.Content[index]))
                             //    {
                             //        continue;
                             //    }

                             //    if (strm.Content[index] == '{')
                             //    {
                             //        bracketCount++;
                             //    }

                             //    if (strm.Content[index] == '}')
                             //    {
                             //        bracketCount--;
                             //    }

                             //    if (bracketCount == 0)
                             //    {
                             //        Action<string> parseMethod = ParseMessage;
                             //        parseMethod.BeginInvoke(blockBuilder.ToString().Trim('\n'), null, null);
                             //        blockBuilder.Clear();
                             //    }
                             //}

                             if (!Running)
                                 strm.CloseStream();

                         })
                         .SingleOrDefault();
        }


        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="p">The p.</param>
        private void ParseMessage(string p, string indexIdentifier )
        {
            // Console.WriteLine(p);
            JObject obj = (JObject)JsonConvert.DeserializeObject(p);

            //check its an update
            var status = obj.SelectToken("user", false);

            if (status != null)
            {
                var urls = obj.SelectToken("entities.urls", false);
                if (urls.HasValues)
                {
                    //it has a URL
                    Console.WriteLine(urls[0]["url"]);

                 
                    Tweet tweet = new Tweet();
                    tweet.Author = new Linker()
                                    {
                                        Id = obj.SelectToken("user.id", false).ToString(),
                                        Name = (string)obj.SelectToken("user.screen_name", false)
                                    };
                    tweet.Content = (string)obj.SelectToken("text", false);

                    tweet.TweetId = (long) obj.SelectToken("id", false);
                    tweet.DatePosted = Tweet.GetDateTimeFromTwitterFormat((string)obj["created_at"]);
                    tweet.ReTweetCount = (int)obj.SelectToken("retweet_count", false);
                    System.Diagnostics.Debug.WriteLine(tweet.Content);

                    //get the index
                    //TODO: when site streaming activated updated it for this 
                    //string indexIdentifier = (string) obj.SelectToken("for_user", false);
             
                    _tweetIndexer.IndexTweet(tweet, indexIdentifier);
                }
            }

            System.Diagnostics.Debug.WriteLine("Message: {0}", new object[] { obj.ToString() });
        }


    }
}
