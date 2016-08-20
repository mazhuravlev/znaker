using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Znaker.Migrations;

namespace Znaker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataAccessProvider _db;

        public HomeController(IDataAccessProvider db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            _db.AddTestEntity(new TestEntity
            {
                Text = new Random().Next(1, 9999999).ToString()
            });

            ViewBag.db = _db.CountTestEntities();

            ViewBag.test = "Call to action!!!!!!!";
            return View();
        }

        public IActionResult Phone()
        {
            var phone = new Phone
            {
                Id = Convert.ToInt64(new Random().Next(1, 09999999)),
                CreatedAt = new DateTime()
            };
            _db.AddPhone(phone);
            var count = _db.CountPhones();
            Console.WriteLine($"Phones count is {count}");
            ViewBag.phone = phone.Id;

            return View();
        }
    }
}
