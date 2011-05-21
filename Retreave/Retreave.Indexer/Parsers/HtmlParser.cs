using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retreave.Indexer.Parsers
{
    /// <summary>
    /// Parses a HTML file, finding the title and removing html tags 
    /// so that it is ready to be a document
    /// </summary>
    public class HtmlParser
    {
        private string _document;
        public HtmlParser(string document)
        {
            this._document = document;
        }

        /// <summary>
        /// Finds the title in the document
        /// </summary>
        /// <returns>The title</returns>
        public string FindTitle()
        {
            string title = Regex.Match(_document, @"(?<=<s*title(?:\s[^>]*)?\>)[\s\S]*?(?=\</\s*title(?:\s[^>]*)?\>)"
                                      , RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Value;
            title = title.Trim();
            return title;
        }

        /// <summary>
        /// Strips HTML from the document 
        /// See References below for origins of regex
        /// Will alter the document
        /// </summary>
        /// <returns>the modified document</returns>
        public string StripHtml()
        {
            //Strips the <script> tags from the Html
            string scriptregex = @"<scr" + @"ipt[^>.]*>[\s\S]*?</sc" + @"ript>";
            Regex scripts = new Regex(scriptregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            _document = scripts.Replace(_document, " ");

            //Strips the <style> tags from the Html
            string styleregex = @"<style[^>.]*>[\s\S]*?</style>";
            Regex styles = new Regex(styleregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            _document = styles.Replace(_document, " ");

            //Strips the <!--comment--> tags from the Html	
            string commentregex = @"<!(?:--[\s\S]*?--\s*)?>";
            Regex comments = new Regex(commentregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            _document = comments.Replace(_document, " ");

            //Strips the HTML tags from the Html
            Regex objRegExp = new Regex("<(.|\n)+?>", RegexOptions.IgnoreCase);

            //Replace all HTML tag matches with the empty string
            _document = objRegExp.Replace(_document, " ");

            //Replace all _remaining_ < and > with &lt; and &gt;
            _document = _document.Replace("<", "&lt;");
            _document = _document.Replace(">", "&gt;");

            //replace new lines with spaces 
            _document = _document.Replace(Environment.NewLine, " ");
            

            return _document;
        }
    }

    /* References For this Document 
     * 
     * Much influence from Searcharoo, which is an open source example search engine built 
     * to demonstrate various searching functions in C#.
     * http://searcharoo.net 
     * 
     * Regexes for stripping HTML
     * http://www.4guysfromrolla.com/webtech/042501-1.shtml
     * 
     * Using regex to find tags without a trailing slash
     * http://concepts.waetech.com/unclosed_tags/index.cfm
     *         
     * http://msdn.microsoft.com/library/en-us/script56/html/js56jsgrpregexpsyntax.asp
     *   
     * Replace html comment tags
     * http://www.faqts.com/knowledge_base/view.phtml/aid/21761/fid/53
     * 
     */
}
