using SimpleORMLibrary.GeneralExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.Models
{
    public class Model
    {
        public String Name { get; set; } //Nombre en el sistema de la tabla o coleccion
        public bool AllFieldsOnTheSameDatabase { get; set; }//Si solo tiene una base de datos
        public String PrimaryDatabase { get; set; }//Nombre en el sistema de la base de datos principal
        public List<Field> Fields { get; set; }//Campos
        public List<Field> PrimaryKeys { get; set; }//Claves primarias
        public List<Relation> Relations { get; set; } //Relaciones

        public override String ToString() //Genera una String que contiene los datos del mapa
        {
            String resultado = "";

            resultado += "Name = " + Name + "\n";
            resultado += "AllFieldsOnTheSameDatabase = " + AllFieldsOnTheSameDatabase.ToString() + "\n";
            resultado += "PrimaryDatabase = " + PrimaryDatabase + "\n";

            resultado += "Fields:\n";
            foreach (Field f in Fields)
                resultado += f.ToString();

            resultado += "Primary keys:\n";
            foreach (Field pk in PrimaryKeys)
                resultado += pk.ToString();

            resultado += "Relations:\n";
            foreach (Relation r in Relations)
                resultado += r.ToString();

            return resultado;
        }

        //Campos pertenecientes a la base de datos de ese nombre
        public List<Field> fieldsInDatabase(String databaseName)
        {
            if (AllFieldsOnTheSameDatabase && PrimaryDatabase == databaseName)
            {
                return Fields;
            }
            else
            {
                List<Field> l = new List<Field>();

                foreach(Field f in Fields)
                {
                    if (f.DatabaseName == databaseName)
                        l.Add(f);
                }
                return l;
            }
        }

        //Devuelve si una determinada base de datos con ese nombre contiene el objeto o alguno de sus campos
        public bool modelInDatabase(String databaseName)
        {
            if (PrimaryDatabase == databaseName)
            {
                return true;
            }
            else
            {
                foreach(Field f in Fields)
                {
                    if (f.DatabaseName == databaseName)
                        return true;
                }
                return false;
            }
        }

        //Devuelve los nombres de las bases de datos usadas
        public List<String> databasesUsed()
        {
            List<String> databaseNames = new List<string>();
            databaseNames.Add(PrimaryDatabase);

            if (!AllFieldsOnTheSameDatabase)
                foreach (Field f in Fields)
                    if (!databaseNames.Contains(f.DatabaseName))
                        databaseNames.Add(f.DatabaseName);

            return databaseNames;
        }

        //Devuelve el nombre en el sistema del campo con ese nombre en la base de datos
        public String fieldDatabaseNameToInternalName(String dbName)
        {
            foreach (Field pk in PrimaryKeys)
            {
                if (pk.DatabaseFieldName == dbName)
                {
                    return pk.Name;
                }
            }

            foreach (Field f in Fields)
            {
                if (f.DatabaseFieldName == dbName)
                {
                    return f.Name;
                }
            }

            throw new GeneralORMException("No existe el campo que tenga ese nombre en la base de datos en el modelo");
        }

        //Devuelve la relación de dicho nombre
        public Relation getRelationByName(String name)
        {
            foreach (Relation r in Relations)
            {
                if (r.Name == name)
                {
                    return r;
                }
            }
            return null;
        }

        //Devuelve el campo de dicho nombre
        public Field getFieldByName(String name)
        {
            foreach (Field f in Fields)
            {
                if (f.Name == name)
                {
                    return f;
                }
            }
            return null;
        }
    }
}
