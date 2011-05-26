using System;
using System.Collections;
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
using Retreave.Indexer.Parsers;
using Retreave.Indexer.Retrievers;
using Retreave.SearchEngine;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace Retreave.Indexer.Indexers
{
    public class TwitterStreamIndexer : IIndexer
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
                    Console.WriteLine("URL" + uri);

                    //setup index writer
                    StandardAnalyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
                    FSDirectory luceneDirectory = FSDirectory.Open(new DirectoryInfo(@"C:\LuceneIndex\"));
                    IndexWriter writer = new IndexWriter(luceneDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);


                    //need to check if its not already indexed
                    //if it is already indexed, then just add a user to the index field in lucene
                    SearchEngineService searchEngine = new SearchEngineService();
                    Document existingDoc = searchEngine.GetDocumentForUrl(uri.ToString());


                    if (existingDoc != null)
                    {
                        //document already exists, add a user to it.
                        Console.WriteLine("Already Exists");

                        Field[] usersFields = existingDoc.GetFields(Settings.FIELD_USERS);

                        //get all the current users 
                        List<string> indexedUsers = new List<string>();
                        foreach (Field field in usersFields)
                        {
                            indexedUsers.Add(field.StringValue());
                            Console.WriteLine(field.StringValue());
                        }

                        bool wasUpdated = false;
                        //add any associated users who aren't in that list 
                        foreach (var user in index.AssociatedUsers)
                        {
                            if (!indexedUsers.Contains(user.UserId.ToString()))
                            {
                                Random random = new Random();
                                Field newUserField = new Field(Settings.FIELD_USERS, random.Next(100).ToString(), Field.Store.YES, Field.Index.ANALYZED);
                                existingDoc.Add(newUserField);
                                wasUpdated = true;
                            }
                        }

                        //only update document if it was changed.
                        if (wasUpdated)
                            writer.UpdateDocument(new Term(Settings.FIELD_ID, existingDoc.GetField(Settings.FIELD_ID).StringValue()), existingDoc);
                        writer.Close();
                        continue;
                    }

                    //if its not..
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

                    Document luceneDocument = new Document();

                    //todo add stemming, tokenizing etc here 
                    Field textField = new Field(Settings.FIELD_DOCUMENT_TEXT, documentText, Field.Store.YES, Field.Index.ANALYZED);
                    Field urlField = new Field(Settings.FIELD_URL, uri.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
                    Field titleField = new Field(Settings.FIELD_TITLE, title, Field.Store.YES, Field.Index.ANALYZED);
                    Field idField = new Field(Settings.FIELD_ID, Guid.NewGuid().ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
                    //add each user as a user field
                    foreach (var user in index.AssociatedUsers)
                    {
                        Field usersField = new Field(Settings.FIELD_USERS, user.UserId.ToString(), Field.Store.YES, Field.Index.ANALYZED);
                        luceneDocument.Add(usersField);
                    }

                    luceneDocument.Add(textField);
                    luceneDocument.Add(titleField);
                    luceneDocument.Add(urlField);
                    luceneDocument.Add(idField);

                    writer.AddDocument(luceneDocument);
                    writer.Optimize();
                    writer.Close();

                }
            }

        }
    }
}
