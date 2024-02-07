using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class HomeController : Controller
    {
        public dbContext db = new dbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ListarVideos()
        {
           ViewBag.videos = db.video.Where(x=> x.active == true).ToList();
           return View();
        }
    }
}