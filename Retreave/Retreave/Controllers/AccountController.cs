using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;
using Retreave.Domain.Helpers;

namespace Retreave.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authorize()
        {
            // Step 1 - Retrieve an OAuth Request Token
            TwitterService service = new TwitterService(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret);
            OAuthRequestToken requestToken = service.GetRequestToken("http://localhost:63516/Account/AuthorizeCallback"); // <-- The registered callback URL

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthenticationUrl(requestToken);
            return new RedirectResult(uri.ToString(), false /*permanent*/);
        }


        // This URL is registered as the application's callback at http://dev.twitter.com
        public ActionResult AuthorizeCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken { Token = oauth_token };

            // Step 3 - Exchange the Request Token for an Access Token
            TwitterService service = new TwitterService(AuthenticationTokens.TwitterConsumerKey,AuthenticationTokens.TwitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);
            

            //Store the access token and secret and create a new user account

            //Queue the users account tweets for indexing



            //store the access token and access token secret

            return RedirectToAction("ViewTweets", new { accessToken = accessToken.Token, accessTokenSecret = accessToken.TokenSecret });

            // Step 4 - User authenticates using the Access Token
            //service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
          //  TwitterUser user = service.VerifyCredentials();
            
          //  ViewBag.Message = string.Format("Your username is {0}", user.ScreenName);
            

            return View();
        }

        public ActionResult ViewTweets(string accessToken, string accessTokenSecret)
        {
            TwitterService service2 = new TwitterService(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret);

            service2.AuthenticateWith(
                AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret,
            accessToken, accessTokenSecret);
            TwitterUser user2 = service2.VerifyCredentials();
            var tweets = service2.ListTweetsMentioningMe(5);
            
            ViewBag.Message = string.Format("Your username is {0}", user2.ScreenName);
            ViewBag.Tweets = tweets;
            
            return View("AuthorizeCallback");
        }

    }
}
