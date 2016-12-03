using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elena.Constants.OrderController;
using Elena.Models;
using Elena.Services;
using Elena.Settings;
using Elena.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Elena.Controllers
{
    public class OrderController : Controller
    {
        private readonly ElenaDbContext _db;
        //private readonly IOptions<AppSettings> _appSettings;
        //private readonly IBrowserConfigService _browserConfigService;
        //private readonly IManifestService _manifestService;
        //private readonly IOpenSearchService _openSearchService;
        //private readonly IRobotsService _robotsService;

        public OrderController(
            //IBrowserConfigService browserConfigService,
            //IManifestService manifestService,
            //IOpenSearchService openSearchService,
            //IRobotsService robotsService,
            //IOptions<AppSettings> appSettings,
            ElenaDbContext db)
        {
            _db = db;
            //_appSettings = appSettings;
            //_browserConfigService = browserConfigService;
            //_manifestService = manifestService;
            //_openSearchService = openSearchService;
            //_robotsService = robotsService;
        }

        [HttpGet("", Name = OrderControllerRoute.GetIndex)]
        public IActionResult Index()
        {
            return View(OrderControllerAction.Index,
                model: new MakeOrderViewModel());
        }

        [HttpPost("submit", Name = OrderControllerRoute.PostSubmit)]
        public IActionResult Submit([FromForm] MakeOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //???
            }


            var address = new Address
            {
                Streetname = model.Streetname,
                Number = model.Number,
                PostalCode = model.PostalCode,
                City = model.City,
                Country = model.Country
            };
            var customer = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Address = address,
                //VatNumber = 
            };
            var order = new Order
            {
                Date = DateTime.UtcNow,
                Customer = customer,
                Products = model.Products,
                Status = OrderStatus.Placed
            };

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Addresses.Add(address);
                    _db.Customers.Add(customer);
                    _db.Orders.Add(order);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return Redirect(OrderControllerRoute.GetFail);
                }
            }

            return Redirect(OrderControllerRoute.GetSuccess);
        }

        [HttpGet("success", Name = OrderControllerRoute.GetSuccess)]
        public IActionResult OrderPlaced()
        {
            return View();
        }

        [HttpGet("fail", Name = OrderControllerRoute.GetFail)]
        public IActionResult OrderFailed()
        {
            return View();
        }
    }
}
