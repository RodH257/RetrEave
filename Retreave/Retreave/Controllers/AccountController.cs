using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
using Retreave.Models;
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


            ServiceLayer.UserDetailsService.AuthenticateTwitterAccount(authentication, twitterUser.ScreenName,
                                                                       twitterUser.Id);

            //store the credentials in forms auth? 
            var authTicket = new FormsAuthenticationTicket(1, twitterUser.ScreenName, DateTime.Now,
                                                 DateTime.Now.AddMonths(6), true, "");

            string cookieContents = FormsAuthentication.Encrypt(authTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
            {
                Expires = authTicket.Expiration,
                Path = FormsAuthentication.FormsCookiePath
            };

            if (HttpContext != null)
            {
                HttpContext.Response.Cookies.Add(cookie);
            }

            //new {authentication.AccessToken, authentication.AccessTokenSecret, twitterUser.ScreenName}
            return RedirectToAction("UserProfile");
        }

        /// <summary>
        /// Displays the users profile
        /// </summary>
        [Authorize]
        public ActionResult UserProfile()
        {
            //Now pass onto controller to see if they need to store or create an account.
            RegisteredUser user =
                ServiceLayer.UserDetailsService.GetUserByUserName(HttpContext.User.Identity.Name);


            if (user == null)
                return RedirectToAction("Index");

            IEnumerable<RetreaveIndex> indexes =  ServiceLayer.IndexQueuerService.GetIndexesQueuedByUser(user.UserId);

            UserProfileViewModel viewModel = new UserProfileViewModel() {Indexes = indexes, User = user};
            

            return View(viewModel);
        }


    }
}
