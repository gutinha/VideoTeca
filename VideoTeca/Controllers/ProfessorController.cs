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
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        public ActionResult BuscarSubArea(int id)
        {
            var subAreas = db.subarea.Where(x => x.id_area == id).ToList().Select(a => new { Id = a.id, Nome = a.nome });
            return Json(subAreas, JsonRequestBehavior.AllowGet);
        }


    }
}