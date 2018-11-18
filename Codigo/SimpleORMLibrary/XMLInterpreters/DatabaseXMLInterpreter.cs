using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleORMLibrary.Databases;
using System.Xml;
using SimpleORMLibrary.GeneralExceptions;

namespace SimpleORMLibrary.XMLInterpreters
{
    public class DatabaseXMLInterpreter
    {
        //Devuelve una base de datos MySQL cargandola desde un nodo de <Databases>
        static private MySQLDatabase MySQLdatabaseCreator(XmlNode databaseXml)
        {
            MySQLDatabase db = new MySQLDatabase();

            if (databaseXml["Name"] == null) { throw new GeneralORMException("El fichero XML no define el nombre interno de una base de datos MySQL"); }
            if (databaseXml["Server"] == null) { throw new GeneralORMException("El fichero XML no define el servidor de una base de datos MySQL"); }
            if (databaseXml["UID"] == null) { throw new GeneralORMException("El fichero XML no define la UID de una base de datos MySQL"); }
            if (databaseXml["Port"] == null) { throw new GeneralORMException("El fichero XML no define el puerto de una base de datos MySQL"); }
            if (databaseXml["SSLMode"] == null) { throw new GeneralORMException("El fichero XML no define el modo ssl de una base de datos MySQL"); }

            db.Name = databaseXml["Name"].InnerText;
            db.Server = databaseXml["Server"].InnerText;
            db.UID = databaseXml["UID"].InnerText;
            db.Port = databaseXml["Port"].InnerText;
            db.SSLMode = databaseXml["SSLMode"].InnerText;
            if (databaseXml["Password"] == null) { db.Password = null; } else { db.Password = databaseXml["Password"].InnerText; }
            if (databaseXml["DatabaseName"] == null) { db.DatabaseName = null; } else { db.DatabaseName = databaseXml["DatabaseName"].InnerText; }

            return db;
        }

        //Devuelve una base de datos MongoDB cargandola desde un nodo de <Databases>
        static private MongoDBDatabase MongoDBdatabaseCreator(XmlNode databaseXml)
        {
            MongoDBDatabase db = new MongoDBDatabase();

            if (databaseXml["Name"] == null) { throw new GeneralORMException("El fichero XML no define el nombre interno de una base de datos MySQL"); }
            if (databaseXml["Server"] == null) { throw new GeneralORMException("El fichero XML no define el servidor de una base de datos MySQL"); }
            if (databaseXml["UserName"] == null) { throw new GeneralORMException("El fichero XML no define la UID de una base de datos MySQL"); }
            if (databaseXml["Port"] == null) { throw new GeneralORMException("El fichero XML no define el puerto de una base de datos MySQL"); }
            if (databaseXml["SSLMode"] == null) { throw new GeneralORMException("El fichero XML no define el modo ssl de una base de datos MySQL"); }

            db.Name = databaseXml["Name"].InnerText;
            db.Server = databaseXml["Server"].InnerText;
            db.UserName = databaseXml["UserName"].InnerText;
            db.Port = databaseXml["Port"].InnerText;
            db.SSLMode = Convert.ToBoolean(databaseXml["SSLMode"].InnerText);
            if (databaseXml["Password"] == null) { db.Password = null; } else { db.Password = databaseXml["Password"].InnerText; }
            if (databaseXml["DatabaseName"] == null) { db.DatabaseName = null; } else { db.DatabaseName = databaseXml["DatabaseName"].InnerText; }

            return db;
        }

        //Devuelve una base de datos cargandola desde un nodo de <Databases>
        static private Database databaseCreator(XmlNode databaseXml)
        {
            if (databaseXml["Type"] == null) { throw new GeneralORMException("El fichero XML no define el tipo de una base de datos"); }

            switch (databaseXml["Type"].InnerText)
            {
                case "MySQL":
                    return MySQLdatabaseCreator(databaseXml);
                case "MongoDB":
                    return MongoDBdatabaseCreator(databaseXml);
                default:
                    throw new GeneralORMException("El fichero XML define Bases de datos de tipo desconocido");
            }
        }

        //Devuelve un diccionario de bases de datos leidas deste ese fichero
        static public Dictionary<string, Database> getDatabases(String Filename)
        {
            Dictionary<string, Database> databases = new Dictionary<string, Database>();

            XmlDocument xml = new XmlDocument();
            xml.Load(Filename);

            if(xml.DocumentElement.Name != "Databases") { throw new GeneralORMException("El fichero XML no define Bases de datos"); }
            if(xml.DocumentElement.ChildNodes.Count == 0) { throw new GeneralORMException("El fichero XML no define ninguna base de datos"); }

            foreach (XmlNode databaseNodes in xml.DocumentElement.ChildNodes)
            {
                Database db = databaseCreator(databaseNodes);

                databases.Add(db.Name, db);
            }

            return databases;
        }
    }
}
