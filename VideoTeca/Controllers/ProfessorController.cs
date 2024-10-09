using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;
using VideoTeca.Services;

namespace VideoTeca.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly dbContext db = new dbContext();
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;

        public ProfessorController(IVideoService videoService, IUserService userService)
        {
            _videoService = videoService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            ViewBag.Areas = _videoService.GetAllAreas();
            return View();
        }

        public ActionResult EnviarVideo()
        {
            ViewBag.Areas = _videoService.GetAllAreas();
            long userLogado = Convert.ToInt64(Session["id_user"]);
            bool termos = _userService.GetUserTerms(userLogado);
            ViewBag.Termos = termos;
            return View();
        }

        [HttpPost]
        public ActionResult EnviarVideo(VideoViewModel videoViewModel)
        {
            try
            {
                long userId = Convert.ToInt64(Session["id_user"]);
                _videoService.CreateVideo(videoViewModel, userId);
                TempData["s"] = "Vídeo enviado com sucesso! Aguarde um avaliador aprovar seu vídeo.";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.Message;
                return RedirectToAction("Index", "Professor");
            }

            return RedirectToAction("Index", "Professor");
        }

        public ActionResult BuscarSubArea(int id)
        {
            //var subAreas = db.subarea.Where(x => x.id_area == id).ToList().Select(a => new { Id = a.id, Nome = a.nome });
            var subAreas = db.area
            .Where(a => a.id == id)
            .SelectMany(a => a.subarea)
            .Select(sa => new
            {
                Id = sa.id,
                Nome = sa.nome
            })
            .ToList();
            return Json(subAreas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarVideosEnviadosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            long userLogado = Convert.ToInt64(Session["id_user"]);
            IQueryable<video> videos = db.video.Where(v => v.enviadoPor == userLogado);

            //Filtro por área
            if (Area != null && Area != 0)
            {
                videos = videos.Where(v => v.id_area == Area);
            }

            //Filtro por subÁrea
            if (SubArea != null && SubArea != 0)
            {
                videos = videos.Where(v => v.id_subarea == SubArea);
            }

            int quantidade = limit ?? 10;
            int pagina = offset ?? 0;

            switch (sort)
            {
                case "titulo":
                    if (order.Equals("asc"))
                    {
                        videos = videos.OrderBy(x => x.titulo);
                    }
                    else
                    {
                        videos = videos.OrderByDescending(x => x.titulo);
                    }
                    break;

                case "id_status":
                    if (order.Equals("asc"))
                    {
                        videos = videos.OrderBy(x => x.status.nome);
                    }
                    else
                    {
                        videos = videos.OrderByDescending(x => x.status.nome);
                    }
                    break;

                case "id_area":
                    if (order.Equals("asc"))
                    {
                        videos = videos.OrderBy(x => x.area.nome);
                    }
                    else
                    {
                        videos = videos.OrderByDescending(x => x.area.nome);
                    }
                    break;

                case "id_subarea":
                    if (order.Equals("asc"))
                    {
                        videos = videos.OrderBy(x => x.subarea.nome);
                    }
                    else
                    {
                        videos = videos.OrderByDescending(x => x.subarea.nome);
                    }
                    break;

                default:
                    videos = videos.OrderBy(x => x.titulo);
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                try
                {
                    videos = videos.Where(v=> v.titulo.ToLower().Contains(search.ToLower()) || 
                                              v.status.nome.Contains(search.ToLower())  ||
                                              v.area.nome.Contains(search.ToLower())    ||
                                              v.subarea.nome.Contains(search.ToLower()));
                } catch (Exception)
                {
                    videos = videos.Where(v => v.titulo.ToLower().Contains(search.ToLower()));
                }
            }

            int totalItens = videos.Count();

            var resultados = videos.Skip(pagina)
                                    .Take(quantidade)
                                    .ToList()
                                    .Select(x => new
                                    {
                                        id = x.id,
                                        x.active,
                                        x.titulo,
                                        id_status = x.status.nome,
                                        id_area = x.area.nome,
                                        id_subarea = x.subarea != null ? x.subarea.nome : "Nenhum"
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult AceitarTermosModal()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditarVideo(VideoViewModel videoViewModel)
        {
            try
            {
                _videoService.EditVideo(videoViewModel);
                TempData["s"] = "Vídeo editado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.Message;
                return RedirectToAction("Index", "Professor");
            }

            return RedirectToAction("Index", "Professor");
        }

        public ActionResult ExcluirVideoModal(int id)
        {
            var video = db.video.Find(id);
            return View(video);
        }

        [HttpPost]
        public ActionResult ExcluirVideo(int id)
        {
            try
            {
                _videoService.DeleteVideo(id);
                TempData["s"] = "Excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.Message;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public ActionResult EditarVideo(int id)
        {
            try
            {
                var video = db.video.Find(id);
                if (video != null)
                {
                    return View(video);
                }
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.ToString();
            }
            TempData["e"] = "Video não encontrado.";
            return RedirectToAction("Index", "Professor");
        }
        public ActionResult DetalhesVideo(int id)
        {
            var video = db.video.Find(id);
            return View(video);
        }

        public ActionResult AtivarVideoModal(int id)
        {
            try
            {
                var video = db.video.Find(id);
                return View(video);
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.ToString();
            }
            TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ActionName("AtivarVideoModal")]
        public ActionResult AtivarVideoModalPost(int id)
        {
            try
            {
                _videoService.ActivateVideo(id);
                TempData["s"] = "Vídeo ativado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.Message;
                return RedirectToAction("Index", "Professor");
            }

            return RedirectToAction("Index", "Professor");
        }

    }
}