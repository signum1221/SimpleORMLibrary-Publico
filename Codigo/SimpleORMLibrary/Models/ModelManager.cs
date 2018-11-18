using SimpleORMLibrary.XMLInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.Models
{
    public class ModelManager
    {
        private static Dictionary<String, Model> models = new Dictionary<String, Model>();

        //Recupera un mapa de clases según su nombre interno
        public static Model getModel(String internalName)
        {
            return models[internalName];
        }

        //Elimina de memoria un mapa de una clase
        public static void removeModel(String internalName)
        {
            models.Remove(internalName);
        }

        //Recupera todos los mapas de clases
        public static Dictionary<String, Model> getModels()
        {
            return models;
        }

        //Carga mapas de clase desde un fichero XML
        public static void addModels(String filename)
        {
            ModelXMLInterpreter.getModels(filename).ToList().ForEach(x => models[x.Key] = x.Value);
        }

        //Devuelve los modelos que son almacenados total o parcialmente en una base de datos
        public static List<Model> modelsInDatabase(String databaseName)
        {
            List<Model> l = new List<Model>();
            foreach (Model m in models.Values)
            {
                if (m.modelInDatabase(databaseName))
                    l.Add(m);
            }
            return l;
        }

    }
}
