using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class ProfessorController : Controller
    {
        public dbContext db = new dbContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}