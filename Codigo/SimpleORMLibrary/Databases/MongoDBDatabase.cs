using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleORMLibrary.Databases
{
    public class MongoDBDatabase : Database
    {
        public String Server { get; set; } //IP del servidor
        public String UserName { get; set; } //Nombre del usuario
        public String Password { get; set; } //Contraseña del usuario
        public String Port { get; set; } //Puerto del servidor
        public bool SSLMode { get; set; } //Si el modo SSL se encuentra activado


        public MongoDBDatabase() { Type = "MongoDB"; } //Constructor por defecto, introduce como tipo de la base de datos "MongoDB"

        public override String ToString()  //Genera una String que contiene los datos de la base de datos
        {
            String resultado = "";

            resultado += "Name = " + Name + "\n";
            resultado += "Server = " + Server + "\n";
            resultado += "DatabaseName = " + DatabaseName + "\n";
            resultado += "UserName = " + UserName + "\n";
            resultado += "Password = " + Password + "\n";
            resultado += "Port = " + Port + "\n";
            resultado += "SSLMode = " + SSLMode + "\n";

            return resultado;
        }

        public override String getConnectionString() //Genera la String de conexión para MongoDB
        {
            String resultado = "mongodb://";
            resultado += UserName;
            resultado += ":";
            resultado += Password;
            resultado += "@";
            resultado += Server;
            resultado += ":";
            resultado += Port;
            resultado += "/";
            resultado += DatabaseName;
            resultado += "?";
            resultado += "ssl=" + SSLMode.ToString().ToLower();

            return resultado;
        }
    }
}
