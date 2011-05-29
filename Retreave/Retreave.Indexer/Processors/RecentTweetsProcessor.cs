using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Retreave.Domain;
using Retreave.Domain.Models;
using Retreave.Indexer.Indexers;
using Retreave.Indexer.Parsers;
using Retreave.Indexer.Retrievers;
using Retreave.Domain.Services;
using Version = Lucene.Net.Util.Version;

namespace Retreave.Indexer.Processors
{
    /// <summary>
    /// Indexes the tweets in the past
    /// </summary>
    public class RecentTweetsProcessor
    {
        private TweetIndexer _tweetIndexer = new TweetIndexer();
        private UrlIndexer _urlIndexer = new UrlIndexer();

        /// <summary>
        /// Perform the indexing on a twitter stream
        /// </summary>
        /// <param name="index">the stream to index</param>
        public void Process(RetreaveIndex index)
        {
            //get the users tweets
            TwitterStreamRetriever retreaver = new TwitterStreamRetriever(index.AssociatedUsers.First().AuthDetails);
            IList<Tweet> tweets = retreaver.GetAllTweets();

            IList<Tweet> tweetsWithUrls = new List<Tweet>();
            //find the ones with URLs
            foreach (Tweet tweet in tweets)
            {
                if (tweet.ContainsUrl())
                    tweetsWithUrls.Add(tweet);
            }
            
            Console.WriteLine("Found " + tweetsWithUrls.Count + " tweets out of " + tweets.Count + " Have URLs");

            //foreach one, retrieve the HTML);
            foreach (Tweet tweetWithUrl in tweetsWithUrls)
            {

                //index the tweet in the tweet index
                _tweetIndexer.IndexTweet(tweetWithUrl, index.IndexStreamIdentifier);

                //index all the URl's from tweet - this should be done later though.
               // _urlIndexer.IndexUrlsInTweet(tweetWithUrl, index);
            }

            ServiceLayer.IndexQueuerService.MarkIndexComplete(index.IndexId);
        }
    }
}
