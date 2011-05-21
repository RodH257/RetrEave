using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.Helpers;
using Retreave.Domain.Models;
using TweetSharp;

namespace Retreave.Indexer.Retrievers
{
    class TwitterStreamRetriever
    {
        private TwitterAuthentication _authentication;

        public TwitterStreamRetriever(TwitterAuthentication authentication)
        {
            this._authentication = authentication;
        }


        /// <summary>
        /// Gets all the users tweets.
        /// </summary>
        /// <returns></returns>
        public IList<Tweet> GetAllTweets()
        {
            List<Tweet> tweets = new List<Tweet>();

            TwitterService twitterService = new TwitterService(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret);
            twitterService.AuthenticateWith(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret,
                                      _authentication.AccessToken, _authentication.AccessTokenSecret);
            
            TwitterUser twitterUser = twitterService.VerifyCredentials();

            //ListTweetsOnHomeTimeline only returns 200 (or 800?) results each go. Need to send the requests a few times per hour 
            //with the paging/counts set?
            var returnedTweets = twitterService.ListTweetsOnHomeTimeline(200);

            foreach (var returnedTweet in returnedTweets)
            {
                Tweet tweet = new Tweet();
                tweet.Author = new Linker()
                                   {Id = returnedTweet.Author.ScreenName, Name = returnedTweet.Author.ScreenName};
                tweet.Content = returnedTweet.Text;
                tweet.DatePosted = returnedTweet.CreatedDate;

                tweets.Add(tweet);
            }
            return tweets;
        }
        
    }
}
