using SimpleORMLibrary.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public List<LineaFactura> Lineas { get; set; }
        public String Fecha { get; set; }

        public Factura(int idFactura)
        {
            IdFactura = idFactura;
            Lineas = new List<LineaFactura>();
            Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public Factura()
        {
            Session s = new Session();
            int id;
            try { id = (int)s.getMax(typeof(Factura), nameof(IdFactura)); }
            catch (System.NullReferenceException) { id = 0; }
            IdFactura = id + 1;
            Lineas = new List<LineaFactura>();
            Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
