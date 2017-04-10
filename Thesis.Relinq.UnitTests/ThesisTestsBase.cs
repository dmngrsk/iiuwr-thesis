using Xunit;
using Npgsql;
using Thesis.Relinq.NpgsqlWrapper;
using System;

namespace Thesis.Relinq.UnitTests
{
    public class ThesisTestsBase : IDisposable
    {
        protected NpgsqlConnection connection;

        public ThesisTestsBase()
        {
            NpgsqlConnectionAdapter adapter = new NpgsqlConnectionAdapter
            {
                Server = "localhost",
                Port = 5432,
                Username = "dmngrsk",
                Password = "qwerty",
                Database = "northwind"
            };
            
            connection = adapter.GetConnection();
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }
    }
}