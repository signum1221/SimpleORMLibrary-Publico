using MySql.Data.MySqlClient;
using SimpleORMLibrary.GeneralExceptions;
using SimpleORMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.Databases
{
    public class MySQLDatabase : Database
    {
        public String Server { get; set; } //IP del servidor
        public String UID { get; set; } //Identificador único
        public String Password { get; set; } //Contraseña para el UID
        public String Port { get; set; }  //Puerto del servidor
        public String SSLMode { get; set; } //Si el modo SSL se encuentra activado


        public MySQLDatabase() { Type = "MySQL"; } //Constructor por defecto, introduce como tipo de la base de datos "MySQL"

        public override String ToString() //Genera una String que contiene los datos de la base de datos
        {
            String resultado = "";

            resultado += "Name = " + Name + "\n";
            resultado += "Server = " + Server + "\n";
            resultado += "DatabaseName = " + DatabaseName + "\n";
            resultado += "UID = " + UID + "\n";
            resultado += "Password = " + Password + "\n";
            resultado += "Port = " + Port + "\n";
            resultado += "SSL Mode = " + SSLMode + "\n";

            return resultado;
        }

        public override String getConnectionString() //Genera la String de conexión para MYSQL
        {
            String resultado = "";

            resultado += "server=" + Server + ";";
            resultado += "port=" + Port + ";";
            resultado += "database=" + DatabaseName + ";";
            resultado += "uid=" + UID + ";";
            resultado += "pwd=" + Password + ";";
            resultado += "sslmode=" + SSLMode + ";";

            return resultado;
        }
    }
}
