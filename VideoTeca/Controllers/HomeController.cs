using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;
using VideoTeca.Services;

namespace VideoTeca.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IVideoService _videoService;

        public HomeController(IUserService userService, IVideoService videoService)
        {
            _userService = userService;
            _videoService = videoService;
        }

        public ActionResult Index()
        {
            string queryString = Request.QueryString["dados"];
            if (!string.IsNullOrEmpty(queryString))
            {
                try
                {
                    usuarioDTO dadosOriginais = _userService.DecryptUser(queryString);
                    var userDb = _userService.GetUserByEmail(dadosOriginais.Email);
                    if (userDb != null)
                    {
                        //Authentication here
                    }
                }
                catch (Exception ex)
                {
                    TempData["e"] = "Erro ao decodificar a string base64: " + ex.Message;
                }
            }

            return View();
        }

        public ActionResult ListarVideos(string area = null, string subarea = null, string titulo = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Areas = _videoService.GetAllAreas();

            int totalVideos;
            var videos = _videoService.GetVideos(area, subarea, titulo, page, pageSize, out totalVideos);

            ViewBag.videos = videos;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalVideos / pageSize);

            return View();
        }

        public ActionResult CriarConta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CriarConta(FormCollection formulario)
        {
            var email = formulario["login"];
            var name = formulario["nome"];
            var password = formulario["password"];

            if (_userService.IsUserExists(email))
            {
                TempData["e"] = "Não foi possível cadastrar, Já existe um usuário com esse email";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _userService.CreateUser(name, email, password);

                var usuario = _userService.GetUserByEmail(email);
                Session["id_user"] = usuario.id.ToString();
                Session["nome"] = usuario.nome;
                Session["role"] = usuario.permission.ToString();
                TempData["s"] = "Conta criada com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult BuscarSubArea(int id)
        {
            var subAreas = _videoService.GetSubAreasByAreaId(id)
                                    .Select(sa => new { Id = sa.id, Nome = sa.nome })
                                    .ToList();
            return Json(subAreas, JsonRequestBehavior.AllowGet);
        }

    }
}