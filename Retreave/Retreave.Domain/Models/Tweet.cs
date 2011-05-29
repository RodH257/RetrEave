using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retreave.Domain.Models
{
    /// <summary>
    /// Represents a Tweet in our system.
    /// </summary>
    public class Tweet
    {
        public const string REGEX_FOR_URLS =
            @"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Z0-9+&@#/%=~_|$?!:,.]*[A-Z0-9+&@#/%=~_|$]";

        public Linker Author { get; set; }
        public  string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public int ReTweetCount { get; set; }
        public long TweetId { get; set; }

        public override string ToString()
        {
            return "Tweet by " + Author.Name + " on " + DatePosted.ToShortDateString() + " content: " + Content;
        }

        public bool ContainsUrl()
        {
            return Regex.IsMatch(Content, REGEX_FOR_URLS, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Gets all the URLs present in a certain tweet
        /// </summary>
        /// <returns>list of string URLs</returns>
        public IList<Uri> GetUrlsFromTweet()
        {
            IList<Uri> urls = new List<Uri>();

            foreach (Match match in Regex.Matches(Content, REGEX_FOR_URLS,RegexOptions.IgnoreCase))
            {
                Uri uri;
                if (Uri.TryCreate(match.Value, UriKind.Absolute, out uri))
                {
                    urls.Add(uri);
                }
            }
            return urls;
        }
        
        /// <summary>
        /// Parsing code from 
        /// http://blogs.msdn.com/b/bursteg/archive/2009/05/29/twitter-api-from-c-getting-a-user-s-time-line.aspx
        /// </summary>
        public static DateTime GetDateTimeFromTwitterFormat(string date)
        {
            string dayOfWeek = date.Substring(0, 3).Trim();
            string month = date.Substring(4, 3).Trim();
            string dayInMonth = date.Substring(8, 2).Trim();
            string time = date.Substring(11, 9).Trim();
            string offset = date.Substring(20, 5).Trim();
            string year = date.Substring(25, 5).Trim();
            string dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            DateTime ret = DateTime.Parse(dateTime);
            return ret;
        }

        
    }
}
