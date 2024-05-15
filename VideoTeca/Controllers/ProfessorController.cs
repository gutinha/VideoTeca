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

namespace VideoTeca.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly dbContext db = new dbContext();

        public ActionResult Index()
        {
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        public ActionResult EnviarVideo()
        {
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        public ActionResult BuscarSubArea(int id)
        {
            var subAreas = db.subarea.Where(x => x.id_area == id).ToList().Select(a => new { Id = a.id, Nome = a.nome });
            return Json(subAreas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarVideosEnviadosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            long userLogado = Convert.ToInt64(Session["id_user"]);
            IQueryable<video> videos = db.video.Where(v => v.active == true && v.enviadoPor == userLogado);

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
                                        x.titulo,
                                        id_status = x.status.nome,
                                        id_area = x.area.nome,
                                        id_subarea = x.subarea != null ? x.subarea.nome : "Nenhum"
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EnviarVideo(FormCollection formulario)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var titulo = formulario["titulo"];
                    var descricao = formulario["descricao"];
                    var url = formulario["url"];
                    var area = Convert.ToInt64(formulario["area"]);
                    var subareaStr = formulario["subarea"];
                    int subarea;
                    long userLogado = Convert.ToInt64(Session["id_user"]);
                    video video = new video
                    {
                        titulo = titulo,
                        url = url,
                        id_area = area,
                        enviadoPor = userLogado,
                        active = true,
                        id_status = 0,
                        aprovado = false,
                        enviadoEm = DateTime.Now
                    };
                    if (!string.IsNullOrEmpty(subareaStr) && int.TryParse(subareaStr, out subarea))
                    {
                        video.id_subarea = subarea;
                    }
                    if (!string.IsNullOrEmpty(descricao))
                    {
                        video.descricao = descricao;
                    }
                    db.video.Add(video);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["e"] = "Alguma coisa deu errado! Procure o administrador do sistema: " + ex.ToString();
                    return RedirectToAction("Index", "Professor");
                }
            }
            TempData["s"] = "Video enviado com sucesso! Aguarde um avaliador aprovar seu video.";
            return RedirectToAction("Index", "Professor");
        }

        public ActionResult AceitarTermosModal()
        {
            return View();
        }

    }
}