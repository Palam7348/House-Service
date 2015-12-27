using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HouseServicesSigmaProject.Models;

namespace HouseServicesSigmaProject.Controllers
{
    public class AddController : Controller
    {
        // GET: Add

        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View();
        }



    }
}