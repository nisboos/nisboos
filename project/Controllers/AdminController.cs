using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using project.Models;

namespace project.Controllers
{
    public class AdminController : Controller
    {
        projectEntities db = new projectEntities();
        // GET: Admin
        public ActionResult Index()
        {
            var products = db.tProduct.ToList();
            return View("Index", "_LayoutAdmin", products);
        }
        public ActionResult Index1()
        {
            var products = db.tProduct.ToList();
            return View("Index", "_LayoutAdmin", products);
        }
    }
}