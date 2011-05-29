using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Retreave.Domain;
using Retreave.Domain.Models;
using Retreave.Indexer.Indexers;

namespace Retreave.Indexer.Processors
{
    public class StreamQueuedTweetsProcessor
    {
        private IndexSearcher _tweetSearcher;
        private UrlIndexer _urlIndexer;
        public StreamQueuedTweetsProcessor()
        {
            FSDirectory tweetDirectory = FSDirectory.Open(new DirectoryInfo(Settings.TWEET_INDEX_DIR), null);
            _tweetSearcher = new IndexSearcher(tweetDirectory, true);
            _urlIndexer = new UrlIndexer();
        }


        /// <summary>
        /// Processes the next tweet from the queue
        /// </summary>
        public void ProcessNextInQueue()
        {
            //read the tweets with a MinValue as their index date 
            TermQuery query = new TermQuery(new Term(Settings.FIELD_TWEET_DATE_INDEXED, "0"));
            Filter filter = new QueryWrapperFilter(query);
            TopDocs results = _tweetSearcher.Search(query, filter, 1);

            //if no results, return
            if (results.totalHits == 0)
                return;

            Document tweetDoc = _tweetSearcher.Doc(results.scoreDocs[0].doc);

            //construct the tweet
            Tweet tweetToProcess = GetTweetFromDocument(tweetDoc);

            Console.WriteLine("Processing Tweet " + tweetToProcess.TweetId);

            //get the indexes
            var indexes = GetIndexesFromDocument(tweetDoc);

            //index the urls from the tweet
            _urlIndexer.IndexUrlsInTweet(tweetToProcess, indexes);
        }


        /// <summary>
        /// Gets the indexes from the document fields
        /// </summary>
        private IList<string> GetIndexesFromDocument(Document tweetDoc)
        {
            Field[] indexesFields = tweetDoc.GetFields(Settings.FIELD_TWEET_INDEXES);
            List<string> indexes = new List<string>();
            foreach (Field field in indexesFields)
            {
                indexes.Add(field.StringValue());
            }
            return indexes;
        } 

        /// <summary>
        /// Extracts the tweet from the document fields
        /// </summary>
        private Tweet GetTweetFromDocument(Document tweetDoc)
        {
            Tweet tweet = new Tweet();

            tweet.Content = tweetDoc.GetField(Settings.FIELD_TWEET_TEXT).StringValue();
            tweet.TweetId = long.Parse(tweetDoc.GetField(Settings.FIELD_TWEET_ID).StringValue());
            tweet.DatePosted = DateTime.Parse(tweetDoc.GetField(Settings.FIELD_TWEET_DATE_POSTED).StringValue());
            tweet.Author = new Linker();
            tweet.Author.Id = tweetDoc.GetField(Settings.FIELD_TWEET_LINKER_ID).StringValue();
            tweet.Author.Name = tweetDoc.GetField(Settings.FIELD_TWEET_LINKER_NAME).StringValue();
            tweet.Author.ReputationScore = double.Parse(tweetDoc.GetField(Settings.FIELD_TWEET_LINKER_REP).StringValue());


            return tweet;
        }
    }
}
