using SimpleORMLibrary.XMLInterpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.Databases
{
    public class DatabaseManager
    {
        private static Dictionary<String,Database> databases = new Dictionary<String,Database>();

        //Recupera una base de datos según su nombre interno
        public static Database getDatabase(String internalName)
        {
            return databases[internalName];
        }

        //Recupera todas las bases de datos
        public static Dictionary<String, Database> getDatabases()
        {
            return databases;
        }

        //Carga las bases de datos desde un fichero XML
        public static void loadDatabases(String filename)
        {
            databases = DatabaseXMLInterpreter.getDatabases(filename);
        }
    }
}
