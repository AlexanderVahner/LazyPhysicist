using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
using System.Reflection;
using LazyPhysicist.Common;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LazyOptimizer.DB
{
    public class SQLiteService : IDisposable
    {
        private string dbFileName;
        private SQLiteConnection connection;
        private SQLiteTransaction transaction;
        private bool connected = false;
        public SQLiteService(string dbFileName)
        {
            this.dbFileName = dbFileName;
            try
            {
                if (!File.Exists(dbFileName))
                {
                    SQLiteConnection.CreateFile(dbFileName);
                }
                connection = new SQLiteConnection($@"Data Source=""{dbFileName}""");
                Connected = true;
                CreateTables();

                SQLiteFunction.RegisterFunction(typeof(LevenshteinDistanceFunction));
                
            }
            catch (Exception e)
            {
                Logger.Write(this, e.Message, LogMessageType.Error);
            }
        }

        private void CreateTables()
        {
            if (Connected)
            {
                string sql = Properties.Resources.data_db;
                Execute(sql);
            }
        }

        private void AddParametersToCommand(SQLiteCommand command, IEnumerable<object> parameters)
        {
            if (command != null && (parameters?.Count() ?? 0) > 0)
            {
                string debugString = "Parameters: ";
                foreach (object p in parameters)
                {
                    SQLiteParameter parameter = new SQLiteParameter
                    {
                        Value = p
                    };
                    command.Parameters.Add(parameter);
                    debugString += p.ToString() + ", ";
                }
                Logger.Write(this, debugString, LogMessageType.Debug);
            }
            else
            {
                Logger.Write(this, $"Can't add parameters into command.\nSQL: {command?.CommandText ?? "<EMPTY>"}\nParameters count: {parameters?.Count() ?? 0}", LogMessageType.Error);
            }
        }
        public void Select<T>(IList<T> destination, string sqlRequest, IEnumerable<object> parameters = null)
        {
            if (destination != null)
            {
                Type type = typeof(T);

                using (SQLiteCommand command = new SQLiteCommand(sqlRequest, connection))
                {
                    if ((parameters?.Count() ?? 0) > 0)
                    {
                        AddParametersToCommand(command, parameters);
                    }
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T newItem;
                            ConstructorInfo constructor = type.GetConstructors().FirstOrDefault();
                            newItem = (T)constructor.Invoke(new object[] { this });
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                FieldInfo fieldInfo = type.GetFields().FirstOrDefault(fi => fi.Name == reader.GetName(i));
                                if (fieldInfo != null)
                                {
                                    fieldInfo.SetValue(newItem, reader.GetValue(i));
                                }
                                else
                                {
                                    Logger.Write(this, $@"SELECT: Class {type.Name} doesn't have a field named ""{reader.GetName(i)}"".", LogMessageType.Error);
                                }

                            }

                            destination.Add(newItem);
                        }
                    }
                }
            }
            else
            {
                Logger.Write(this, "SELECT: Destination collection is NULL.", LogMessageType.Error);
            }
        }
        public void SelectOne<T>(out T destination, string sqlRequest, IEnumerable<object> parameters = null)
        {
            List<T> list = new List<T>();
            Select(list, sqlRequest, parameters);
            destination = list.FirstOrDefault();
        }
        public void Select<T>(IList<T> destination, IEnumerable<FilterCondition> conditions, string orderBy = "")
        {
            if (destination != null)
            {
                Type type = typeof(T);
                DBTableNameAttribute tableNameAttr = type.GetCustomAttributes(typeof(DBTableNameAttribute), false).FirstOrDefault() as DBTableNameAttribute;
                string tableName = tableNameAttr?.DBName ?? "";
                if (tableName != "")
                {
                    string sqlString = $"SELECT rowid, * FROM {tableName} ";
                    if (conditions.Count() > 0)
                    {
                        sqlString += "WHERE ";
                        foreach (FilterCondition c in conditions)
                        {
                            sqlString += c.ConditionString + " AND ";
                        }
                        sqlString += " (1 = 1) ";
                    }
                    if (orderBy != "")
                    {
                        sqlString += "ORDER BY " + orderBy;
                    }

                    sqlString += ";";

                    Select(destination, sqlString);
                }
                else
                {
                    Logger.Write(this, $"SELECT: Class {type.Name} doesn't have a correct DBTableName Attribute.", LogMessageType.Error);
                }

            }
            else
            {
                Logger.Write(this, "SELECT: Destination collection is NULL.", LogMessageType.Error);
            }
        }
        public int Execute(string sqlRequest, IEnumerable<object> parameters = null)
        {
            int result = 0;
            using (SQLiteCommand command = new SQLiteCommand(sqlRequest, connection))
            {
                Logger.Write(this, sqlRequest, LogMessageType.Debug);

                if ((parameters?.Count() ?? 0) > 0)
                {
                    AddParametersToCommand(command, parameters);
                }

                result = command.ExecuteNonQuery();
            }
            return result;
        }
        public object GetValue(string sqlRequest, IEnumerable<object> parameters = null)
        {
            object result = null;
            using (SQLiteCommand command = new SQLiteCommand(sqlRequest, connection))
            {
                Logger.Write(this, sqlRequest, LogMessageType.Debug);

                if ((parameters?.Count() ?? 0) > 0)
                {
                    AddParametersToCommand(command, parameters);
                }

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetValue(0);
                    }
                }
            }
            return result;
        }
        public int SetRecordValue(string tableName, long rowId, string fieldName, object value)
        {
            string sql = $"UPDATE {tableName} SET {fieldName} = ? WHERE rowid = ?";
            return Execute(sql, new object[] { value, rowId });
        }
        public int LastInsertedRowId()
        {
            return (GetValue("TYPES INT; SELECT last_insert_rowid();") as int?) ?? -1;
        }
        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            transaction.Commit();
        }
        public void RollbackTransaction()
        {
            transaction.Rollback();
        }
        public void Dispose()
        {
            connection.Dispose();
        }
        public bool Connected
        {
            get => connected;
            set
            {
                if (value)
                {
                    connection.Open();
                    connected = true;
                }
                else
                {
                    connection.Close();
                    connected = false;
                }
                
            }
        }
    }
}
