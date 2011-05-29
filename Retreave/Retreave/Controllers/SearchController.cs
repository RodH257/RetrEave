using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Retreave.Domain.Models;
using Retreave.Domain.Services;
using Retreave.Models;

namespace Retreave.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        [Authorize]
        public ActionResult Index()
        {
            RegisteredUser user = ServiceLayer.UserDetailsService.GetUserByUserName(HttpContext.User.Identity.Name);
            IEnumerable<RetreaveIndex> indexes = ServiceLayer.IndexQueuerService.GetIndexesQueuedByUser(user.UserId);
            NewSearchViewModel viewModel = new NewSearchViewModel() { SearchableIndexes = indexes, User = user };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PerformSearch(string searchText, IEnumerable<int> selectedIndexes)
        {
            //get the indexes
            IList<RetreaveIndex> indexesToSearch = ServiceLayer.IndexQueuerService.GetIndexesByIdList(selectedIndexes).ToList();

            RegisteredUser user = ServiceLayer.UserDetailsService.GetUserByUserName(HttpContext.User.Identity.Name);

            //create a new SearchQuery
            SearchQuery query = new SearchQuery { IndexesToSearch = indexesToSearch, Querier = user, QueryText = searchText };

            ResultSet results;
            using (ISearchEngineService service = ServiceLayer.SearchEngineService)
            {
                results = service.Search(query);
            }
            return View(results);
        }

    }
}
