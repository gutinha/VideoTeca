using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoTeca.Models;
using VideoTeca.Models.ViewModels;
using VideoTeca.Services;

namespace VideoTeca.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            {
                if (!_userService.ValidateUser(model.Email, model.Password))
                {
                    TempData["e"] = "Email ou senha incorretos!";
                    return RedirectToAction("Login");
                }

                var usuario = _userService.GetUserByEmail(model.Email);
                Session["id_user"] = usuario.id.ToString();
                Session["nome"] = usuario.nome;
                Session["role"] = usuario.permission.ToString();
                TempData["s"] = "Login realizado com sucesso!";
                return RedirectToAction("Index");
            }
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
    }
}