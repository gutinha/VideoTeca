using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using VideoTeca.Models;
using VideoTeca.Services;

namespace VideoTeca.Controllers
{
    public class AvaliadorController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliadorController(IVideoService videoService, IAvaliacaoService avaliacaoService)
        {
            _videoService = videoService;
            _avaliacaoService = avaliacaoService;
        }

        public ActionResult Index()
        {
            ViewBag.Areas = _videoService.GetAllAreas();
            return View();
        }

        public ActionResult DetalhesAvaliacao(int id)
        {
            var video = _videoService.GetVideoById(id);
            return View(video);
        }

        public ActionResult AvaliarVideo(int id)
        {
            var userName = Convert.ToString(Session["nome"]);
            if (_avaliacaoService.IsVideoLockedByAnotherUser(id, userName))
            {
                TempData["e"] = "Este vídeo está sendo editado por outro usuário.";
                return RedirectToAction("Index");
            }

            _avaliacaoService.LockVideo(id, userName);
            var video = _videoService.GetVideoById(id);
            return View(video);

        }
        [HttpPost]
        public ActionResult AvaliarVideo(FormCollection formulario)
        {
            try
            {
                long userId = Convert.ToInt64(Session["id_user"]);
                var justificativa = formulario["justificativa"];
                var videoId = Convert.ToInt64(formulario["id"]);

                _avaliacaoService.SaveAvaliacao(userId, videoId, justificativa);
                TempData["s"] = "Vídeo avaliado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ListarVideosEnviadosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            long userLogado = Convert.ToInt64(Session["id_user"]);
            string userName = Convert.ToString(Session["nome"]);
            int pageSize = limit ?? 10;
            int page = offset ?? 0;
            int totalItems;

            var videos = _avaliacaoService.GetVideosToReview(userLogado, userName, Area, SubArea, search, sort, order, page, pageSize, out totalItems);

            var resultados = videos.Select(x => new
            {
                id = x.Id,
                titulo = x.Titulo,
                id_status = x.AreaNome,
                id_area = x.AreaNome,
                id_subarea = x.SubareaNome
            }).ToList();

            return Json(new { total = totalItems, rows = resultados }, JsonRequestBehavior.AllowGet);
        }
    }


}