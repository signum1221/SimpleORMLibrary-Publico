using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.Models
{
    public class Field
    {
        //Nombre Interno
        public String Name { get; set; } //Nombre en el sistema del campo
        private String _DatabaseFieldName;
        //Nombre en el gestor de base de datos
        public String DatabaseFieldName //Nombre en la base de datos del campo, toma el nombre del sistema si no se ha introducido uno
        {
            get
            {
                if (_DatabaseFieldName == null)
                    return Name;
                else
                    return _DatabaseFieldName;
            }
            set { _DatabaseFieldName = value; }

        }
        public String DatabaseName { get; set; } //Nombre de la base de datos que contiene al campo

        public override String ToString() //Genera una String que contiene los datos del campo
        {
            String resultado = "";

            resultado += "Name = " + Name + "\n";
            resultado += "DatabaseFieldName = " + DatabaseFieldName.ToString() + "\n";
            resultado += "DatabaseName = " + DatabaseName + "\n";

            return resultado;
        }
    }
}