using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleORMLibrary.Models
{
    public class ForeignKey
    {
        public String Name { get; set; } //Nombre como campo en el modelo que contiene la relación
        public String NameAsPk { get; set; } //Nombre como clave principal en el modelo con el que se relaciona
    }
}
