using SimpleORMLibrary.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class LineaFactura
    {
        public int IdLinea { get; set; }
        public Producto Prod { get; set; }
        public int Cantidad { get; set; }
        public String NombreProducto { get; set; }
        public double Precio { get; set; }

        public LineaFactura()
        {
        }

        public LineaFactura(int idLinea, int cantidad, Producto prod, String nombreProducto, double precio)
        {
            IdLinea = idLinea;
            Cantidad = cantidad;
            Prod = prod ?? throw new ArgumentNullException(nameof(prod));
            NombreProducto = nombreProducto;
            Precio = precio;
        }

        public LineaFactura(int cantidad, Producto prod, String nombreProducto, double precio)
        {
            Session s = new Session();
            int id;
            try { id = (int)s.getMax(typeof(LineaFactura), nameof(IdLinea)); }
            catch (System.NullReferenceException) { id = 0; }
            IdLinea = id + 1;

            Cantidad = cantidad;
            Prod = prod ?? throw new ArgumentNullException(nameof(prod));
            NombreProducto = nombreProducto;
            Precio = precio;
        }
    }
}
