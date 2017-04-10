using Xunit;
using System.Linq;
using Thesis.Relinq.NpgsqlWrapper;
using Thesis.Relinq.UnitTests.Models;

namespace Thesis.Relinq.UnitTests
{
    public class BinaryExpressionTests : ThesisTestsBase
    {
        [Fact]
        public void equal()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID == 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID == 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" = 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void not_equal()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID != 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID != 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" != 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void greater_than()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID > 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID > 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" > 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void greater_than_or_equal()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID >= 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID >= 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" >= 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void less_than()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID < 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID < 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" < 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void less_than_or_equal()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID <= 7
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID <= 7);
            
            string psqlCommand = "SELECT * FROM Employees WHERE \"EmployeeID\" <= 7;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void add()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID + 5) < 10
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID + 5) < 10);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" + 5) < 10;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void substract()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID - 5) > 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID - 5) > 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" - 5) > 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void multiply()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID * 2) < 10
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID * 2) < 10);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" * 2) < 10;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void divide()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID / 5) > 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID / 5) > 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" / 5) > 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void modulo()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID % 2) == 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID % 2) == 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" % 2) = 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void bitwise_and()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID & 4) == 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID & 4) == 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" & 4) = 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void bitwise_or()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID | 1) != c.EmployeeID
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID | 1) != c.EmployeeID);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" | 1) != \"EmployeeID\";";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void bitwise_xor()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID ^ 2) == 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID ^ 2) == 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" # 2) = 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void bitwise_shift_left()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID << 2) < 16
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID << 2) < 16);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" << 2) < 16;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void bitwise_shift_right()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where (c.EmployeeID >> 3) > 0
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => (c.EmployeeID >> 3) > 0);
            
            string psqlCommand = "SELECT * FROM Employees WHERE (\"EmployeeID\" >> 3) > 0;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void logical_and()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID < 7 && c.EmployeeID > 3
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID < 7 && c.EmployeeID > 3);
            
            string psqlCommand = "SELECT * FROM Employees " +
                "WHERE \"EmployeeID\" < 7 AND \"EmployeeID\" > 3;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }

        [Fact]
        public void logical_or()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Employees>(connection)
                where c.EmployeeID > 7 || c.EmployeeID < 3
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Where(c => c.EmployeeID > 7 || c.EmployeeID < 3);
            
            string psqlCommand = "SELECT * FROM Employees " +
                "WHERE \"EmployeeID\" > 7 OR \"EmployeeID\" < 3;";

            // Act
            var expected = NpgsqlRowConverter<Employees>.ReadAllRows(connection, psqlCommand).ToArray();
            var actual = myQuery.ToArray();
            var actual2 = myQuery2.ToArray();

            // Assert
            AssertExtension.EqualByJson(expected, actual);
            AssertExtension.EqualByJson(expected, actual2);
        }
    }
}