using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;
using VideoTeca.Services;

namespace VideoTeca.Controllers
{
    public class CoordenadorController : Controller
    {
        private readonly dbContext db = new dbContext();
        private readonly IVideoService _videoService;
        private readonly IAreaService _areaService;
        private readonly IUserService _userService;
        public CoordenadorController(IVideoService videoService, IAreaService areaService, IUserService userService)
        {
            _videoService = videoService;
            _areaService = areaService;
            _userService = userService;
        }
        // GET: Coordenador
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ControleVideos()
        {
            ViewBag.Areas = _areaService.GetAllAreas();
            return View();
        }

        public ActionResult DetalhesVideo(int id)
        {
            var video = _videoService.GetVideoById(id);
            return View(video);
        }

        public ActionResult EditarVideo(int id)
        {
            var video = _videoService.GetVideoById(id);
            return View(video);
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
                TempData["e"] = "Ocorreu algum erro: " + ex.Message;
                return RedirectToAction("ControleVideos", "Coordenador");
            }

            return RedirectToAction("ControleVideos", "Coordenador");
        }


        public ActionResult ControleUsuarios()
        {
            ViewBag.Usuarios = _userService.GetAllUsers();
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
            try
            {
                string nome = formulario["Nome"];
                _areaService.CreateSubarea(nome);
                TempData["s"] = "Subárea criada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleSubareas");
        }

        public ActionResult CriarSubarea()
        {
            return View();
        }

        public ActionResult CriarArea()
        {
            ViewBag.Subareas = _areaService.GetAllActiveSubareas();
            return View();
        }

        [HttpPost]
        public ActionResult CriarArea(FormCollection formulario)
        {
            try
            {
                string nome = formulario["Nome"];
                List<long> subareas = formulario["Subareas[]"]?.Split(',').Select(long.Parse).ToList() ?? new List<long>();
                _areaService.CreateArea(nome, subareas);
                TempData["s"] = "Área criada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleAreas");
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
                _areaService.DeleteSubarea(id);
                TempData["s"] = "Subárea excluída com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleSubareas");
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
                _areaService.ActivateSubarea(idSubarea);
                TempData["s"] = "Subárea ativada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleSubareas");
        }


        [HttpPost]
        public ActionResult ExcluirArea(int id)
        {
            try
            {
                _areaService.DeleteArea(id);
                TempData["s"] = "Área excluída com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleAreas");
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
                _areaService.ActivateArea(idArea);
                TempData["s"] = "Área ativada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleAreas");
        }

        public ActionResult EditarArea(long id)
        {
            var area = _areaService.GetAreaById(id);
            ViewBag.Subareas = _areaService.GetAllActiveSubareas();
            ViewBag.SubareasAreas = _areaService.GetSubareasFromArea(id);
            return View(area);
        }

        [HttpPost]
        public ActionResult EditarArea(FormCollection formulario)
        {
            try
            {
                long areaId = Convert.ToInt64(formulario["idArea"]);
                string nome = formulario["Nome"];
                List<long> subareas = formulario["AreasSubarea[]"]?.Split(',').Select(long.Parse).ToList() ?? new List<long>();

                _areaService.EditArea(areaId, nome, subareas);
                TempData["s"] = "Área editada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
                return RedirectToAction("ControleAreas");
            }

            return RedirectToAction("ControleAreas");
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
                _userService.DeactivateUser(id);
                TempData["s"] = "Usuário excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleUsuarios");
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
                _userService.ActivateUser(idUsuario);
                TempData["s"] = "Usuário ativado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleUsuarios");
        }

        public ActionResult EditarUsuario(int id)
        {
            var usuario = _userService.GetUserById(id);
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
                    long userId = Convert.ToInt64(formulario["idUsuario"]);
                    long permission = Convert.ToInt64(formulario["TipoUsuario"]);
                    List<long> areaIds = formulario["AreasUsuario[]"]?.Split(',').Select(long.Parse).ToList() ?? new List<long>();

                    _userService.EditUser(userId, permission, areaIds);

                    transaction.Commit();
                    TempData["s"] = "Usuário editado com sucesso!";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["e"] = "Ocorreu algum erro, contate o administrador do sistema: " + ex.Message;
                }

                return RedirectToAction("ControleUsuarios");
            }
        }

        public ActionResult ListarVideosAjax(string search, string sort, string order, int? Area, int? SubArea, int? limit = 10, int? offset = 0)
        {
            int pageSize = limit ?? 10;
            int page = offset ?? 0;
            int totalItems;

            var videos = _videoService.GetFilteredVideos(search, sort, order, Area, SubArea, page, pageSize, out totalItems);

            var resultados = videos.Select(x => new
            {
                id = x.Id,
                active = x.Active,
                titulo = x.Titulo,
                id_status = x.StatusNome,
                id_area = x.AreaNome,
                id_subarea = x.SubareaNome
            }).ToList();

            return Json(new { total = totalItems, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarUsuariosAjax(string search, string sort, string order, int? Grupo, int? limit = 10, int? offset = 0)
        {
            int pageSize = limit ?? 10;
            int page = offset ?? 0;
            int totalItems;

            var usuarios = _userService.GetFilteredUsers(search, sort, order, Grupo, page, pageSize, out totalItems);

            var resultados = usuarios.Select(x => new
            {
                id = x.id,
                active = x.active,
                nome = x.nome,
                email = x.email,
                permission = CONSTANTES.userTypes.TryGetValue(x.permission, out var permissionName) ? permissionName : "Desconhecido"
            }).ToList();

            return Json(new { total = totalItems, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarAreasAjax(string search, string sort, string order, int? limit = 10, int? offset = 0)
        {
            int pageSize = limit ?? 10;
            int page = offset ?? 0;
            int totalItems;

            var areas = _areaService.GetFilteredAreas(search, sort, order, page, pageSize, out totalItems);

            var resultados = areas.Select(x => new
            {
                id = x.id,
                nome = x.nome,
                active = x.active
            }).ToList();

            return Json(new { total = totalItems, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarSubareasAjax(string search, string sort, string order, int? limit = 10, int? offset = 0)
        {
            int pageSize = limit ?? 10;
            int page = offset ?? 0;
            int totalItems;

            var subareas = _areaService.GetFilteredSubareas(search, sort, order, page, pageSize, out totalItems);

            var resultados = subareas.Select(x => new
            {
                id = x.id,
                nome = x.nome,
                active = x.active
            }).ToList();

            return Json(new { total = totalItems, rows = resultados }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AtivarVideoModal(int id)
        {
            try
            {
                var video = _videoService.GetVideoById(id);
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
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleVideos");
        }

        public ActionResult ExcluirVideoModal(int id)
        {
            var video = _videoService.GetVideoById(id);
            return View(video);
        }

        [HttpPost]
        public ActionResult ExcluirVideo(int id)
        {
            try
            {
                _videoService.DeleteVideo(id);
                TempData["s"] = "Vídeo excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["e"] = "Ocorreu um erro: " + ex.Message;
            }

            return RedirectToAction("ControleVideos");
        }

    }
}