using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebTFG.Controllers.Usuario
{
    public class PerfilController : Controller
    {
        public IActionResult Perfil()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                var user = JsonConvert.DeserializeObject<WebTFG.Models.Usuario>(HttpContext.Session.GetString("user"));
                return View(user);
            }
            else
            {
                return RedirectToAction("Inicio", "Inicio");
            }
        }
    }
}