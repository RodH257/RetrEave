using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Retreave.Domain;
using Retreave.Domain.Enums;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
using Retreave.Indexer.Indexers;
using Retreave.Indexer.Processors;
using Retreave.Indexer.Retrievers;


namespace Retreave.Indexer
{
    class Program
    {
        static bool _running = true;
        static void Main(string[] args)
        {
            //initialize the indexes in case they haven't been made yet
            InitializeIndexes();


            //indexer thread
            Thread indexerThread = new Thread(() =>
                                           {
                                               while (_running)
                                               {
                                                   RecentTweetsProcessor processor = new RecentTweetsProcessor();
                                                   StreamQueuedTweetsProcessor streamProcessor = new StreamQueuedTweetsProcessor();

                                                   //process the new indexes
                                                   RetreaveIndex newIndex =
                                                       ServiceLayer.IndexQueuerService.GetNextIndexToProcess();
                                                 
                                                   if (newIndex != null)
                                                   {
                                                       //do the initial indexing of the previous tweets for convenience. 
                                                       if (newIndex.IndexType == IndexType.TwitterStreamIndex)
                                                       {
                                                           Console.WriteLine("Processing new index name " +
                                                                             newIndex.Name);
                                                           processor.Process(newIndex);
                                                       }
                                                   }

                                                   // read through the tweets queued by the streamer
                                                   streamProcessor.ProcessNextInQueue();

                                                   Thread.Sleep(1 * 1000);
                                               }
                                           });



            Thread streamerThread = new Thread(() =>
                                          {
                                              StreamingTwitterFeedRetriever streamer = new StreamingTwitterFeedRetriever();
                                              //get users tweets to stream

                                              List<RetreaveIndex> streamingIndexes = new List<RetreaveIndex>();
                                              while (_running)
                                              {
                                                  //check for new feeds to stream
                                                  List<RetreaveIndex> indexesFromDatabase = ServiceLayer.IndexQueuerService.GetUserIndexesToStream().ToList();

                                                  if (indexesFromDatabase.Count > streamingIndexes.Count)
                                                  {
                                                      streamingIndexes = indexesFromDatabase;
                                                      //if found one, start a new  stream and stop old one
                                                      streamer.Running = false;
                                                      streamer.StreamUsersTweets(streamingIndexes);
                                                  }
                                              }
                                              streamer.Running = false;
                                          });

            indexerThread.Start();
            streamerThread.Start();

            Console.WriteLine("Indexer Running.. Press any key to exit");
            Console.ReadLine();

            _running = false;

        }

        private static void InitializeIndexes()
        {
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            FSDirectory tweetDirectory = FSDirectory.Open(new DirectoryInfo(Settings.TWEET_INDEX_DIR));
            FSDirectory urlDirectory = FSDirectory.Open(new DirectoryInfo(Settings.URL_INDEX_DIR));

            IndexWriter writer = new IndexWriter(tweetDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.Close();

            IndexWriter urlWriter = new IndexWriter(urlDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            urlWriter.Close();

        }
    }
}
