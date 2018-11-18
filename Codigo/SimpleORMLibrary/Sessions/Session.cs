using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleORMLibrary.Sessions.Transactions;
using SimpleORMLibrary.Databases;
using SimpleORMLibrary.GeneralExceptions;
using SimpleORMLibrary.Models;
using System.Reflection;

namespace SimpleORMLibrary.Sessions
{
    public class Session
    {
        //Devuelve un objeto TransactionManager
        public static TransactionManager startTransaction(Database database)
        {
            return createTransaction(database);
        }

        //Crea un objeto TransactionManager según el tipo de la base de datos
        private static TransactionManager createTransaction(Database database)
        {
            Type databaseType = database.GetType();

            if (databaseType == typeof(MySQLDatabase))
            {
                return new MySQLTransactionManager { Db = database };
            }
            else if (databaseType == typeof(MongoDBDatabase))
            {
                return new MongoDBTransactionManager { Db = database };
            }
            else
            {
                throw new GeneralORMException("No existe clase de transacción para ese tipo de base de datos");
            }
        }

        //Convierte los contenidos de las propiedades de los objetos a una lista con el nombre en la base de datos y el valor de estos
        private static List<Tuple<String, Object>> fieldsToDatabasePairs(Object obj, List<Field> lf)
        {
            Type type = obj.GetType();
            List<Tuple<String, Object>> ldb = new List<Tuple<String, Object>>();
            foreach (Field f in lf)
            {
                Object fValue = type.GetProperty(f.Name).GetValue(obj);

                ldb.Add(new Tuple<String, Object>(f.DatabaseFieldName, fValue));
            }
            return ldb;
        }

        //Actualiza la base de datos con las relaciones entre dos objetos
        private void link(Object obj, Model m, List<Tuple<String, Object>> pks, List<ForeignKey> foreignKeys)
        {
            List<Tuple<String, Object>> fields = new List<Tuple<String, Object>>();
            foreach (Tuple<String, Object> pk in pks)
            {
                String foreignKey = (from fk in foreignKeys where fk.NameAsPk == pk.Item1 select fk.Name).First();
                fields.Add(new Tuple<String, Object>(foreignKey, pk.Item2));
            }

            List<Tuple<String, Object>> fpks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            Database db = DatabaseManager.getDatabase(m.PrimaryDatabase);

            TransactionManager tr = startTransaction(db);

            tr.update(m.Name, fpks, fields);
        }

        //Guarda un objeto en la base de datos
        private void saveOne(Object obj, Model m, Type type)
        {
            List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            List<Tuple<String, Object>> foreignKeys = new List<Tuple<String, Object>>();
            foreach (Relation r in m.Relations)
            {
                Object foreignObject = type.GetProperty(r.Name).GetValue(obj);
                if (foreignObject != null)
                {
                    Model foreignModel = ModelManager.getModel(r.ModelName);
                    if (r.OneToMany)
                    {
                        Type ftype = foreignObject.GetType().GetGenericArguments()[0];
                        foreach (Object objectToSave in foreignObject as System.Collections.IList)
                        {
                            try { saveOne(objectToSave, foreignModel, ftype); } catch (GeneralORMException) { }
                            link(objectToSave, foreignModel, pks, r.ForeignKeys);
                        }
                    }
                    else
                    {
                        try { save(foreignObject); } catch (GeneralORMException) { }
                        List<Tuple<String, Object>> fpks = fieldsToDatabasePairs(foreignObject, foreignModel.PrimaryKeys);
                        foreach (Tuple<String, Object> fpk in fpks)
                        {
                            String foreignKey = (from fk in r.ForeignKeys where fk.NameAsPk == fpk.Item1 select fk.Name).First();
                            foreignKeys.Add(new Tuple<String, Object>(foreignKey, fpk.Item2));
                        }
                    }
                }
            }
            if(foreignKeys.Count == 0){ foreignKeys = null; }

            foreach (String dbName in m.databasesUsed())
            {
                Database db = DatabaseManager.getDatabase(dbName);

                List<Tuple<String, Object>> fields = fieldsToDatabasePairs(obj, m.fieldsInDatabase(dbName));

                TransactionManager tr = startTransaction(db);
                if (m.PrimaryDatabase == dbName)
                {
                    tr.save(m.Name, pks, fields, foreignKeys);
                }
                else
                {
                    tr.save(m.Name, pks, fields, null);
                }
            }
        }

        //Guarda en la base de datos el objeto tras extraer el mapa de clases y su tipo
        public void save(Object obj)
        {
            Type type = obj.GetType();
            Model m = ModelManager.getModel(type.Name);
            saveOne(obj, m, type);
        }

        //Borra el objeto
        public void delete(Object obj)
        {
            Type type = obj.GetType();
            Model m = ModelManager.getModel(type.Name);

            List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            foreach (String dbName in m.databasesUsed())
            {
                Database db = DatabaseManager.getDatabase(dbName);

                TransactionManager tr = startTransaction(db);
                tr.delete(m.Name, pks);
            }
        }

        //Actualiza el objeto
        public void update(Object obj)
        {
            Type type = obj.GetType();
            Model m = ModelManager.getModel(type.Name);

            List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            List<Tuple<String, Object>> foreignKeys = null;
            foreach (Relation r in m.Relations)
            {
                Object foreignObject = type.GetProperty(r.Name).GetValue(obj);

                if (foreignObject != null)
                {
                    Model foreignModel = ModelManager.getModel(r.ModelName);
                    if (r.OneToMany)
                    {
                        Type ftype = foreignObject.GetType().GetGenericArguments()[0];
                        foreach (Object objectToSave in foreignObject as System.Collections.IList)
                        {
                            var fpks = fieldsToDatabasePairs(objectToSave, foreignModel.PrimaryKeys);
                            try { if (getWhere(objectToSave.GetType(), fpks).Count == 0) { saveOne(objectToSave, foreignModel, ftype); } else update(objectToSave); }
                            catch (GeneralORMException) { update(objectToSave); }
                            link(objectToSave, foreignModel, pks, r.ForeignKeys);
                        }

                        Database db = DatabaseManager.getDatabase(foreignModel.PrimaryDatabase);
                    }
                    else
                    {
                        try { if (getWhere(foreignObject.GetType(), pks).Count == 0) { save(foreignObject); } else { update(foreignObject); } }
                        catch (GeneralORMException) { update(foreignObject); }

                        List<Tuple<String, Object>> fpks = fieldsToDatabasePairs(foreignObject, foreignModel.PrimaryKeys);
                        foreignKeys = new List<Tuple<String, Object>>();
                        foreach (Tuple<String, Object> fpk in fpks)
                        {
                            String foreignKey = (from fk in r.ForeignKeys where fk.NameAsPk == fpk.Item1 select fk.Name).First();
                            foreignKeys.Add(new Tuple<String, Object>(foreignKey, fpk.Item2));
                        }
                    }
                }
            }

            foreach (String dbName in m.databasesUsed())
            {
                Database db = DatabaseManager.getDatabase(dbName);

                List<Tuple<String, Object>> fields = fieldsToDatabasePairs(obj, m.fieldsInDatabase(dbName));

                TransactionManager tr = startTransaction(db);
                if (dbName == m.PrimaryDatabase)
                {
                    List<Tuple<String, Object>> fieldsAndForeign = new List<Tuple<String, Object>>();
                    fieldsAndForeign.AddRange(fields);
                    if (foreignKeys != null) { fieldsAndForeign.AddRange(foreignKeys); }
                    if (fieldsAndForeign.Count > 0) { tr.update(m.Name, pks, fieldsAndForeign); }
                }
                else
                {
                    tr.update(m.Name, pks, fields);
                }
            }
        }

        //Carga en un objeto los resultados de recuperarlo
        private void resPairToObject(Object obj, List<Tuple<String, Object>> results, Type type, Model m)
        {
            foreach (Tuple<String, Object> resPair in results)
            {
                try
                {
                    String propName = m.fieldDatabaseNameToInternalName(resPair.Item1);
                    type.GetProperty(propName).SetValue(obj, resPair.Item2);
                }
                catch (GeneralORMException)
                {
                    continue;
                }
            }
        }

        //Recupera el objeto
        public void get(Object obj)
        {
            Type type = obj.GetType();
            Model m = ModelManager.getModel(type.Name);

            List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            foreach (String dbName in m.databasesUsed())
            {
                Database db = DatabaseManager.getDatabase(dbName);

                TransactionManager tr = startTransaction(db);
                List<Tuple<String, Object>> results = tr.get(m.Name, pks);

                resPairToObject(obj, results, type, m);
            }
        }

        //Recupera una propiedad del objeto
        public void getProperty(Object obj, String name)
        {
            Type type = obj.GetType();
            Model m = ModelManager.getModel(type.Name);

            List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

            Relation r = m.getRelationByName(name);
            if (r != null)
            {
                List<String> relationFields = new List<String>();
                foreach (ForeignKey fk in r.ForeignKeys)
                {
                    relationFields.Add(fk.Name);
                }
                if (r.OneToMany == false)
                {
                    Database db = DatabaseManager.getDatabase(m.PrimaryDatabase);
                    TransactionManager tr = startTransaction(db);

                    List<Tuple<String, Object>> foreingKeysAndValues = tr.getForeignKeysFromOneToOne(m.Name, pks, relationFields);

                    Type ftype = type.GetProperty(r.Name).PropertyType;
                    Object foreignObject = Activator.CreateInstance(ftype);

                    foreach (Tuple<String, Object> fkVal in foreingKeysAndValues)
                    {
                        String fPk = (from fk in r.ForeignKeys where fk.Name == fkVal.Item1 select fk.NameAsPk).First();

                        ftype.GetProperty(fPk).SetValue(foreignObject, fkVal.Item2);
                    }

                    try
                    {
                        get(foreignObject);
                        type.GetProperty(name).SetValue(obj, foreignObject);
                    } catch (GeneralORMException)
                    {
                        type.GetProperty(name).SetValue(obj, null);

                    }
                }
                else
                {
                    Model fmodel = ModelManager.getModel(r.ModelName);
                    Database db = DatabaseManager.getDatabase(fmodel.PrimaryDatabase);

                    List<Tuple<String, Object>> fields = new List<Tuple<String, Object>>();
                    foreach (ForeignKey fk in r.ForeignKeys)
                    {
                        String fieldValue = type.GetProperty(fk.NameAsPk).GetValue(obj).ToString();
                        fields.Add(new Tuple<String, Object>(fk.Name, fieldValue));
                    }

                    TransactionManager tr = startTransaction(db);

                    List<List<Tuple<String, Object>>> results = tr.getWhere(r.ModelName, fields);

                    Type fltype = type.GetProperty(r.Name).PropertyType;
                    Type ftype = fltype.GetGenericArguments()[0];
                    var foreignObjectList = (System.Collections.IList)Activator.CreateInstance(fltype);
                    foreach (List<Tuple<String, Object>> result in results)
                    {
                        Object foreignObject = Activator.CreateInstance(ftype);

                        resPairToObject(foreignObject, result, ftype, fmodel);

                        foreignObjectList.Add(foreignObject);
                    }

                    type.GetProperty(name).SetValue(obj, foreignObjectList);
                }
            }
            else
            {
                Field f = m.getFieldByName(name);

                Database db = DatabaseManager.getDatabase(f.DatabaseName);
                TransactionManager tr = startTransaction(db);

                Object result = tr.getField(m.Name, pks, f.Name);

                type.GetProperty(name).SetValue(obj, result);
            }
        }

        //Recupera todos los objetos de un tipo
        public System.Collections.IList getAll(Type type)
        {
            return getWhere(type, null);
        }

        //Recupera todos los objetos filtrando con campos
        public System.Collections.IList getWhere(Type type, List<Tuple<String, Object>> fieldsAndValues)
        {
            Model m = ModelManager.getModel(type.Name);

            Type ltype = typeof(List<>).MakeGenericType(type);
            var allObjects = (System.Collections.IList)Activator.CreateInstance(ltype);

            Database db = DatabaseManager.getDatabase(m.PrimaryDatabase);
            TransactionManager tr = startTransaction(db);

            List<List<Tuple<String, Object>>> results = tr.getWhere(m.Name, fieldsAndValues);

            foreach (List<Tuple<String, Object>> result in results)
            {
                Object obj = Activator.CreateInstance(type);

                resPairToObject(obj, result, type, m);

                List<Tuple<String, Object>> pks = fieldsToDatabasePairs(obj, m.PrimaryKeys);

                foreach (String dbName in m.databasesUsed())
                {
                    if (dbName == m.PrimaryDatabase) continue;

                    Database secondaryDb = DatabaseManager.getDatabase(dbName);
                    TransactionManager secondTr = startTransaction(secondaryDb);
                    List<Tuple<String, Object>> partialResult = secondTr.get(m.Name, pks);

                    resPairToObject(obj, partialResult, type, m);
                }

                allObjects.Add(obj);
            }

            return allObjects;
        }

        //Recupera el valor maximo de una propiedad
        public Object getMax(Type type, String field)
        {
            Model m = ModelManager.getModel(type.Name);

            Database db = null;
            Field f = (from fields in m.Fields where fields.Name == field select fields).FirstOrDefault();
            if(f == null) {
                f = (from fields in m.PrimaryKeys where fields.Name == field select fields).FirstOrDefault();
                if(f != null)
                {
                    db = DatabaseManager.getDatabase(m.PrimaryDatabase);
                }
                else
                {
                    throw new GeneralORMException("No existe el campo en la base de datos");
                }
            }
            else
            {
                db = DatabaseManager.getDatabase(f.DatabaseName);
            }
            
            TransactionManager tr = startTransaction(db);

            Object result = tr.getMax(m.Name, field);

            return result;
        }
    }
}
