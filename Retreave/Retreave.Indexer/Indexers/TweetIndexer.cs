using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Retreave.Domain;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
using Version = Lucene.Net.Util.Version;

namespace Retreave.Indexer.Indexers
{
    /// <summary>
    /// Indexes the actual tweets themselves
    /// </summary>
    public class TweetIndexer
    {
        private ISearchEngineService _searchengine;
        private StandardAnalyzer _analyzer;
        public TweetIndexer()
        {
            _searchengine = ServiceLayer.SearchEngineService;
            _analyzer = new StandardAnalyzer(Version.LUCENE_29);
        }


        /// <summary>
        /// Indexes a tweet in the TweetIndex 
        /// Checks if it is new, if not, it will update the index list with the indexId
        /// If it is new, it will set it as unprocessed so the UrlIndexer will read it
        /// </summary>
        public void IndexTweet(Tweet tweetToIndex, string indexId)
        {
            //setup index writing 
            FSDirectory tweetDirectory = FSDirectory.Open(new DirectoryInfo(Settings.TWEET_INDEX_DIR));
            IndexWriter tweetWriter = new IndexWriter(tweetDirectory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            //check the tweet is not already indexed.
            Document existingTweet = _searchengine.GetDocumentForTweetId(tweetToIndex.TweetId);

            //if the tweet doens't exist, index it.
            if (existingTweet == null)
            {
                Document tweetDocument = IndexTweetDetails(tweetToIndex, indexId);
                tweetWriter.AddDocument(tweetDocument);
            }
            else
            {
                //update the index
                UpdateIndexForDocument(indexId, existingTweet);
            }

            tweetWriter.Optimize();
            tweetWriter.Close();
        }


        /// <summary>
        /// Updates the indexes for a tweet document
        /// </summary>
        private void UpdateIndexForDocument(string indexId, Document existingTweet)
        {
            Field[] indexesFields = existingTweet.GetFields(Settings.FIELD_URL_INDEXES);

            //get all the current users 
            List<string> indexes = new List<string>();
            foreach (Field field in indexesFields)
            {
                indexes.Add(field.StringValue());
            }


            //see if the indexes contains the current index
            if (!indexes.Contains(indexId))
            {
                //if not, add it 
                Field newIndexField = new Field(Settings.FIELD_URL_INDEXES, indexId.ToString(), Field.Store.YES, Field.Index.ANALYZED);
                existingTweet.Add(newIndexField);
            }
        }

        /// <summary>
        /// Updates the date indexed to be now.
        /// </summary>
        public void UpdateDateIndexed(Tweet tweetToUpdate)
        {
            FSDirectory tweetDirectory = FSDirectory.Open(new DirectoryInfo(Settings.TWEET_INDEX_DIR));
            IndexWriter tweetWriter = new IndexWriter(tweetDirectory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            //update the date indexed on the tweet
            Document existingTweet = _searchengine.GetDocumentForTweetId(tweetToUpdate.TweetId);

            //update the field when it was updated
            Field dateUpdated = existingTweet.GetField(Settings.FIELD_TWEET_DATE_INDEXED);
            dateUpdated.SetValue(DateTime.Now.ToString());
            
            tweetWriter.UpdateDocument(new Term(Settings.FIELD_TWEET_ID, existingTweet.GetField(Settings.FIELD_TWEET_ID).StringValue()), existingTweet);

            tweetWriter.Close();
        }

        /// <summary>
        /// Gets the lucene document for a certain tweet
        /// </summary>
        private Document IndexTweetDetails(Tweet tweet, string indexId)
        {
            Document luceneDocument = new Document();
            Field textField = new Field(Settings.FIELD_TWEET_TEXT, tweet.Content, Field.Store.YES, Field.Index.ANALYZED);
            Field idField = new Field(Settings.FIELD_TWEET_ID, tweet.TweetId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field linkerNameField = new Field(Settings.FIELD_TWEET_LINKER_ID, tweet.Author.Name, Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field linkerIdField = new Field(Settings.FIELD_TWEET_LINKER_NAME, tweet.Author.Id, Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field linkerRepField = new Field(Settings.FIELD_TWEET_LINKER_REP, tweet.Author.ReputationScore.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field datePostedField = new Field(Settings.FIELD_TWEET_DATE_POSTED, tweet.DatePosted.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field dateUpdatedField = new Field(Settings.FIELD_TWEET_DATE_INDEXED, "0", Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field indexField = new Field(Settings.FIELD_TWEET_INDEXES, indexId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);

            luceneDocument.Add(textField);
            luceneDocument.Add(idField);
            luceneDocument.Add(linkerNameField);
            luceneDocument.Add(linkerRepField);
            luceneDocument.Add(linkerIdField);
            luceneDocument.Add(datePostedField);
            luceneDocument.Add(dateUpdatedField);
            luceneDocument.Add(indexField);

            return luceneDocument;
        }



    }
}
