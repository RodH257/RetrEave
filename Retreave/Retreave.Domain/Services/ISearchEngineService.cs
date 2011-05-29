using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Retreave.Domain.Models;

namespace Retreave.Domain.Services
{
    public interface ISearchEngineService: IDisposable 
    {
        /// <summary>
        /// Gets the results for a specific term
        /// </summary>
        IList<string> GetUrlsForTerm(string term);

        /// <summary>
        /// Finds a document for a URL if there is one.
        /// </summary>
        Document GetDocumentForUrl(string url);

        /// <summary>
        /// Performs a serach on the databse from a query object 
        /// </summary>
        ResultSet Search(SearchQuery searchQuery);

        /// <summary>
        /// Gets the document for a certain tweet id 
        /// </summary>
        Document GetDocumentForTweetId(long tweetId);
    }
}