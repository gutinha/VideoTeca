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

        public ActionResult ControleUsuarios()
        {
            ViewBag.Usuarios = db.usuario.ToList();
            return View();
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

    }
}