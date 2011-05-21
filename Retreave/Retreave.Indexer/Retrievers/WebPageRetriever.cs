using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retreave.Indexer.Retrievers
{
    /// <summary>
    /// Retrieves the web page from the web server
    /// </summary>
    class WebPageRetriever
    {
        public Uri PageToRetrieve { get; set; }
        public string Encoding { get; set; }
        public long Length { get; set; }
        public string Content { get; set; }
        public WebPageRetriever(Uri pageToRetrieve)
        {
            this.PageToRetrieve = pageToRetrieve;
        }

        /// <summary>
        /// Retrieves the HTML from the URL
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            DownloadPage();
            return this.Content;
        }

        /// <summary>
        /// Downloads the page
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool DownloadPage()
        {
            bool success = false;
            // Open the requested URL
            string unescapedUri = Regex.Replace(PageToRetrieve.AbsoluteUri, @"&amp;amp;", @"&", RegexOptions.IgnoreCase);
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(unescapedUri);

            req.AllowAutoRedirect = true;
            req.MaximumAutomaticRedirections = 3;
            req.UserAgent = "Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; RetrEave.Com; robot)";
            req.KeepAlive = true;
            req.Timeout = 5 * 1000; 

            // Get the stream from the returned web response
            System.Net.HttpWebResponse webresponse = null;
            try
            {
                webresponse = (System.Net.HttpWebResponse)req.GetResponse();
            }
            catch (System.Net.WebException we)
            {   //remote url not found, 404; remote url forbidden, 403
                //TODO do something here, like log it?
            }
            
            if (webresponse != null)
            {
                success = GetResponse(webresponse);
                webresponse.Close();
            }

            return success;
        }


        /// <summary>
        /// Handles the response from server
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool GetResponse(System.Net.HttpWebResponse response)
        {
            string enc = "utf-8"; // default
            if (response.ContentEncoding != String.Empty)
            {
                // Use the HttpHeader Content-Type in preference to the one set in META
                this.Encoding = response.ContentEncoding;
            }
            else 
            {
                this.Encoding = enc; // default
            }
           
            System.IO.StreamReader stream = new System.IO.StreamReader
                (response.GetResponseStream(), System.Text.Encoding.GetEncoding(this.Encoding));

            //store final URl if it was a redirect
            this.PageToRetrieve = response.ResponseUri; 
            this.Length = response.ContentLength;
            this.Content = stream.ReadToEnd();
            stream.Close();
            return true; //success
        }
    }
}

/*
 * References for this class:
 *  http://www.c-sharpcorner.com/Code/2003/Dec/ReadingWebPageSources.asp
 *  http://www.codeproject.com/KB/IP/Spideroo.aspx
 * 
 */
