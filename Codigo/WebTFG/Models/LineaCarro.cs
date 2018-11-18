using SimpleORMLibrary.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class LineaCarro
    {
        public int IdLinea { get; set; }
        public int Cantidad { get; set; }
        public Producto Prod { get; set; }

        public LineaCarro()
        {
        }

        public LineaCarro(int idLinea, int cantidad, Producto prod)
        {
            IdLinea = idLinea;
            Cantidad = cantidad;
            Prod = prod ?? throw new ArgumentNullException(nameof(prod));
        }

        public LineaCarro(int cantidad, Producto prod)
        {
            Session s = new Session();
            int id;
            try { id = (int)s.getMax(typeof(LineaCarro), nameof(IdLinea)); }
            catch (System.NullReferenceException) { id = 0; }
            IdLinea = id + 1;

            Cantidad = cantidad;
            Prod = prod ?? throw new ArgumentNullException(nameof(prod));
        }
    }
}
