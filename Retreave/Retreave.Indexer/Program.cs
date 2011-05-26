using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Retreave.Domain.Enums;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
using Retreave.Indexer.Indexers;


namespace Retreave.Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            //initialize index

            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            FSDirectory luceneDirectory = FSDirectory.Open(new DirectoryInfo(@"C:\LuceneIndex\"));

            IndexWriter writer = new IndexWriter(luceneDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.Close();

            RetreaveIndex index = ServiceLayer.IndexQueuerService.GetNextIndexToProcess();
            if (index.IndexType == IndexType.TwitterStreamIndex)
            {
                TwitterStreamIndexer indexer = new TwitterStreamIndexer();
                indexer.Index(index);
            }

            Console.Read();
        }
    }
}
