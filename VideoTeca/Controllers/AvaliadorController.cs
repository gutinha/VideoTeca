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

namespace VideoTeca.Controllers
{
    public class AvaliadorController : Controller
    {
        private readonly dbContext db = new dbContext();
        // GET: Avaliador
        public ActionResult Index()
        {
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        public ActionResult AvaliarVideo(int id)
        {
            var video = db.video.Find(id);
            return View(video);
        }
        [HttpPost]
        public ActionResult AvaliarVideo(FormCollection formulario)
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    long userLogado = Convert.ToInt64(Session["id_user"]);
                    var user = db.usuario.Where(x => x.id == userLogado).FirstOrDefault();
                    var justificativa = formulario["justificativa"];
                    var id_video = Convert.ToInt64(formulario["id"]);
                    var video = db.video.Find(id_video);
                    if (justificativa != null)
                    {
                        if (!justificativa.IsEmpty() || !justificativa.Equals(""))
                        {
                            var novaAvaliacao = new video_avaliacoes
                            {
                                id_avaliador = user.id,
                                id_video = id_video,
                                justificativa = justificativa,
                                data_avaliacao = DateTime.Now
                            };
                            db.video_avaliacoes.Add(novaAvaliacao);
                            video.id_status = 1;
                        }
                    }
                    else
                    {
                        video.aprovado = true;
                        video.id_status = 2;
                    }

                    db.Entry(video).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();

                } catch(Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("Index");
                }
            }
            TempData["s"] = "Video avaliado com sucesso!";
            return RedirectToAction("Index");
        }

        public ActionResult ListarVideosEnviadosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            long userLogado = Convert.ToInt64(Session["id_user"]);
            var user = db.usuario.Where(x => x.id == userLogado).First();
            IQueryable<video> videos = db.video.Where(v => v.active == true && 
                                                      v.area.usuario.Any(u=> u.id == user.id) && 
                                                      v.aprovado == false &&
                                                      (!v.video_avaliacoes.Any() || v.video_avaliacoes.Any(a => a.justificativa == null)));

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
                                        id_subarea = x.subarea != null ? x.subarea.nome : "Nenhum"
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }
    }


}