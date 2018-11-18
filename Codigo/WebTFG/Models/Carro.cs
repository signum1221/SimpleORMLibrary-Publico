using SimpleORMLibrary.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class Carro
    {
        public int IdCarro { get; set; }
        public List<LineaCarro> Lineas { get; set; }

        public Carro(int idCarro)
        {
            IdCarro = idCarro;
            Lineas = new List<LineaCarro>();
        }

        public Carro()
        {
            Session s = new Session();
            int id;
            try { id = (int)s.getMax(typeof(Carro), nameof(IdCarro)); }
            catch (System.NullReferenceException) { id = 0; }
            IdCarro = id + 1;
            Lineas = new List<LineaCarro>();
        }

        public void AnyadirProducto(int cantidad, Producto producto)
        {
            Session s = new Session();
            LineaCarro lc = new LineaCarro(cantidad, producto);
            this.Lineas.Add(lc);
            s.update(this);
        }
    }
}
