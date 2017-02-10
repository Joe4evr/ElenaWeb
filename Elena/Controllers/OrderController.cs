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
        private const string SessionKey = "productids";
        private readonly ElenaDbContext _db;

        public OrderController(ElenaDbContext db)
        {
            _db = db;
        }

        [HttpGet("order", Name = OrderControllerRoute.GetIndex)]
        public IActionResult Index()
        {
            return View(OrderControllerAction.Index,
                model: new MakeOrderViewModel(GetProducts()));
        }

        [HttpPost("submit", Name = OrderControllerRoute.PostSubmit)]
        public IActionResult Submit([FromForm] MakeOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(OrderControllerAction.Index,
                    model: new MakeOrderViewModel(GetProducts()));
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

        private IEnumerable<Product> GetProducts()
        {
            byte[] productIds;
            var products = new List<Product>();
            if (HttpContext.Session.TryGetValue(SessionKey, out productIds))
            {
                for (int i = 0; i < productIds.Length; i += 4)
                {
                    int id = BitConverter.ToInt32(productIds, i);
                    var a = _db.Products.SingleOrDefault(p => p.Id == id && p.IsAvailable);
                    if (a != null)
                    {
                        products.Add(a);
                    }
                }
            }

            return products;
        }

        public int AddId(int id)
        {
            byte[] v;
            if (!HttpContext.Session.TryGetValue(SessionKey, out v))
            {
                v = v.Concat(BitConverter.GetBytes(id)).ToArray();
            }
            else
            {
                v = BitConverter.GetBytes(id);
            }
            HttpContext.Session.Set(SessionKey, v);
            return v.Length / 4;
        }
    }
}
