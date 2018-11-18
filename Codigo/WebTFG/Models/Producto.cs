using SimpleORMLibrary.Sessions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public double Precio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ImagenPrincipal { get; set; }
        public List<String> Imagenes { get; set; }
        public string Archivo { get; set; }

        public Producto(){ }

        public Producto(int id, double precio, string nombre, string descripcion, string imagenPrincipal, List<string> imagenes, string archivo)
        {
            IdProducto = id;
            Precio = precio;
            Nombre = nombre;
            Descripcion = descripcion;
            ImagenPrincipal = imagenPrincipal ?? throw new ArgumentNullException(nameof(imagenPrincipal));
            Imagenes = imagenes ?? throw new ArgumentNullException(nameof(imagenes));
            Archivo = archivo ?? throw new ArgumentNullException(nameof(archivo));
        }

        public Producto(double precio, string nombre, string descripcion, string imagenPrincipal, List<string> imagenes, string archivo)
        {
            Session s = new Session();
            int id;
            try { id = (int)s.getMax(typeof(Producto), nameof(IdProducto)); }
            catch (System.NullReferenceException) { id = 0; }
            IdProducto = id + 1;

            Precio = precio;
            Nombre = nombre;
            Descripcion = descripcion;
            ImagenPrincipal = imagenPrincipal ?? throw new ArgumentNullException(nameof(imagenPrincipal));
            Imagenes = imagenes ?? throw new ArgumentNullException(nameof(imagenes));
            Imagenes = new List<string>();
            Archivo = archivo ?? throw new ArgumentNullException(nameof(archivo));
        }
        
        /* 
         * Función usada para comprobar el funcionamiento de la aplicación sin conectarla a bases de datos.
         * Devuelve siempre la misma lista de productos
         */
        static public List<Producto> debugNoDBInitialization()
        {
            List<Producto> listaDebug = new List<Producto>();
            for (int i = 0; i < 50; i++)
            {
                Producto prod = new Producto(i, 10.0 + i * .1, "a", "desc", "0.jpg", new List<string> { "0-0.jpg", "0-1.jpg", "0-2.jpg", "0-3.jpg" }, "0.jpg");
                listaDebug.Add(prod);
            }
            return listaDebug;
        }

        static public void debugRegistraProductosYMideTiempo()
        {
            Session s = new Session();

            //SimpleORMLibrary.Models.ModelManager.removeModel("Producto");
            //SimpleORMLibrary.Models.ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\ProductoMysql.xml");

            for (int multiplier = 1; multiplier <= 10000/*00*/; multiplier = multiplier * 10)
            {
                int i = 1;
                Stopwatch stopWatchGuarda = new Stopwatch();
                stopWatchGuarda.Start();
                for (; i <= 1 * multiplier; i++)
                {
                    Producto prod = new Producto(
                    i,
                    1 + i * 0.1,
                    "Producto" + i,
                    "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc, quis gravida magna mi a libero. Fusce vulputate eleifend sapien. Vestibulum purus quam, scelerisque ut, mollis sed, nonummy id, met",
                    "Mockup.jpg",
                    new List<string> { "Mockup2.jpg" },//No se registrará
                    "Producto.zip");
                    s.save(prod);
                }
                stopWatchGuarda.Stop();
                TimeSpan ts = stopWatchGuarda.Elapsed;
                System.Diagnostics.Debug.WriteLine("-Guarda-" + ts.TotalSeconds + "-------" + (i-1));

                Stopwatch stopWatchRecupera = new Stopwatch();
                stopWatchRecupera.Start();
                s.getAll(typeof(Producto));
                stopWatchRecupera.Stop();
                ts = stopWatchRecupera.Elapsed;
                System.Diagnostics.Debug.WriteLine("-Recupera-" + ts.TotalSeconds + "-------" + (i - 1));

                Stopwatch stopWatchBorra = new Stopwatch();
                stopWatchBorra.Start();
                for (int j = 1; j <= 1 * multiplier; j++)
                {
                    Producto prod = new Producto();
                    prod.IdProducto = j;
                    s.delete(prod);
                }
                stopWatchBorra.Stop();
                ts = stopWatchBorra.Elapsed;
                System.Diagnostics.Debug.WriteLine("-Borra-" + ts.TotalSeconds + "-------" + (i - 1));
            }
        }

        static public void debugRegistraProductos()
        {
            Session s = new Session();
            for (int i = 0; i < 500; i++)
            {
                Producto prod = new Producto(
                i,
                1 + i * 0.1,
                "Producto" + i,
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc, quis gravida magna mi a libero. Fusce vulputate eleifend sapien. Vestibulum purus quam, scelerisque ut, mollis sed, nonummy id, met",
                "Mockup.jpg",
                new List<string> { "Mockup2.jpg" },//No se registrará
                "Producto.zip");
                s.save(prod);
            }
        }
}
}
