using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleORMLibrary.Sessions;
using WebTFG.Models;

namespace WebTFG.Controllers.Inicio
{
    public class InicioController : Controller
    {
        public IActionResult Inicio()
        {
            Session s = new Session();
            List<WebTFG.Models.Producto> productos = s.getAll(typeof(WebTFG.Models.Producto)) as List<WebTFG.Models.Producto>;
            return View(productos);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}