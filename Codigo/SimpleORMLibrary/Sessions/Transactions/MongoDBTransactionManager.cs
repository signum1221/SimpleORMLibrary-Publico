using MongoDB.Driver;
using SimpleORMLibrary.Databases;
using SimpleORMLibrary.GeneralExceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using MongoDB.Bson;

namespace SimpleORMLibrary.Sessions.Transactions
{
    public class MongoDBTransactionManager : TransactionManager
    {
        //Recupera la colección mongo según el nombre de esta
        private IMongoCollection<dynamic> getCollection(String table)
        {
            String connstr = Db.getConnectionString();
            MongoClient client = new MongoClient(connstr);
            return client.GetDatabase(Db.DatabaseName).GetCollection<dynamic>(table);
        }
        //Graba en la base de datos
        public override void save(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields, List<Tuple<String, Object>> foreignKeys)
        {
            var mongoColl = getCollection(table);
            BsonIdManager bsonidman = new BsonIdManager();
            mongoColl.InsertOne(bsonidman);
            var filterInsert = Builders<dynamic>.Filter.Eq("_id", bsonidman._id);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB save operation"); }
            var updateInsert = Builders<dynamic>.Update.Set(pks[0].Item1, pks[0].Item2);
            for (int i = 1; i < pks.Count; i++)
            {
                Tuple<String, Object> pk = pks[i];
                updateInsert = updateInsert.Set(pk.Item1, pk.Item2);
            }
            if (fields != null)
            {
                foreach (Tuple<String, Object> field in fields)
                {
                    updateInsert = updateInsert.Set(field.Item1, field.Item2);
                }
            }
            if (foreignKeys != null)
            {
                foreach (Tuple<String, Object> foreignKey in foreignKeys)
                {
                    updateInsert = updateInsert.Set(foreignKey.Item1, foreignKey.Item2);
                }
            }

            try
            {
                mongoColl.UpdateOne(filterInsert, updateInsert);
            }
            catch (Exception e)
            {
                var filter = Builders<dynamic>.Filter.Eq("_id", bsonidman._id);
                mongoColl.DeleteOne(filter);
                throw new GeneralORMException("An exception of type " + e.GetType() +
                        " was encountered while attempting to save the object to MongoDB.");
            }
        }
        //Borra en la base de datos
        public override void delete(String table, List<Tuple<String, Object>> pks)
        {
            var mongoColl = getCollection(table);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB delete operation"); }
            var filter = Builders<dynamic>.Filter.Eq(pks[0].Item1, pks[0].Item2);

            for (int i = 1; i < pks.Count; i++)
            {
                Tuple<String, Object> pk = pks[i];
                filter = filter & Builders<dynamic>.Filter.Eq(pks[i].Item1, pks[i].Item2);
            }

            mongoColl.DeleteOne(filter);
        }
        //Actualiza en la base de datos
        public override void update(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields)
        {
            var mongoColl = getCollection(table);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB update operation"); }
            var filter = Builders<dynamic>.Filter.Eq(pks[0].Item1, pks[0].Item2);

            for (int i = 1; i < pks.Count; i++)
            {
                Tuple<String, Object> pk = pks[i];
                filter = filter & Builders<dynamic>.Filter.Eq(pks[i].Item1, pks[i].Item2);
            }

            UpdateDefinition<dynamic> updateFields = null;
            if (fields.Count > 0)
            {
                updateFields = Builders<dynamic>.Update.Set(fields[0].Item1, fields[0].Item2);
                for (int i = 1; i < fields.Count; i++)
                {
                    Tuple<String, Object> field = fields[i];
                    updateFields = updateFields.Set(field.Item1, field.Item2);
                }
            }

            if(fields.Count > 0)
                mongoColl.UpdateOne(filter, updateFields);
        }
        //recupera de la base de datos
        public override List<Tuple<String, Object>> get(String table, List<Tuple<String, Object>> pks)
        {
            var mongoColl = getCollection(table);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB get operation"); }
            var filter = Builders<dynamic>.Filter.Eq(pks[0].Item1, pks[0].Item2);

            String fieldsToIgnore = "{_id: 0, _t: 0}";

            for (int i = 1; i < pks.Count; i++)
            {
                Tuple <String, Object> pk = pks[i];
                filter = filter & Builders<dynamic>.Filter.Eq(pks[i].Item1, pks[i].Item2);
            }
            try { 
                var dicSelect = (IDictionary<String, Object>)mongoColl.Find<dynamic>(filter).Project<dynamic>(fieldsToIgnore).First();

                List<Tuple<String, Object>> results = new List<Tuple<String, Object>>();
                foreach(String key in dicSelect.Keys)
                {
                    results.Add(new Tuple<String, Object>(key, dicSelect[key]));
                }
                return results;
            }
            catch (Exception e)
            {
                throw new GeneralORMException("An exception of type " + e.GetType() +
                        " was encountered while attempting to get the object from MongoDB.");
            }
        }
        //recupera de la base de datos un solo campo
        public override Object getField(String table, List<Tuple<String, Object>> pks, String selectedField)
        {
            var mongoColl = getCollection(table);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB getField operation"); }
            var filter = Builders<dynamic>.Filter.Eq(pks[0].Item1, pks[0].Item2);

            String fieldsTofilter = "{_id: 0, _t: 0," + pks[0].Item1 + ": 0";

            for (int i = 1; i < pks.Count; i++)
            {
                fieldsTofilter += ", " + pks[i].Item1 + ": 0";
                Tuple<String, Object> pk = pks[i];
                filter = filter & Builders<dynamic>.Filter.Eq(pks[i].Item1, pks[i].Item2);
            }
            fieldsTofilter = ", " + selectedField + ":1 }";

            try
            {

                var dicSelect = (IDictionary<String, Object>)mongoColl.Find<dynamic>(filter).Project<dynamic>(fieldsTofilter).First();

                foreach (String key in dicSelect.Keys)
                {
                    return dicSelect[key];
                }
                return null;
            }
            catch (Exception e)
            {
                throw new GeneralORMException("An exception of type " + e.GetType() +
                        " was encountered while attempting to geta field of the object from MongoDB.");
            }
        }
        //recupera las claves ajenas de uno a uno y su valor
        public override List<Tuple<String, Object>> getForeignKeysFromOneToOne(String table, List<Tuple<String, Object>> pks, List<String> relationFields)
        {
            var mongoColl = getCollection(table);

            if (pks.Count < 1) { new GeneralORMException("There is no primary keys asociated with the mongoDB getForeignKeysFromOneToOne operation"); }
            var filter = Builders<dynamic>.Filter.Eq(pks[0].Item1, pks[0].Item2);

            String fieldsTofilter = "{_id: 0, _t: 0," + pks[0].Item1 + ": 0";

            for (int i = 1; i < pks.Count; i++)
            {
                fieldsTofilter += ", " + pks[i].Item1 + ": 0";
                Tuple<String, Object> pk = pks[i];
                filter = filter & Builders<dynamic>.Filter.Eq(pks[i].Item1, pks[i].Item2);
            }
            for (int i = 0; i < relationFields.Count; i++)
            {
                fieldsTofilter += ", " + relationFields[i] + ": 1";
            }

            var dicSelect = (IDictionary<String, Object>)mongoColl.Find<dynamic>(filter).Project<dynamic>(fieldsTofilter).First();

            List<Tuple<String, Object>> results = new List<Tuple<String, Object>>();
            foreach (String key in dicSelect.Keys)
            {
                results.Add(new Tuple<String,Object>(key, dicSelect[key]));
            }

            return results;
        }
        //recupera todos los resultados cuyos campos coinciden con los proporcionados
        public override List<List<Tuple<String, Object>>> getWhere(String table, List<Tuple<String, Object>> fields)
        {
            var mongoColl = getCollection(table);

            FilterDefinition<dynamic> filter = null;
            String fieldsTofilter = "{_id: 0, _t: 0}";
            if (fields != null && fields.Count != 0)
            {
                filter = Builders<dynamic>.Filter.Eq(fields[0].Item1, fields[0].Item2);
                
                for (int i = 1; i < fields.Count; i++)
                {
                    Tuple<String, Object> pk = fields[i];
                    filter = filter & Builders<dynamic>.Filter.Eq(fields[i].Item1, fields[i].Item2);
                }
            }
            else
            {
                filter = Builders<dynamic>.Filter.Empty;
            }

            List<List<Tuple<String, Object>>> results = new List<List<Tuple<String, Object>>>();
            foreach (var item in mongoColl.Find<dynamic>(filter).Project<dynamic>(fieldsTofilter).ToList())
            {
                var dicSelect = (IDictionary<String, Object>)item;
                List<Tuple<String, Object>> oneResult = new List<Tuple<String, Object>>();
                foreach (String key in dicSelect.Keys)
                {
                    oneResult.Add(new Tuple<String, Object>(key, dicSelect[key]));
                }
                if(oneResult.Count > 0) { results.Add(oneResult); }
            }

            return results;
        }
        //recupera el máximo del campo seleccionado
        public override Object getMax(String table, String field)
        {
            var mongoColl = getCollection(table);

            String fieldToRecover = "{_id:0, "+field+": 1}";

            try
            {
                var result = (IDictionary<String, Object>)mongoColl.Find(new BsonDocument()).Sort(new BsonDocument("$natural", -1)).Project<dynamic>(fieldToRecover).FirstOrDefault();
                if(result == null) { return null; }
                foreach (String key in result.Keys)
                {
                    return result[key];
                }
                return null;
            }
            catch (Exception e)
            {
                throw new GeneralORMException("An exception of type " + e.GetType() +
                        " was encountered while attempting to get the object from MongoDB.");
            }
        }

    }
}
