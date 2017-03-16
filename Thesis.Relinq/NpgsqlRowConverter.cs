using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Npgsql;

namespace Thesis.Relinq
{
    public class NpgsqlRowConverter<T>
    {
        private static PropertyInfo[] _propertiesInfo = typeof(T).GetProperties();
        private static ReadOnlyCollection<Npgsql.Schema.NpgsqlDbColumn> _columns;

        public NpgsqlRowConverter(ReadOnlyCollection<Npgsql.Schema.NpgsqlDbColumn> columns)
        {
            _columns = columns;
        }

        public static IEnumerable<T> ReadAllRows(NpgsqlConnection connection, string query)
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = query;
            return ReadAllRows(connection, command);
        }

        public static IEnumerable<T> ReadAllRows(NpgsqlConnection connection, NpgsqlCommand command)
        {
            connection.Open();
            List<T> rows = new List<T>();

            using (var reader = command.ExecuteReader())
            {
                var columnSchema = reader.GetColumnSchema();
                var rowConverter = new NpgsqlRowConverter<T>(columnSchema);

                while (reader.Read())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    
                    var obj = rowConverter.ConvertArrayToObject(row);
                    rows.Add(obj);
                }
            }

            connection.Close();
            command.Dispose();
            return rows;
        }

        private T ConvertArrayToObject(object[] row)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
                    
            for (int i = 0; i < _columns.Count; i++)
            {
                var prop = typeof(T).GetProperty(_columns[i].ColumnName);
                var propType = prop.PropertyType;
                
                try {
                    prop.SetValue(obj, Convert.ChangeType(row[i], propType));
                } catch (InvalidCastException) { /* Value is null */ }
            }

            return obj;
        }
    }
}