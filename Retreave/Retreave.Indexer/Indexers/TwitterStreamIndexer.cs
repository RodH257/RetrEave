using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retreave.Domain.Models;
using Retreave.Indexer.Parsers;
using Retreave.Indexer.Retrievers;

namespace Retreave.Indexer.Indexers
{
   public class TwitterStreamIndexer: IIndexer
   {
       /// <summary>
       /// Perform the indexing on a twitter stream
       /// </summary>
       /// <param name="index">the stream to index</param>
       public void Index(RetreaveIndex index)
       {
           //get the users tweets
           TwitterStreamRetriever retreaver = new TwitterStreamRetriever(index.AssociatedUsers.First().AuthDetails);
           IList<Tweet> tweets = retreaver.GetAllTweets();

           IList<Tweet> tweetsWithUrls = new List<Tweet>();
           //find the ones with URLs
           foreach (Tweet tweet in tweets)
           {
            //   Console.WriteLine(tweet.ToString());
               if (tweet.ContainsUrl())
                   tweetsWithUrls.Add(tweet);
           }

           Console.WriteLine("Found " + tweetsWithUrls.Count + " tweets out of " + tweets.Count + " Have URLs");
           //foreach one, retrieve the HTML);
           foreach (Tweet tweetWithUrl in tweetsWithUrls)
           {
               foreach (Uri uri in tweetWithUrl.GetUrlsFromTweet())
               {

                   //retrieve HTMl and get it ready for parsing
                   WebPageRetriever retriever = new WebPageRetriever(uri);
                   string html = retriever.GetHtml();

                   if (html == null)
                   {
                       Console.WriteLine("No HTML returned, continuing");
                       continue;
                   }
                   HtmlParser parser = new HtmlParser(html);
                   
                   //retrieve the title from the document 
                   string title = parser.FindTitle();

                   Console.WriteLine(title);
                   //remove the html tags to leave only text.
                   string documentText = parser.StripHtml();
                   //send it to lucene as a document 
                   Console.WriteLine(documentText.Substring(0, 500));
               }
           }

       }
   }
}
