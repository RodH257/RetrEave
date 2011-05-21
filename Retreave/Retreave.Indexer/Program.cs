using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
