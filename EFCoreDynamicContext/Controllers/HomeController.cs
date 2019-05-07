using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EFCoreDynamicContext.Models;
using EFCoreDynamicContext.BusinessLayer;

namespace EFCoreDynamicContext.Controllers
{
    public class HomeController : Controller
    {
        public IUserService Users { get; }

        public HomeController(IUserService users)
        {
            Users = users;
        }

        public IActionResult Index()
        {
            ViewBag.UserList = Users.GetUsers();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
