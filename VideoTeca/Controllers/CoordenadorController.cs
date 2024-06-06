using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using VideoTeca.Models;

namespace VideoTeca.Controllers
{
    public class CoordenadorController : Controller
    {
        private readonly dbContext db = new dbContext();
        private static readonly Dictionary<long, string> PermissionNames = new Dictionary<long, string>
        {
            { 1, "Usuário" },
            { 2, "Professor" },
            { 3, "Avaliador" },
            { 4, "Dicom" },
            { 5, "Coordenador" }
        };
        // GET: Coordenador
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ControleVideos()
        {
            ViewBag.Areas = db.area.ToList();
            return View();
        }

        

        public ActionResult ControleUsuarios()
        {
            ViewBag.Usuarios = db.usuario.ToList();
            return View();
        }

        public ActionResult ControleAreas()
        {
            return View();
        }
        public ActionResult ControleSubareas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CriarSubarea(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string nome = formulario["Nome"];
                    //var subarea = db.subarea.Where(a => a.nome.Contains(formulario["Nome"]));
                    var subarea = db.subarea.Where(a => a.nome.Contains(nome)).ToList();

                    if (subarea.Any())
                    {
                        TempData["e"] = "Já existe uma subárea com esse nome.";
                        return RedirectToAction("ControleSubareas");
                    }

                    subarea newSubarea = new subarea();
                    newSubarea.nome = nome;
                    newSubarea.active = true;
                    
                    db.subarea.Add(newSubarea);
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["s"] = "Subárea criada com sucesso!";
                    return RedirectToAction("ControleSubareas");
                }
                catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("ControleUsuarios");
                }
            }
        }

        public ActionResult CriarSubarea()
        {
            return View();
        }

        public ActionResult CriarArea()
        {
            ViewBag.Subareas = db.subarea.Where(s => s.active == true).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CriarArea(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    string nome = formulario["Nome"];
                    var area = db.area.Where(a => a.nome.Contains(nome)).ToList();

                    if (area.Any())
                    {
                        TempData["e"] = "Já existe uma área com esse nome.";
                        return RedirectToAction("ControleAreas");
                    }

                    area newArea = new area();
                    newArea.nome = nome;
                    newArea.active = true;

                    if (!string.IsNullOrEmpty(formulario["Subareas[]"]))
                    {
                        string[] subareas = formulario["Subareas[]"].Split(',');
                        //converter array de string em array de int
                        List<long> arrayIntSubareas = subareas.Select(long.Parse).ToList();

                        var selectedAreas = db.subarea.Where(a => arrayIntSubareas.Contains(a.id)).ToList();
                        foreach (var subarea in selectedAreas)
                        {
                            newArea.subarea.Add(subarea);
                        }
                    }

                    db.area.Add(newArea);
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["s"] = "Área criada com sucesso!";
                    return RedirectToAction("ControleAreas");
                }
                catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("ControleAreas");
                }
            }
        }

        public ActionResult DetalhesAreaModal(int id)
        {
            var area = db.area.Find(id);
            ViewBag.Subareas = db.subarea.Where(v => v.area.Any(z => z.id == id)).ToList();
            return View(area);
        }

        public ActionResult ExcluirAreaModal(int id)
        {
            var area = db.area.Find(id);
            ViewBag.Subareas = db.subarea.Where(v => v.area.Any(z => z.id == id)).ToList();
            return View(area);
        }

        public ActionResult DetalhesSubareaModal(int id)
        {
            var subarea = db.subarea.Find(id);
            return View(subarea);
        }

        public ActionResult ExcluirSubareaModal(int id)
        {
            var subarea = db.subarea.Find(id);
            return View(subarea);
        }

        [HttpPost]
        public ActionResult ExcluirSubarea(int id)
        {
            try
            {
                var subarea = db.subarea.Find(id);
                subarea.active = false;
                db.Entry(subarea).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Excluído com sucesso!";
                return RedirectToAction("ControleSubareas");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleSubareas");
            }
        }

        public ActionResult EditarSubarea(int id)
        {
            var subarea = db.subarea.Find(id);
            return View(subarea);
        }

        [HttpPost]
        public ActionResult EditarSubarea(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var subarea = Convert.ToInt64(formulario["idSubarea"]);
                    var editSubarea = db.subarea.Find(subarea);

                    if (editSubarea == null)
                    {
                        TempData["e"] = "Subárea não encontrada.";
                        return RedirectToAction("ControleSubareas");
                    }
                    string nome = formulario["Nome"];
                    editSubarea.nome = nome;

                    db.Entry(editSubarea).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["s"] = "Subárea editada com sucesso!";
                    return RedirectToAction("ControleSubareas");
                }
                catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("ControleSubareas");
                }
            }
        }

        public ActionResult AtivarSubareaModal(int id)
        {
            var subarea = db.subarea.Find(id);
            return View(subarea);
        }

        [HttpPost]
        [ActionName("AtivarSubareaModal")]
        public ActionResult AtivarSubareaModalPost(int idSubarea)
        {
            try
            {
                var subarea = db.subarea.Find(idSubarea);
                subarea.active = true;
                db.Entry(subarea).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Subárea ativada com sucesso!";
                return RedirectToAction("ControleSubareas");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleSubareas");
            }
        }


        [HttpPost]
        public ActionResult ExcluirArea(int id)
        {
            try
            {
                var area = db.area.Find(id);
                area.active = false;
                db.Entry(area).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Excluído com sucesso!";
                return RedirectToAction("ControleAreas");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleAreas");
            }
        }

        public ActionResult AtivarAreaModal(int id)
        {
            var area = db.area.Find(id);
            ViewBag.Subareas = db.subarea.Where(v => v.area.Any(z => z.id == id)).ToList();
            return View(area);
        }

        [HttpPost]
        [ActionName("AtivarAreaModal")]
        public ActionResult AtivarAreaModalPost(int idArea)
        {
            try
            {
                var area = db.area.Find(idArea);
                area.active = true;
                db.Entry(area).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Área ativada com sucesso!";
                return RedirectToAction("ControleAreas");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleAreas");
            }
        }

        public ActionResult EditarArea(int id)
        {
            var area = db.area.Find(id);
            ViewBag.Subareas = db.subarea.Where(s=> s.active == true).ToList();
            ViewBag.SubareasAreas = db.subarea.Where(v => v.area.Any(z => z.id == id)).ToList();
            return View(area);
        }

        [HttpPost]
        public ActionResult EditarArea(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var area = Convert.ToInt64(formulario["idArea"]);
                    var editArea = db.area.Find(area);

                    if (editArea == null)
                    {
                        TempData["e"] = "Área não encontrada.";
                        return RedirectToAction("ControleAreas");
                    }

                    //excluir todas as areas pertencentes ao usuário
                    editArea.subarea.Clear();

                    if (!string.IsNullOrEmpty(formulario["AreasSubarea[]"]))
                    {
                        string[] subareasAreas = formulario["AreasSubarea[]"].Split(',');
                        //converter array de string em array de int
                        List<long> arrayIntSubareasArea = subareasAreas.Select(long.Parse).ToList();

                        var selectedAreas = db.subarea.Where(a => arrayIntSubareasArea.Contains(a.id)).ToList();
                        foreach (var subarea in selectedAreas)
                        {
                            editArea.subarea.Add(subarea);
                        }
                    }

                    db.Entry(editArea).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["s"] = "Área editada com sucesso!";
                    return RedirectToAction("ControleAreas");
                }
                catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("ControleAreas");
                }
            }
        }

        public ActionResult DetalhesUsuarioModal(int id)
        {
            var usuario = db.usuario.Find(id);
            ViewBag.AreasUsuario = db.area.Where(v => v.usuario.Any(z => z.id == id)).ToList();
            return View(usuario);
        }

        public ActionResult ExcluirUsuarioModal(int id)
        {
            var usuario = db.usuario.Find(id);
            ViewBag.AreasUsuario = db.area.Where(v => v.usuario.Any(z => z.id == id)).ToList();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult ExcluirUsuario(int id)
        {
            try
            {
                var user = db.usuario.Find(id);
                user.active = false;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Excluído com sucesso!";
                return RedirectToAction("ControleUsuarios");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleUsuarios");
            }
        }

        public ActionResult AtivarUsuarioModal(int id)
        {
            var usuario = db.usuario.Find(id);
            ViewBag.AreasUsuario = db.area.Where(v => v.usuario.Any(z => z.id == id)).ToList();
            return View(usuario);
        }

        [HttpPost]
        [ActionName("AtivarUsuarioModal")]
        public ActionResult AtivarUsuarioModalPost(int idUsuario)
        {
            try
            {
                var user = db.usuario.Find(idUsuario);
                user.active = true;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["s"] = "Usuário ativado com sucesso!";
                return RedirectToAction("ControleUsuarios");
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                return RedirectToAction("ControleUsuarios");
            }
        }

        public ActionResult EditarUsuario(int id)
        {
            var usuario = db.usuario.Find(id);
            //long userLogado = Convert.ToInt64(Session["id_user"]);
            ViewBag.AreasUsuario = db.area.Where(v=> v.usuario.Any(z=> z.id == id)).ToList();
            ViewBag.Areas = db.area.ToList();
            ViewBag.Permissao = db.usuario.Where(u=> u.id == id).Select(x => x.permission).First();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult EditarUsuario(FormCollection formulario)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = Convert.ToInt64(formulario["idUsuario"]);
                    var editUser = db.usuario.Find(user);

                    if (editUser == null)
                    {
                        TempData["e"] = "Usuário não encontrado.";
                        return RedirectToAction("ControleUsuarios");
                    }
                    
                    editUser.permission = Convert.ToInt64(formulario["TipoUsuario"]);

                    //excluir todas as areas pertencentes ao usuário
                    editUser.area.Clear();

                    if (!string.IsNullOrEmpty(formulario["AreasUsuario[]"]))
                    {
                        string[] areasUsuario = formulario["AreasUsuario[]"].Split(',');
                        //converter array de string em array de int
                        List<long> arrayIntAreasUsuario = areasUsuario.Select(long.Parse).ToList();

                        var selectedAreas = db.area.Where(a => arrayIntAreasUsuario.Contains(a.id)).ToList();
                        foreach (var area in selectedAreas)
                        {
                            editUser.area.Add(area);
                        }
                    }

                    db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["s"] = "Usuário editado com sucesso!";
                    return RedirectToAction("ControleUsuarios");
                } catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("ControleUsuarios");
                }
            }
        }

        public ActionResult ListarVideosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            IQueryable<video> videos = db.video;

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
                                        x.active,
                                        x.titulo,
                                        id_status = x.status.nome,
                                        id_area = x.area.nome,
                                        id_subarea = x.subarea != null ? x.subarea.nome : "Nenhum",
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarUsuariosAjax(string search, string sort, string order, int? Grupo, int? limit = 10, int? offset = 0)
        {
            //long userLogado = Convert.ToInt64(Session["id_user"]);
            IQueryable<usuario> usuarios = db.usuario;

            //Filtro por grupos
            if (Grupo != null && Grupo != 0)
            {
                usuarios = usuarios.Where(v => v.permission == Grupo);
            }

            int quantidade = limit ?? 10;
            int pagina = offset ?? 0;

            switch (sort)
            {
                case "nome":
                    if (order.Equals("asc"))
                    {
                        usuarios = usuarios.OrderBy(x => x.nome);
                    }
                    else
                    {
                        usuarios = usuarios.OrderByDescending(x => x.nome);
                    }
                    break;

                case "email":
                    if (order.Equals("asc"))
                    {
                        usuarios = usuarios.OrderBy(x => x.email);
                    }
                    else
                    {
                        usuarios = usuarios.OrderByDescending(x => x.email);
                    }
                    break;

                case "permission":
                    if (order.Equals("asc"))
                    {
                        usuarios = usuarios.OrderBy(x => x.permission);
                    }
                    else
                    {
                        usuarios = usuarios.OrderByDescending(x => x.permission);
                    }
                    break;

                default:
                    usuarios = usuarios.OrderBy(x => x.nome);
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                try
                {
                    usuarios = usuarios.Where(v => v.nome.ToLower().Contains(search.ToLower()) ||
                                              v.email.Contains(search.ToLower()));
                }
                catch (Exception)
                {
                    usuarios = usuarios.Where(v => v.nome.ToLower().Contains(search.ToLower()));
                }
            }

            int totalItens = usuarios.Count();

            var resultados = usuarios.Skip(pagina)
                                    .Take(quantidade)
                                    .ToList()
                                    .Select(x => new
                                    {
                                        id = x.id,
                                        x.active,
                                        x.nome,
                                        x.email,
                                        permission = PermissionNames.TryGetValue(x.permission, out var permissionName) ? permissionName : "Desconhecido"
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreasAjax(string search, string sort, string order, int? limit = 10, int? offset = 0)
        {
            //long userLogado = Convert.ToInt64(Session["id_user"]);
            IQueryable<area> areas = db.area;

            int quantidade = limit ?? 10;
            int pagina = offset ?? 0;

            switch (sort)
            {
                case "nome":
                    if (order.Equals("asc"))
                    {
                        areas = areas.OrderBy(x => x.nome);
                    }
                    else
                    {
                        areas = areas.OrderByDescending(x => x.nome);
                    }
                    break;

                default:
                    areas = areas.OrderBy(x => x.nome);
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                try
                {
                    areas = areas.Where(v => v.nome.ToLower().Contains(search.ToLower()));
                }
                catch (Exception)
                {
                    areas = areas.Where(v => v.nome.ToLower().Contains(search.ToLower()));
                }
            }

            int totalItens = areas.Count();

            var resultados = areas.Skip(pagina)
                                    .Take(quantidade)
                                    .ToList()
                                    .Select(x => new
                                    {
                                        id = x.id,
                                        x.nome,
                                        x.active,
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSubareasAjax(string search, string sort, string order, int? limit = 10, int? offset = 0)
        {
            IQueryable<subarea> subareas = db.subarea;

            int quantidade = limit ?? 10;
            int pagina = offset ?? 0;

            switch (sort)
            {
                case "nome":
                    if (order.Equals("asc"))
                    {
                        subareas = subareas.OrderBy(x => x.nome);
                    }
                    else
                    {
                        subareas = subareas.OrderByDescending(x => x.nome);
                    }
                    break;

                default:
                    subareas = subareas.OrderBy(x => x.nome);
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                try
                {
                    subareas = subareas.Where(v => v.nome.ToLower().Contains(search.ToLower()));
                }
                catch (Exception)
                {
                    subareas = subareas.Where(v => v.nome.ToLower().Contains(search.ToLower()));
                }
            }

            int totalItens = subareas.Count();

            var resultados = subareas.Skip(pagina)
                                    .Take(quantidade)
                                    .ToList()
                                    .Select(x => new
                                    {
                                        id = x.id,
                                        x.nome,
                                        x.active,
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

    }
}