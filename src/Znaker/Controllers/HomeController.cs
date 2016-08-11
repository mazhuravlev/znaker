using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

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
    }
}
