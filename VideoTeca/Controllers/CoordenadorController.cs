using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class CoordenadorController : Controller
    {
        private readonly dbContext db = new dbContext();
        // GET: Coordenador
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ControleUsuarios()
        {
            ViewBag.usuarios = db.usuario.Where(u => u.active == true);
            return View();
        }
    }
}