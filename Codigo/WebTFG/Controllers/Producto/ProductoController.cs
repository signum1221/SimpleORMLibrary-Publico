using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleORMLibrary.Sessions;

namespace WebTFG.Controllers.Producto
{
    public class ProductoController : Controller
    {
        public IActionResult Producto(int id)
        {
            Session s = new Session();
            WebTFG.Models.Producto prod = new WebTFG.Models.Producto();
            prod.IdProducto = id;
            s.get(prod);
            return View(prod);
        }

        public IActionResult AnyadirAlCarro(int id)
        {
            WebTFG.Models.Usuario user;
            if (HttpContext.Session.GetString("user") != null)
            {
                user = JsonConvert.DeserializeObject<WebTFG.Models.Usuario>(HttpContext.Session.GetString("user"));
            }
            else
            {
                return RedirectToAction("Inicio", "Inicio");
            }

            Session s = new Session();
            s.getProperty(user, nameof(user.CarroCompra));
            WebTFG.Models.Producto prod = new WebTFG.Models.Producto();
            prod.IdProducto = id;
            user.CarroCompra.AnyadirProducto(1, prod);
            return RedirectToAction("Producto", "Producto", new { id = id });
        }

        public IActionResult Productos()
        {
            Session s = new Session();
            List<WebTFG.Models.Producto> productos = s.getAll(typeof(WebTFG.Models.Producto)) as List<WebTFG.Models.Producto>;
            return View(productos);
        }
    }
}