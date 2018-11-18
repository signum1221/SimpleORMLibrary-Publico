using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTFG.Models
{
    public class Usuario
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public byte[] Salt { get; set; }
        public List<Factura> Facturas { get; set; }
        public Carro CarroCompra { get; set; }
        public bool Admin { get; set; }
        public double Saldo { get; set; }
        public String FechaRegistro { get; set; }

        public Usuario()
        {
        }

        public Usuario(string username, string email, string nombre, string apellidos)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellidos = apellidos ?? throw new ArgumentNullException(nameof(apellidos));
            Admin = false;
            Saldo = 0.0;
            Facturas = new List<Factura>();
            CarroCompra = new Carro();
            byte[] generatedSalt = new byte[64];
            using (var random = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(generatedSalt);
            }
            Salt = generatedSalt;
            FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
