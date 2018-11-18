using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleORMLibrary.Sessions;
using WebTFG.Models;

namespace WebTFG.Controllers.Carro
{
    public class CarroController : Controller
    {
        public IActionResult Carro()
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

        public IActionResult Decrease(int id, int cantidad)
        {
            cantidad--;
            if(cantidad == 0) { return RedirectToAction("Remove", "Carro", new { id = id }); }
            Session s = new Session();
            LineaCarro lc = new LineaCarro();
            lc.IdLinea = id;
            s.get(lc);
            lc.Cantidad = cantidad;
            s.update(lc);

            return RedirectToAction("Carro", "Carro");
        }
        public IActionResult Increase(int id, int cantidad)
        {
            cantidad++;
            Session s = new Session();
            LineaCarro lc = new LineaCarro();
            lc.IdLinea = id;
            s.get(lc);
            lc.Cantidad = cantidad;
            s.update(lc);

            return RedirectToAction("Carro", "Carro");
        }
        public IActionResult Remove(int id)
        {
            Session s = new Session();
            LineaCarro lc = new LineaCarro();
            lc.IdLinea = id;
            s.delete(lc);

            return RedirectToAction("Carro", "Carro");
        }

        public IActionResult Comprar()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                var user = JsonConvert.DeserializeObject<WebTFG.Models.Usuario>(HttpContext.Session.GetString("user"));

                Session s = new Session();
                s.get(user);
                s.getProperty(user, nameof(user.CarroCompra));
                s.getProperty(user.CarroCompra, nameof(user.CarroCompra.Lineas));
                s.getProperty(user, nameof(user.Facturas));

                Factura f = new Factura();
                var lineaOffset = 0;
                foreach(LineaCarro lc in user.CarroCompra.Lineas)
                {
                    s.getProperty(lc, nameof(lc.Prod));
                    LineaFactura lf = new LineaFactura(lc.Cantidad, lc.Prod, lc.Prod.Nombre, lc.Prod.Precio);
                    lf.IdLinea += lineaOffset;
                    lineaOffset++;
                    f.Lineas.Add(lf);
                    user.Saldo -= lc.Prod.Precio * lc.Cantidad;
                    s.delete(lc);
                }
                user.CarroCompra = null;
                user.Facturas.Add(f);
                s.update(user);

                return RedirectToAction("Perfil", "Perfil");
            }
            else
            {
                return RedirectToAction("Inicio", "Inicio");
            }
        }
    }
}