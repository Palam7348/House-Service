using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HouseServicesSigmaProject.Models;

namespace HouseServicesSigmaProject.Controllers
{
    public class HomeController : Controller
    {

        DatabaseContext db = new DatabaseContext();

        
        public ActionResult Index()
        {

            
           
            ViewBag.Message = "Приветствуем на сайте";

            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Помощь";

            return View();
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Наши контактные данные";

            return View();
        }
    }
}
/*
 [Authorize(Roles = "Admin, Worker")]  
 
 
 
 */