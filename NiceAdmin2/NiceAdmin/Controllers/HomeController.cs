using Microsoft.AspNetCore.Mvc;
using NiceAdmin.BAL;
using NiceAdmin.Models;
using System.Diagnostics;

namespace NiceAdmin.Controllers
{
    [CheckAccess]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else            //if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("Index", "SEC_User");
            //}
            //else
                return View();
        }
        public IActionResult formselements()
        {
            return View();
        }
        public IActionResult formslayouts()
        {
            return View();
        }

        public IActionResult formseditors()
        {
            return View();
        }
        public IActionResult formsvalidation ()
        {
            return View();
        }
    }
}
