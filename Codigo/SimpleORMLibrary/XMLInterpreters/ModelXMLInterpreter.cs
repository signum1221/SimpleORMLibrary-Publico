using System;
using System.Collections.Generic;
using System.Linq;
using SimpleORMLibrary.Models;
using SimpleORMLibrary.GeneralExceptions;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.XMLInterpreters
{
    public class ModelXMLInterpreter
    {
        //Devuelve un campo cargandolo desde un nodo de <PrimaryKeys> o <Fields>
        static private Field fieldCreator(XmlNode fieldXml, bool usingPrincipalDatabase, bool isPrimaryKey)
        {
            Field f = new Field();

            if (fieldXml["Name"] == null) { throw new GeneralORMException("El fichero XML no define el nombre de un campo"); }
            if (usingPrincipalDatabase == false && fieldXml["DatabaseName"] == null) { throw new GeneralORMException("El fichero XML no define la base de datos de un campo"); }

            f.Name = fieldXml["Name"].InnerText;
            
            if (!usingPrincipalDatabase)
                f.DatabaseName = fieldXml["DatabaseName"].InnerText;
            else
                f.DatabaseName = null;

            if (fieldXml["DatabaseFieldName"] == null) { f.DatabaseFieldName = null; } else { f.DatabaseFieldName = fieldXml["DatabaseFieldName"].InnerText; }

            return f;
        }

        //Devuelve una clave principal cargandola desde un nodo de <PrimaryKeys>
        static private Field primaryKeyCreator(XmlNode fieldXml)
        {
            return fieldCreator(fieldXml, true, true);
        }

        //Devuelve una relación cargandola desde un nodo de <Relations>
        static private Relation relationCreator(XmlNode relationXml)
        {
            Relation r = new Relation();

            if (relationXml["Name"] == null) { throw new GeneralORMException("El fichero XML no define el nombre de la relación en código"); }
            if (relationXml["ModelName"] == null) { throw new GeneralORMException("El fichero XML no define el nombre del modelo con el que esta relacionado"); }
            if (relationXml["OneToMany"] == null) { throw new GeneralORMException("El fichero XML no define si la relación es uno a muchos o uno a uno"); }
            if (relationXml["ForeignKeys"] == null) { throw new GeneralORMException("El fichero XML no define claves ajenas para una relación"); }

            r.Name = relationXml["Name"].InnerText;
            r.ModelName = relationXml["ModelName"].InnerText;
            r.OneToMany = Convert.ToBoolean(relationXml["OneToMany"].InnerText);

            List<ForeignKey> fkeys = new List<ForeignKey>();
            foreach(XmlNode relationNode in relationXml["ForeignKeys"].ChildNodes)
            {
                if (relationNode["Name"] == null) { throw new GeneralORMException("El fichero XML no especifica el nombre de una clave ajena"); }
                if (relationNode["NameAsPk"] == null) { throw new GeneralORMException("El fichero XML no especifica el nombre de la clave principal a la que referencia una clave ajena"); }

                ForeignKey fk = new ForeignKey();
                fk.Name = relationNode["Name"].InnerText;
                fk.NameAsPk = relationNode["NameAsPk"].InnerText;

                fkeys.Add(fk);
            }

            r.ForeignKeys = fkeys;

            return r;
        }

        //Devuelve un mapa de clase cargandolo desde un nodo de <Models>
        static private Model modelCreator(XmlNode modelXml)
        {
            Model m = new Model();

            if (modelXml["Name"] == null) { throw new GeneralORMException("El fichero XML no define el nombre de un modelo"); }
            if (modelXml["AllFieldsOnTheSameDatabase"] == null) { throw new GeneralORMException("El fichero XML no define si el modelo pertenece a una sola base de datos"); }
            if (modelXml["PrimaryDatabase"] == null) { throw new GeneralORMException("El fichero XML no define la base de datos principal"); }

            m.Name = modelXml["Name"].InnerText;

            try
            {
                m.AllFieldsOnTheSameDatabase = Convert.ToBoolean(modelXml["AllFieldsOnTheSameDatabase"].InnerText);
            }
            catch (FormatException)
            {
                throw new GeneralORMException("El fichero XML no define correctamente si el modelo pertenece a una sola base de datos");
            }

            m.PrimaryDatabase = modelXml["PrimaryDatabase"].InnerText;

            //Carga de campos
            List<Field> fields = new List<Field>();
            if (modelXml["Fields"] != null)
            {
                foreach (XmlNode modelNodes in modelXml["Fields"].ChildNodes)
                {
                    Field f;

                    f = fieldCreator(modelNodes, m.AllFieldsOnTheSameDatabase, false);

                    fields.Add(f);
                }
            }
            m.Fields = fields;

            //Carga de claves principales
            List<Field> primaryKeys = new List<Field>();
            if (modelXml["PrimaryKeys"] != null)
            {
                foreach (XmlNode modelNodes in modelXml["PrimaryKeys"].ChildNodes)
                {
                    Field pk;

                    pk = primaryKeyCreator(modelNodes);

                    primaryKeys.Add(pk);
                }
            }
            else { throw new GeneralORMException("El fichero XML no define las claves principales del modelo"); }
            m.PrimaryKeys = primaryKeys;

            //Carga relaciones
            List<Relation> relations = new List<Relation>();
            if (modelXml["Relations"] != null)
            {
                foreach (XmlNode modelNodes in modelXml["Relations"].ChildNodes)
                {
                    Relation r;

                    r = relationCreator(modelNodes);

                    relations.Add(r);
                }
            }
            m.Relations = relations;

            return m;
        }

        //Devuelve un diccionario de mapas de clases leidos deste ese fichero
        static public Dictionary<string, Model> getModels(String Filename)
        {
            Dictionary<string, Model> models = new Dictionary<string, Model>();

            XmlDocument xml = new XmlDocument();
            xml.Load(Filename);

            if (xml.DocumentElement.Name != "Models") { throw new GeneralORMException("El fichero XML no define modelos"); }
            if (xml.DocumentElement.ChildNodes.Count == 0) { throw new GeneralORMException("El fichero XML no define ningun modelo"); }

            foreach (XmlNode modelNodes in xml.DocumentElement.ChildNodes)
            {
                Model m = modelCreator(modelNodes);

                models.Add(m.Name, m);
            }

            return models;
        }
    }
}
