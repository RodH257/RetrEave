using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Retreave.Domain;
using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    /// <summary>
    /// Service layer for searching the index
    /// </summary>
    public class SearchEngineService : ISearchEngineService, IDisposable
    {
        private IndexSearcher _searcher;
        private IndexSearcher _tweetSearcher;
        public SearchEngineService()
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(Settings.URL_INDEX_DIR), null);
            _searcher = new IndexSearcher(directory, true);

            FSDirectory tweetDirectory = FSDirectory.Open(new DirectoryInfo(Settings.TWEET_INDEX_DIR), null);
            _tweetSearcher = new IndexSearcher(tweetDirectory, true);
        }

        #region Private Helpers

        /// <summary>
        /// Constructs a standard query on a specified field with a certain term
        /// </summary>
        private Query CreateStandardQuery(string term, string field)
        {
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, field, analyzer);
            return parser.Parse(term);
        }

        /// <summary>
        /// performs a basic query
        /// </summary>
        private TopDocs PerformQuery(Query query)
        {
            Filter filter = new QueryWrapperFilter(query);
            return _searcher.Search(query, filter, 50);
        }

        /// <summary>
        /// Performs a search on the tweet index
        /// </summary>
        private TopDocs PerformTweetQuery(Query query)
        {
            Filter filter = new QueryWrapperFilter(query);
            return _tweetSearcher.Search(query, filter, 50);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the results for a specific term
        /// </summary>
        public IList<string> GetUrlsForTerm(string term)
        {
            List<string> resultsStrings = new List<string>();
            Query query = CreateStandardQuery(term, Settings.FIELD_URL_DOCUMENT_TEXT);
            TopDocs results = PerformQuery(query);

            foreach (var result in results.scoreDocs)
            {
                Document doc = _searcher.Doc(result.doc);
                resultsStrings.Add(doc.Get(Settings.FIELD_URL_URL));
            }

            return resultsStrings;
        }

        /// <summary>
        /// Finds a document for a URL if there is one.
        /// </summary>
        public Document GetDocumentForUrl(string url)
        {
            TermQuery query = new TermQuery(new Term(Settings.FIELD_URL_URL, url));
            TopDocs results = PerformQuery(query);

            if (results.totalHits == 0)
                return null;
            return _searcher.Doc(results.scoreDocs[0].doc);
        }


        /// <summary>
        /// Gets a document for a tweet id if there is one 
        /// </summary>
        public Document GetDocumentForTweetId(long tweetId)
        {
            TermQuery query = new TermQuery(new Term(Settings.FIELD_TWEET_ID, tweetId.ToString()));
            TopDocs results = PerformTweetQuery(query);

            if (results.totalHits == 0)
                return null;
            return _tweetSearcher.Doc(results.scoreDocs[0].doc);
        }

        /// <summary>
        /// Performs a serach on the databse from a query object 
        /// </summary>
        public ResultSet Search(SearchQuery searchQuery)
        {
            ResultSet resultSet = new ResultSet(searchQuery);

            //need to query indexes as well as the term
            BooleanQuery booleanQuery = new BooleanQuery();

            //searches for only terms that are present in the indexes selected.
            string indexSearch = "";
            int count = 0;
            foreach (RetreaveIndex index in searchQuery.IndexesToSearch)
            {
                if (count != 0)
                    indexSearch += " OR ";
                //use the index stream identifier as that is what is pulled from twitter
                indexSearch += index.IndexStreamIdentifier;
                count++;
            }

            Query indexQuery = CreateStandardQuery(indexSearch, Settings.FIELD_URL_INDEXES);
            Query termQuery = CreateStandardQuery(searchQuery.QueryText, Settings.FIELD_URL_DOCUMENT_TEXT);
            booleanQuery.Add(indexQuery, BooleanClause.Occur.MUST);
            booleanQuery.Add(termQuery, BooleanClause.Occur.MUST);

            //do the query
            TopDocs results = PerformQuery(booleanQuery);

            foreach (ScoreDoc topDoc in results.scoreDocs)
            {
                Document doc = _searcher.Doc(topDoc.doc);

                //construct the result
                Result result = new Result();
                result.Title = doc.GetField(Settings.FIELD_URL_TITLE).StringValue();
                result.Url = doc.GetField(Settings.FIELD_URL_URL).StringValue();

                //get the index ids
                List<string> indexIds = new List<string>();
                foreach (Field field in doc.GetFields(Settings.FIELD_URL_INDEXES))
                {
                    indexIds.Add(field.StringValue());
                }

                //query the database for them
                result.OriginatingIndexes = ServiceLayer.IndexQueuerService.GetIndexesByUniqueIdList(indexIds).ToList();
                resultSet.Results.Add(result);
            }


            return resultSet;
        }


        #endregion

        public void Dispose()
        {
            _searcher.Close();
        }
    }
}
