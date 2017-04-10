using Xunit;
using System.Linq;
using Thesis.Relinq.NpgsqlWrapper;
using Thesis.Relinq.UnitTests.Models;

namespace Thesis.Relinq.UnitTests
{
    public class ResultOperatorTests : ThesisTestsBase
    {
        [Fact]
        public void select_count()
        {
            // Arrange
            var myQuery = 
                from c in PsqlQueryFactory.Queryable<Customers>(connection)
                select c;

            var myQuery2 = PsqlQueryFactory.Queryable<Customers>(connection)
                .Select(c => c);
            
            string psqlCommand = "SELECT COUNT(*) FROM Customers;";

            // Act
            var expected = NpgsqlRowConverter<int>.ReadAllRows(connection, psqlCommand).Single();
            var actual = myQuery.Count();
            var actual2 = myQuery2.Count();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected, actual2);
        }

        [Fact]
        public void select_average()
        {
            // Arrange
            var myQuery = 
                from e in PsqlQueryFactory.Queryable<Employees>(connection)
                select new decimal(e.EmployeeID);

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Select(e => new decimal(e.EmployeeID));
            
            string psqlCommand = "SELECT AVG(\"EmployeeID\") FROM Employees;";

            // Act
            var expected = NpgsqlRowConverter<decimal>.ReadAllRows(connection, psqlCommand).Single();
            var actual = myQuery.Average();
            var actual2 = myQuery2.Average();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected, actual2);
        }

        [Fact]
        public void select_sum()
        {
            // Arrange
            var myQuery = 
                from e in PsqlQueryFactory.Queryable<Employees>(connection)
                select (int)e.EmployeeID;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Select(e => (int)e.EmployeeID);
            
            string psqlCommand = "SELECT SUM(\"EmployeeID\") FROM Employees;";

            // Act
            var expected = NpgsqlRowConverter<int>.ReadAllRows(connection, psqlCommand).Single();
            var actual = myQuery.Sum();
            var actual2 = myQuery2.Sum();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected, actual2);
        }
        
        [Fact]
        public void select_min()
        {
            // Arrange
            var myQuery = 
                from e in PsqlQueryFactory.Queryable<Employees>(connection)
                select (int)e.EmployeeID;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Select(e => (int)e.EmployeeID);
            
            string psqlCommand = "SELECT MIN(\"EmployeeID\") FROM Employees;";

            // Act
            var expected = NpgsqlRowConverter<int>.ReadAllRows(connection, psqlCommand).Single();
            var actual = myQuery.Min();
            var actual2 = myQuery2.Min();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected, actual2);
        }

        [Fact]
        public void select_max()
        {
            // Arrange
            var myQuery = 
                from e in PsqlQueryFactory.Queryable<Employees>(connection)
                select (int)e.EmployeeID;

            var myQuery2 = PsqlQueryFactory.Queryable<Employees>(connection)
                .Select(e => (int)e.EmployeeID);
            
            string psqlCommand = "SELECT MAX(\"EmployeeID\") FROM Employees;";

            // Act
            var expected = NpgsqlRowConverter<int>.ReadAllRows(connection, psqlCommand).Single();
            var actual = myQuery.Max();
            var actual2 = myQuery2.Max();

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expected, actual2);
        }
    }
}