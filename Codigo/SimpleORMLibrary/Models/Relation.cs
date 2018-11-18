using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleORMLibrary.Models
{
    public class Relation
    {
        public String Name { get; set; } //Nombre en el sistema de la relacion
        public String ModelName { get; set; } //Nombre en el sistema del modelo con el que se relaciona
        public bool OneToMany { get; set; } //Si la relación es uno a muchos, en caso de ser falso representa que se trata de una relación uno a uno
        public List<ForeignKey> ForeignKeys { get; set; } //Claves ajenas

        public override String ToString() //Genera una String que contiene los datos de la relación
        {
            String resultado = "";

            resultado += "ModelName = " + ModelName + "\n";
            resultado += "OneToMany = " + OneToMany.ToString() + "\n";
            resultado += "ForeignKeys:\n";

            foreach (ForeignKey key in ForeignKeys)
            {
                resultado += "Name:" + key.Name + "\n";
                resultado += "NameAsPk:" + key.NameAsPk + "\n";
            }

            return resultado;
        }
    }
}
