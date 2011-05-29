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
            IEnumerable<TwitterStatus> returnedTweets = null;

            //try it a few times
            int retryCount = 0;
            while (returnedTweets == null)
            {
                returnedTweets = twitterService.ListTweetsOnHomeTimeline(200);

                retryCount++;

                //give up after 5 retries
                if (retryCount == 5)
                    return tweets;
            }

            foreach (var returnedTweet in returnedTweets)
            {
                Tweet tweet = new Tweet();

                TwitterStatus statusToExamine = returnedTweet;

                if (returnedTweet.RetweetedStatus != null)
                {
                    statusToExamine = returnedTweet.RetweetedStatus;

                }

                tweet.Author = new Linker() { Id = statusToExamine.Author.ScreenName, Name = statusToExamine.Author.ScreenName };
                tweet.Content = statusToExamine.Text;
                tweet.DatePosted = statusToExamine.CreatedDate;
                tweet.TweetId = statusToExamine.Id;
                tweet.ReTweetCount = GetRetweetCountFromRawData(statusToExamine.RawSource);

                tweets.Add(tweet);
            }
            return tweets;
        }

        //TODO: parse Json
        private int GetRetweetCountFromRawData(string data)
        {
            return 0;
        }

    }
}
