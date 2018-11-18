using System;
using System.Collections.Generic;
using System.Text;
using SimpleORMLibrary.Databases;
using SimpleORMLibrary.GeneralExceptions;
using SimpleORMLibrary.Models;

namespace SimpleORMLibrary.Sessions.Transactions
{
    public abstract class TransactionManager
    {
        //Base de datos con la que se conecta
        public Database Db { get; set; }

        //Graba en la base de datos
        public abstract void save(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields, List<Tuple<String, Object>> foreignKeys);
        //Borra en la base de datos
        public abstract void delete(String table, List<Tuple<String, Object>> pks);
        //Actualiza en la base de datos
        public abstract void update(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields);
        //recupera de la base de datos
        public abstract List<Tuple<String, Object>> get(String table, List<Tuple<String, Object>> pks);
        //recupera de la base de datos un solo campo
        public abstract Object getField(String table, List<Tuple<String, Object>> pks, String selectedField);
        //recupera las claves ajenas de uno a uno y su valor
        public abstract List<Tuple<String, Object>> getForeignKeysFromOneToOne(String table, List<Tuple<String, Object>> pks, List<String> relationFields);
        //recupera todos los resultados cuyos campos coinciden con los proporcionados
        public abstract List<List<Tuple<String, Object>>> getWhere(String table, List<Tuple<String, Object>> fields);
        //recupera el máximo del campo seleccionado
        public abstract Object getMax(String table, String field);
    }
}
