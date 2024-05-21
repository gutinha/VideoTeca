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
            ViewBag.Usuarios = db.usuario.Where(u => u.active == true).ToList();
            return View();
        }

        public ActionResult EditarUsuario(int id)
        {
            var usuario = db.usuario.Find(id);
            long userLogado = Convert.ToInt64(Session["id_user"]);
            ViewBag.AreasUsuario = db.area.Where(v=> v.usuario.Any(z=> z.id == id));
            ViewBag.Permissao = db.usuario.Where(u=> u.id == userLogado).Select(x => x.permission);
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

                    editUser.permission = Convert.ToInt64(formulario["TipoUsuario"]);
                    string[] areasUsuario = formulario["AreasUsuario[]"].Split(',');

                    //converter array de string em array de int
                    List<long> arrayIntAreasUsuario = new List<long>();
                    foreach (var item in areasUsuario)
                    {
                        arrayIntAreasUsuario.Add(Convert.ToInt64(item));
                    }

                    //excluir todas as areas pertencentes ao usuário
                    editUser.area.Clear();

                    if (!formulario["AreasUsuario[]"].IsEmpty() || formulario["AreasUsuario[]"] != null)
                    {
                        var selectedAreas = db.area.Where(a => arrayIntAreasUsuario.Contains(a.id)).ToList();
                        foreach (var area in selectedAreas)
                        {
                            editUser.area.Add(area);
                        }
                    }
                    db.Entry(editUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                } catch (Exception ex)
                {
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.ToString();
                    transaction.Rollback();
                    return RedirectToAction("Index");
                }
                TempData["s"] = "Usuário editado com sucesso!";
                return RedirectToAction("Index");
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
                                        x.nome,
                                        x.email,
                                        permission = PermissionNames.TryGetValue(x.permission, out var permissionName) ? permissionName : "Desconhecido"
                                    }).ToList();

            return Json(new { total = totalItens, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

    }
}