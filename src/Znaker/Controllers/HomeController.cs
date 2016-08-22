using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace Znaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostgreSqlContext _db;

        public HomeController(PostgreSqlContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.test = "Call to action!!!!!!!";
            return View();
        }

        public IActionResult Phone()
        {
            /*
             var phone = new User
            {
                CreatedAt = new DateTime()
            };
            _db.Phones.Add(phone);
            _db.SaveChanges();
            var count = _db.Phones.Count();
            Console.WriteLine($"Phones count is {count}");
            ViewBag.phone = phone.Id;*/

            return View();
        }
    }
}
