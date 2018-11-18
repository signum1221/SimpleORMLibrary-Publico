using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleORMLibrary.Databases
{
    public abstract class Database
    {
        public String Name { get; set; } //Identificador de la base de datos en el sistema
        public String Type { get; set; } //Tipo de la base de datos (MySQL o MongoDB)
        private String _DatabaseName;
        public String DatabaseName //Nombre de la base de datos en su gestor, en caso de no introducirse se tomará el identificador en el sistema
        {
            get
            {
                if (_DatabaseName == null)
                    return Name;
                else
                    return _DatabaseName;
            }
            set { _DatabaseName = value; }
        }
        public abstract String getConnectionString(); //Función que devuelve la cadena para conectarse a la base de datos
    }
}
