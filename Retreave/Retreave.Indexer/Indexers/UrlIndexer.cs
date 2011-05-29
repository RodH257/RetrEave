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
using Retreave.Indexer.Parsers;
using Retreave.Indexer.Retrievers;

namespace Retreave.Indexer.Indexers
{
    /// <summary>
    /// Performs the indexing of HTML linked from tweets
    /// </summary>
    public class UrlIndexer
    {
        private ISearchEngineService _searchengine;
        private StandardAnalyzer _analyzer;
        private TweetIndexer _tweetIndexer;
        public UrlIndexer()
        {
            _searchengine = ServiceLayer.SearchEngineService;
            _analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            _tweetIndexer = new TweetIndexer();
        }

        /// <summary>
        /// Indexes all the urls in the supplied tweet
        /// </summary>
        public void IndexUrlsInTweet(Tweet tweet, IList<string> indexes)
        {
            //find urls to index in the url index
            foreach (Uri uri in tweet.GetUrlsFromTweet())
            {
                Console.WriteLine("URL" + uri);

                //setup index writer
                FSDirectory luceneDirectory = FSDirectory.Open(new DirectoryInfo(Settings.URL_INDEX_DIR));
                IndexWriter writer = new IndexWriter(luceneDirectory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);


                //need to check if its not already indexed
                //if it is already indexed, then just add a user to the index field in lucene
                Document existingDoc = _searchengine.GetDocumentForUrl(uri.ToString());
                if (existingDoc != null)
                {
                    //document already exists, add a user to it.
                    Console.WriteLine("Already Exists");

                    bool wasUpdated = false;

                    wasUpdated |= UpdateIndexes(existingDoc, indexes);
                    wasUpdated |= UpdateTweets(existingDoc, tweet.TweetId);

                    //only update document if it was changed.
                    if (wasUpdated)
                        writer.UpdateDocument(new Term(Settings.FIELD_URL_ID, existingDoc.GetField(Settings.FIELD_URL_ID).StringValue()), existingDoc);
                    writer.Close();
                    continue;
                }

                Document luceneDocument = IndexUrl(uri, indexes, tweet.TweetId);
                if (luceneDocument != null)
                    writer.AddDocument(luceneDocument);
                writer.Optimize();
                writer.Close();

            }

            //update the date indexed on the tweet
            _tweetIndexer.UpdateDateIndexed(tweet);
        }

        /// <summary>
        /// Updates the list of referencing tweets to include the referencing tweet.
        /// </summary>
        private bool UpdateTweets(Document existingDoc, long tweetId)
        {
            bool wasUpdated = false;

            Field[] tweetsFields = existingDoc.GetFields(Settings.FIELD_URL_TWEETS);

            //get all the current users 
            List<long> tweets = new List<long>();
            foreach (Field field in tweetsFields)
            {
                tweets.Add(long.Parse(field.StringValue()));
            }

            //see if the collcetion contains the current tweet
            if (!tweets.Contains(tweetId))
            {
                //if not, add it 
                Field newIndexField = new Field(Settings.FIELD_URL_TWEETS, tweetId.ToString(), Field.Store.YES, Field.Index.ANALYZED);
                existingDoc.Add(newIndexField);
                wasUpdated = true;
            }

            return wasUpdated;
        }

        /// <summary>
        /// Creates a new document for a certain URL
        /// </summary>
        private Document IndexUrl(Uri uri, IEnumerable<string> indexes, long tweetId)
        {
            //if its not..
            //retrieve HTMl and get it ready for parsing
            WebPageRetriever retriever = new WebPageRetriever(uri);
            string html = retriever.GetHtml();

            if (html == null)
            {
                Console.WriteLine("No HTML returned, continuing");
                return null;
            }

            HtmlParser parser = new HtmlParser(html);

            //retrieve the title from the document 
            string title = parser.FindTitle();

            Console.WriteLine(title);
            //remove the html tags to leave only text.
            string documentText = parser.StripHtml();

            Document luceneDocument = new Document();

            Field textField = new Field(Settings.FIELD_URL_DOCUMENT_TEXT, documentText, Field.Store.YES, Field.Index.ANALYZED);
            Field urlField = new Field(Settings.FIELD_URL_URL, uri.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field titleField = new Field(Settings.FIELD_URL_TITLE, title, Field.Store.YES, Field.Index.ANALYZED);
            Field idField = new Field(Settings.FIELD_URL_ID, Guid.NewGuid().ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
            Field tweetField = new Field(Settings.FIELD_URL_TWEETS, tweetId.ToString(), Field.Store.YES, Field.Index.ANALYZED);

            luceneDocument.Add(textField);
            luceneDocument.Add(titleField);
            luceneDocument.Add(urlField);
            luceneDocument.Add(idField);
            luceneDocument.Add(tweetField);

            foreach (string indexId in indexes)
            {
                Field indexField = new Field(Settings.FIELD_URL_INDEXES, indexId.ToString(), Field.Store.YES,
                                             Field.Index.ANALYZED);
                luceneDocument.Add(indexField);
            }
            return luceneDocument;

        }


        /// <summary>
        /// Updates the indexes 
        /// </summary>
        /// <returns>wether it was updated or not </returns>
        private bool UpdateIndexes(Document existingDoc, IEnumerable<string> newIndexes)
        {
            bool wasUpdated = false;

            Field[] indexesFields = existingDoc.GetFields(Settings.FIELD_URL_INDEXES);

            //get all the current indexes
            List<string> indexes = new List<string>();
            foreach (Field field in indexesFields)
            {
                indexes.Add(field.StringValue());
            }

            foreach (string indexId in newIndexes)
            {
                //see if the indexes contains the current index
                if (!indexes.Contains(indexId))
                {
                    //if not, add it 
                    Field newIndexField = new Field(Settings.FIELD_URL_INDEXES, indexId.ToString(),
                                                    Field.Store.YES, Field.Index.ANALYZED);
                    existingDoc.Add(newIndexField);
                    wasUpdated = true;
                }
            }

            return wasUpdated;
        }
    }
}
