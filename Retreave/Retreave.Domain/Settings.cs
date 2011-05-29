using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retreave.Domain
{
    public class Settings
    {
        public const string LUCENE_DIR = @"C:\LuceneIndex";
        public const string TWEET_INDEX_DIR = @"C:\LuceneIndex\TweetIndex\";
        public const string URL_INDEX_DIR = @"C:\LuceneIndex\UrlIndex\";


        public const string FIELD_URL_DOCUMENT_TEXT = "text";
        public const string FIELD_URL_URL = "url";
        public const string FIELD_URL_TITLE = "title";
        public const string FIELD_URL_ID = "guid";
        public const string FIELD_URL_INDEXES = "indexes";
        public const string FIELD_URL_TWEETS = "tweets";

        public const string FIELD_TWEET_TEXT = "text";
        public const string FIELD_TWEET_ID = "tweet_id";
        public const string FIELD_TWEET_LINKER_NAME = "linker_name";
        public const string FIELD_TWEET_LINKER_ID = "linker_id";
        public const string FIELD_TWEET_LINKER_REP = "linker_rep";
        public const string FIELD_TWEET_DATE_POSTED = "date_posted";
        public const string FIELD_TWEET_DATE_INDEXED = "date_updated";
        public const string FIELD_TWEET_INDEXES = "indexes";

    }
}
