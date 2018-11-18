using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleORMLibrary.Sessions;
using WebTFG.Controllers.Inicio;

namespace WebTFG.Controllers.Login
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            Session s = new Session();
            WebTFG.Models.Usuario user = new WebTFG.Models.Usuario();
            user.Username = username;
            try { s.get(user); } catch (SimpleORMLibrary.GeneralExceptions.GeneralORMException) { return View("Login", "Login incorrecto"); }
            var userjson = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("user", userjson);

            InicioController Ic = new InicioController();
            return RedirectToAction("Inicio", "Inicio");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Inicio", "Inicio");
        }
    }
}