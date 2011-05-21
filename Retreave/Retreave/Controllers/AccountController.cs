using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
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
            TwitterService service = new TwitterService(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, oauth_verifier);


            //Store the access token and secret and create a new user account
            TwitterAuthentication authentication = new TwitterAuthentication()
                                                    {
                                                        AccessToken = accessToken.Token,
                                                        AccessTokenSecret = accessToken.TokenSecret
                                                    };


            //Authenticate account with twitter
            service.AuthenticateWith(AuthenticationTokens.TwitterConsumerKey,
                                        AuthenticationTokens.TwitterConsumerSecret,
                                        authentication.AccessToken, authentication.AccessTokenSecret);

            TwitterUser twitterUser = service.VerifyCredentials();

            return RedirectToAction("UserProfile",new {authentication.AccessToken, authentication.AccessTokenSecret, twitterUser.ScreenName});
        }

        /// <summary>
        /// Displays the users profile
        /// </summary>
        public ActionResult UserProfile(TwitterAuthentication authentication, string screenName)
        {
            //Now pass onto controller to see if they need to store or create an account.
            RegisteredUser user = ServiceLayer.UserDetailsService.AuthenticateTwitterAccount(authentication, screenName);

            if (user == null)
                return Content("Login Failed");
            return View(user);
        }

        public ActionResult ViewTweets(string accessToken, string accessTokenSecret)
        {
            TwitterService service2 = new TwitterService(AuthenticationTokens.TwitterConsumerKey, AuthenticationTokens.TwitterConsumerSecret);

            service2.AuthenticateWith(
                AuthenticationTokens.TwitterConsumerKey,
                AuthenticationTokens.TwitterConsumerSecret,
            accessToken, accessTokenSecret);
            TwitterUser user2 = service2.VerifyCredentials();
            var tweets = service2.ListTweetsMentioningMe(5);

            ViewBag.Message = string.Format("Your username is {0}", user2.ScreenName);
            ViewBag.Tweets = tweets;

            return View("AuthorizeCallback");
        }

    }
}
