using MySql.Data.MySqlClient;
using SimpleORMLibrary.Databases;
using SimpleORMLibrary.GeneralExceptions;
using SimpleORMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleORMLibrary.Sessions.Transactions
{
    class MySQLTransactionManager : TransactionManager
    {
        //Graba en la base de datos
        public override void save(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields, List<Tuple<String, Object>> foreignKeys)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            //Create command string
            List<String> columns = new List<String>();
            List<Object> values = new List<Object>();

            foreach (Tuple<String, Object> pair in pks)
            {
                columns.Add(pair.Item1);
                values.Add(pair.Item2);
            }
            if (fields != null)
            {
                foreach (Tuple<String, Object> pair in fields)
                {
                    columns.Add(pair.Item1);
                    values.Add(pair.Item2);
                }
            }
            if(foreignKeys != null)
            {
                foreach (Tuple<String, Object> pair in foreignKeys)
                {
                    columns.Add(pair.Item1);
                    values.Add(pair.Item2);
                }
            }

            try
            {
                String query = "Insert into " + table + " (";
                for (int i = 0; i < columns.Count; i++)
                {
                    query += columns[i];
                    if(i != columns.Count - 1)
                        query += ", ";
                }
                query += ") ";

                query += "Values (";
                for (int i = 0; i < columns.Count; i++)
                {
                    query += "@"+columns[i];
                    if (i != columns.Count - 1)
                        query += ", ";
                }
                query += ") ";
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < columns.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + columns[i], values[i]);
                }

                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while inserting the data. Neither record was written to database.");
            }
            finally
            {
                conn.Close();
            }
        }
        //Borra en la base de datos
        public override void delete(String table, List<Tuple<String, Object>> pks)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            try
            {
                String query = "Delete from " + table + " where ";

                for (int i = 0; i < pks.Count; i++)
                {
                    query += pks[i].Item1 + " = @" + pks[i].Item1;
                    if (i != pks.Count - 1)
                        query += " and ";
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < pks.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + pks[i].Item1, pks[i].Item2);
                }

                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while deleting the data.");
            }
            finally
            {
                conn.Close();
            }
        }
        //Actualiza en la base de datos
        public override void update(String table, List<Tuple<String, Object>> pks, List<Tuple<String, Object>> fields)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            try
            {
                String query = "Update " + table + " Set ";
                for (int i = 0; i < fields.Count; i++)
                {
                    query += fields[i].Item1 + " = @" + fields[i].Item1;
                    if (i != fields.Count - 1)
                        query += " , ";
                }
                query += " where ";
                for (int i = 0; i < pks.Count; i++)
                {
                    query += pks[i].Item1 + " = @" + pks[i].Item1;
                    if (i != pks.Count - 1)
                        query += " and ";
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < fields.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + fields[i].Item1, fields[i].Item2);
                }
                for (int i = 0; i < pks.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + pks[i].Item1, pks[i].Item2);
                }

                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while updating the data.");
            }
            finally
            {
                conn.Close();
            }
        }
        //recupera de la base de datos
        public override List<Tuple<String, Object>> get(String table, List<Tuple<String, Object>> pks)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            List<Tuple<String, Object>> results = new List<Tuple<String, Object>>();
            try
            {
                String query = "select * from " + table + " where ";
                for (int i = 0; i < pks.Count; i++)
                {
                    query += pks[i].Item1 + " = @" + pks[i].Item1;
                    if (i != pks.Count - 1)
                        query += " and ";
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < pks.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + pks[i].Item1, pks[i].Item2);
                }

                MySqlDataReader dataReader = command.ExecuteReader();

                dataReader.Read();
                for(int i = 0; i < dataReader.FieldCount; i++)
                {
                    results.Add(new Tuple<String, Object>(dataReader.GetName(i), dataReader.GetValue(i)));
                }

                dataReader.Close();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while recovering the data.");
            }
            finally
            {
                conn.Close();
            }

            return results;
        }
        //recupera de la base de datos un solo campo
        public override Object getField(String table, List<Tuple<String, Object>> pks, String selectedField)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            Object result;
            try
            {
                String query = "select " + selectedField + " from " + table + " where ";
                for (int i = 0; i < pks.Count; i++)
                {
                    query += pks[i].Item1 + " = @" + pks[i].Item1;
                    if (i != pks.Count - 1)
                        query += " and ";
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < pks.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + pks[i].Item1, pks[i].Item2);
                }

                MySqlDataReader dataReader = command.ExecuteReader();

                dataReader.Read();

                result = dataReader.GetValue(0);

                dataReader.Close();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while recovering the data.");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
        //recupera las claves ajenas de uno a uno y su valor
        public override List<Tuple<String, Object>> getForeignKeysFromOneToOne(String table, List<Tuple<String, Object>> pks, List<String> relationFields)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            List<Tuple<String, Object>> results = new List<Tuple<String, Object>>();
            try
            {
                String query = "select ";
                for (int i = 0; i < relationFields.Count -1; i++)
                {
                    query += relationFields[i] + ",";
                }
                query += relationFields[relationFields.Count - 1];
                query += " from " + table + " where ";
                for (int i = 0; i < pks.Count; i++)
                {
                    query += pks[i].Item1 + " = @" + pks[i].Item1;
                    if (i != pks.Count - 1)
                        query += " and ";
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                for (int i = 0; i < pks.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + pks[i].Item1, pks[i].Item2);
                }

                MySqlDataReader dataReader = command.ExecuteReader();

                dataReader.Read();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if(dataReader.GetValue(i) is System.DBNull)
                        results.Add(new Tuple<String, Object>(dataReader.GetName(i), null));
                    else
                        results.Add(new Tuple<String, Object>(dataReader.GetName(i), dataReader.GetValue(i)));
                }

                dataReader.Close();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while recovering the data.");
            }
            finally
            {
                conn.Close();
            }

            return results;
        }
        //recupera todos los resultados cuyos campos coinciden con los proporcionados
        public override List<List<Tuple<String, Object>>> getWhere(String table, List<Tuple<String, Object>> fields)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            List<List<Tuple<String, Object>>> results = new List<List<Tuple<String, Object>>>();
            try
            {
                String query = "select * from " + table;
                if (fields != null && fields.Count != 0)
                {
                    query += " where ";
                    for (int i = 0; i < fields.Count; i++)
                    {
                        query += fields[i].Item1 + " = @" + fields[i].Item1;
                        if (i != fields.Count - 1)
                            query += " and ";
                    }
                }
                query += ";";

                command.CommandText = query;
                command.Prepare();
                if (fields != null && fields.Count != 0)
                {
                    for (int i = 0; i < fields.Count; i++)
                    {
                        command.Parameters.AddWithValue("@" + fields[i].Item1, fields[i].Item2);
                    }
                }

                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    List<Tuple<String, Object>> result = new List<Tuple<String, Object>>();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        result.Add(new Tuple<String, Object>(dataReader.GetName(i), dataReader.GetValue(i)));
                    }
                    results.Add(result);
                }

                dataReader.Close();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while recovering the data.");
            }
            finally
            {
                conn.Close();
            }

            return results;
        }
        //recupera el máximo del campo seleccionado
        public override Object getMax(String table, String field)
        {
            MySqlConnection conn = new MySqlConnection(Db.getConnectionString());
            conn.Open();

            MySqlCommand command = conn.CreateCommand();
            MySql.Data.MySqlClient.MySqlTransaction transaction = conn.BeginTransaction();

            command.Connection = conn;
            command.Transaction = transaction;

            Object result = null;
            try
            {
                String query = "select max("+field+") from " + table + ";";

                command.CommandText = query;
                command.Prepare();

                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    result = dataReader.GetValue(0);
                }

                if(result == System.DBNull.Value) { result = null; }

                dataReader.Close();

                transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    if (transaction.Connection != null)
                    {
                        throw new GeneralORMException("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                throw new GeneralORMException("An exception of type " + e.GetType() +
                " was encountered while recovering the data.");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

    }
}
