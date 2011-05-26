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

namespace Retreave.SearchEngine
{
    /// <summary>
    /// Service layer for searching the index
    /// </summary>
    public class SearchEngineService
    {
        private IndexSearcher _searcher;

        public SearchEngineService()
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(@"C:\LuceneIndex"), null);
            _searcher= new IndexSearcher(directory, true);
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
            return _searcher.Search(query, filter, 10);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the results for a specific term
        /// </summary>
        public IList<string> GetUrlsForTerm(string term)
        {
            List<string> resultsStrings = new List<string>();
            Query query = CreateStandardQuery(term, Settings.FIELD_DOCUMENT_TEXT);
            TopDocs results = PerformQuery(query);

            foreach (var result in results.scoreDocs)
            {
                Document doc = _searcher.Doc(result.doc);
                resultsStrings.Add(doc.Get(Settings.FIELD_URL));
            }

            return resultsStrings;
        }

        /// <summary>
        /// Finds a document for a URL if there is one.
        /// </summary>
        public Document GetDocumentForUrl(string url)
        {
            TermQuery query = new TermQuery(new Term(Settings.FIELD_URL, url));

            TopDocs results = PerformQuery(query);

            if (results.totalHits == 0)
                return null;

            return _searcher.Doc(results.scoreDocs[0].doc);
        }

        #endregion

    }
}
