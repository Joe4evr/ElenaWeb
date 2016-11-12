using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.AspNetCore;
using Boilerplate.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Elena.Constants;
using Elena.Services;
using Elena.Settings;
using Elena.ViewModels;
using Elena.Models;

namespace Elena.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private readonly ElenaDbContext _db;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IBrowserConfigService _browserConfigService;
#if NET461
        // The FeedService is not available for .NET Core because the System.ServiceModel.Syndication.SyndicationFeed
        // type does not yet exist. See https://github.com/dotnet/wcf/issues/76.
        private readonly IFeedService _feedService;
#endif
        private readonly IManifestService _manifestService;
        private readonly IOpenSearchService _openSearchService;
        private readonly IRobotsService _robotsService;

        #endregion

        #region Constructors

        public HomeController(
            ElenaDbContext db,
            IBrowserConfigService browserConfigService,
#if NET461
            // The FeedService is not available for .NET Core because the System.ServiceModel.Syndication.SyndicationFeed
            // type does not yet exist. See https://github.com/dotnet/wcf/issues/76.
            IFeedService feedService,
#endif
            IManifestService manifestService,
            IOpenSearchService openSearchService,
            IRobotsService robotsService,
            IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings;
            _browserConfigService = browserConfigService;
#if NET461
            // The FeedService is not available for .NET Core because the System.ServiceModel.Syndication.SyndicationFeed
            // type does not yet exist. See https://github.com/dotnet/wcf/issues/76.
            _feedService = feedService;
#endif
            _manifestService = manifestService;
            _openSearchService = openSearchService;
            _robotsService = robotsService;
        }

        #endregion

        [HttpGet("", Name = HomeControllerRoute.GetIndex)]
        public IActionResult Index()
        {
            return View(HomeControllerAction.Index);
        }

        //[NoTrailingSlash]
        [HttpGet("gallery/{page?}", Name = HomeControllerRoute.GetGallery)]
        public IActionResult Products(int page = 0)
        {
            var products = _db.Products.Where(p => p.Type == ProductType.Painting).ToList();
            ViewBag.Title = "Gallery";
            return base.View(viewName: HomeControllerAction.Products,
                model: new ProductsViewModel(products.Skip(20 * page).Take(20), products.Count));
        }

        //[NoTrailingSlash]
        [HttpGet("cakes/{page?}", Name = HomeControllerRoute.GetCakes)]
        public IActionResult Cakes(int page = 0)
        {
            var products = _db.Products.Where(p => p.Type == ProductType.Cake).ToList();
            ViewBag.Title = "Cakes";
            return base.View(viewName: HomeControllerAction.Products,
                model: new ProductsViewModel(products.Skip(20 * page).Take(20), products.Count));
        }

        //[NoTrailingSlash]
        [HttpGet("detail/{id}", Name = HomeControllerRoute.GetDetail)]
        public IActionResult Detail(int id)
        {
            var product = _db.Products.Single(p => p.Id == id);
            return base.View(viewName: HomeControllerAction.Detail,
                model: new DetailViewModel(product));
        }

        /// <summary>
        /// Gets the Atom 1.0 feed for the current site. Note that Atom 1.0 is used over RSS 2.0 because Atom 1.0 is a
        /// newer and more well defined format. Atom 1.0 is a standard and RSS is not. See
        /// http://rehansaeed.com/building-rssatom-feeds-for-asp-net-mvc/
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> signifying if the request is cancelled.
        /// See http://www.davepaquette.com/archive/2015/07/19/cancelling-long-running-queries-in-asp-net-mvc-and-web-api.aspx</param>
        /// <returns>The Atom 1.0 feed for the current site.</returns>
        [ResponseCache(CacheProfileName = CacheProfileName.Feed)]
        [Route("feed", Name = HomeControllerRoute.GetFeed)]
#if NET461
        public async Task<IActionResult> Feed(CancellationToken cancellationToken)
        {
            return new AtomActionResult(await _feedService.GetFeed(cancellationToken));
        }
#else
        public IActionResult Feed(CancellationToken cancellationToken)
        {
            return Ok("The FeedService is not available for .NET Core because the System.ServiceModel.Syndication.SyndicationFeed type does not yet exist. See https://github.com/dotnet/wcf/issues/76.");
        }
#endif

        [Route("search", Name = HomeControllerRoute.GetSearch)]
        public IActionResult Search(string query)
        {
            // You can implement a proper search function here and add a Search.cshtml page.
            // return this.View(HomeControllerAction.Search);

            // Or you could use Google Custom Search (https://cse.google.co.uk/cse) to index your site and display your
            // search results in your own page.

            // For simplicity we are just assuming your site is indexed on Google and redirecting to it.
            return Redirect(string.Format(
                "https://www.google.co.uk/?q=site:{0} {1}",
                Url.AbsoluteRouteUrl(HomeControllerRoute.GetIndex),
                query));
        }

        /// <summary>
        /// Gets the browserconfig XML for the current site. This allows you to customize the tile, when a user pins
        /// the site to their Windows 8/10 start screen. See http://www.buildmypinnedsite.com and
        /// https://msdn.microsoft.com/en-us/library/dn320426%28v=vs.85%29.aspx
        /// </summary>
        /// <returns>The browserconfig XML for the current site.</returns>
        [NoTrailingSlash]
        [ResponseCache(CacheProfileName = CacheProfileName.BrowserConfigXml)]
        [Route("browserconfig.xml", Name = HomeControllerRoute.GetBrowserConfigXml)]
        public ContentResult BrowserConfigXml()
        {
            string content = _browserConfigService.GetBrowserConfigXml();
            return Content(content, ContentType.Xml, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the manifest JSON for the current site. This allows you to customize the icon and other browser
        /// settings for Chrome/Android and FireFox (FireFox support is coming). See https://w3c.github.io/manifest/
        /// for the official W3C specification. See http://html5doctor.com/web-manifest-specification/ for more
        /// information. See https://developer.chrome.com/multidevice/android/installtohomescreen for Chrome's
        /// implementation.
        /// </summary>
        /// <returns>The manifest JSON for the current site.</returns>
        [NoTrailingSlash]
        [ResponseCache(CacheProfileName = CacheProfileName.ManifestJson)]
        [Route("manifest.json", Name = HomeControllerRoute.GetManifestJson)]
        public ContentResult ManifestJson()
        {
            string content = _manifestService.GetManifestJson();
            return Content(content, ContentType.Json, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the Open Search XML for the current site. You can customize the contents of this XML here. The open
        /// search action is cached for one day, adjust this time to whatever you require. See
        /// http://www.hanselman.com/blog/CommentView.aspx?guid=50cc95b1-c043-451f-9bc2-696dc564766d
        /// http://www.opensearch.org
        /// </summary>
        /// <returns>The Open Search XML for the current site.</returns>
        [NoTrailingSlash]
        [ResponseCache(CacheProfileName = CacheProfileName.OpenSearchXml)]
        [Route("opensearch.xml", Name = HomeControllerRoute.GetOpenSearchXml)]
        public IActionResult OpenSearchXml()
        {
            string content = _openSearchService.GetOpenSearchXml();
            return Content(content, ContentType.Xml, Encoding.UTF8);
        }

        /// <summary>
        /// Tells search engines (or robots) how to index your site.
        /// The reason for dynamically generating this code is to enable generation of the full absolute sitemap URL
        /// and also to give you added flexibility in case you want to disallow search engines from certain paths. The
        /// sitemap is cached for one day, adjust this time to whatever you require. See
        /// http://rehansaeed.com/dynamically-generating-robots-txt-using-asp-net-mvc/
        /// </summary>
        /// <returns>The robots text for the current site.</returns>
        [NoTrailingSlash]
        [ResponseCache(CacheProfileName = CacheProfileName.RobotsText)]
        [Route("robots.txt", Name = HomeControllerRoute.GetRobotsText)]
        public IActionResult RobotsText()
        {
            string content = _robotsService.GetRobotsText();
            return Content(content, ContentType.Text, Encoding.UTF8);
        }
    }
}