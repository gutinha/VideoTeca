using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class AvaliadorController : Controller
    {
        public readonly dbContext db = new dbContext();
        // GET: Avaliador
        public ActionResult Index()
        {
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        public ActionResult ListarVideosEnviadosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            long userLogado = Convert.ToInt64(Session["id_user"]);
            var user = db.usuario.Where(x => x.id == userLogado).FirstOrDefault();
            IQueryable<video> videos = db.video.Where(v => v.active == true && v.id_area == user.id_area && v.aprovado == false);

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
                    videos = videos.Where(v => v.titulo.ToLower().Contains(search.ToLower()) ||
                                              v.status.nome.Contains(search.ToLower()) ||
                                              v.area.nome.Contains(search.ToLower()) ||
                                              v.subarea.nome.Contains(search.ToLower()));
                }
                catch (Exception)
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
                                        id_subarea = x.subarea.nome
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }
    }
}