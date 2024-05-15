using System;
using System.Linq;
using System.Web.Mvc;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class HomeController : Controller
    {
        private readonly dbContext db = new dbContext();

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
            ViewBag.Areas = db.area.ToList();
            ViewBag.videos = db.video.Where(x => x.active == true && x.aprovado == true).OrderByDescending(x => x.enviadoEm).Take(10).ToList();
           return View();
        }

        public ActionResult CriarConta()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection formulario)
        {
            var login = formulario["login"];
            var password = formulario["password"];
            password = Util.hash(password);
            var usuario = db.usuario.Where(u => u.email.Equals(login) && u.password.Equals(password)).FirstOrDefault();

            if (usuario == null)
            {
                TempData["e"] = "Email ou senha incorretos!";
                return RedirectToAction("Login", "Home");
            }

            Session["id_user"] = usuario.id.ToString();
            TempData["s"] = "Login realizado com sucesso!";
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public ActionResult CriarConta(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var login = formulario["login"];
                    var usuario = db.usuario.Where(u => u.email.Equals(login)).FirstOrDefault();

                    if (usuario != null)
                    {
                        TempData["e"] = "Não foi possível cadastrar, Já existe um usuário com esse email";
                        return RedirectToAction("Index", "Home");
                    }

                    usuario user = new usuario
                    {
                        nome = formulario["nome"],
                        email = formulario["login"],
                        password = Util.hash(formulario["password"]),
                        permission = 1,
                        active = true
                    };

                    db.usuario.Add(user);
                    db.SaveChanges();
                    Session["id_user"] = user.id.ToString();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["s"] = "Conta criada com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult BuscarSubArea(int id)
        {
            var subAreas = db.subarea.Where(x => x.id_area == id).ToList().Select(a => new { Id = a.id, Nome = a.nome });
            return Json(subAreas, JsonRequestBehavior.AllowGet);
        }

    }
}