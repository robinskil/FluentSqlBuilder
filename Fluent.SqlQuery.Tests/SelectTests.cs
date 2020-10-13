using System;
using System.Data.SqlClient;
using Xunit;
using Fluent.SqlQuery;

namespace Fluent.SqlQuery.Tests
{
    public class SelectTests
    {
        [Fact]
        public void TestSqlSelectQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Select(a => new { a })
                .GetDbCommand();
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person]", result.CommandText);
        }

        [Fact]
        public void TestSqlSelectColumnsQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Select(a => new { a.Age, a.Id })
                .GetDbCommand();
            Assert.Equal("SELECT [Person].[Age] , [Person].[Id] \n FROM [Person]", result.CommandText);
        }

        [Fact]
        public void TestSqlTop500SelectQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Top(500)
                .Select(a => new { a.Age, a.Id })
                .GetDbCommand();
            Assert.Equal("SELECT TOP 500 [Person].[Age] , [Person].[Id] \n FROM [Person]", result.CommandText);
        }

        [Fact]
        public void TestSqlTop50PercentSelectQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .TopPercent(50)
                .Select(a => new { a.Age, a.Id })
                .GetDbCommand();
            Assert.Equal("SELECT TOP 50 PERCENT [Person].[Age] , [Person].[Id] \n FROM [Person]", result.CommandText);
        }

        [Fact]
        public void TestThrowSqlEmptySelectQuery()
        {
            var connection = new SqlConnection("");
            Assert.Throws<Exception>(() => connection.From<Person>()
                .TopPercent(50)
                .Select(a => new { })
                .GetDbCommand());
        }
    }
}
