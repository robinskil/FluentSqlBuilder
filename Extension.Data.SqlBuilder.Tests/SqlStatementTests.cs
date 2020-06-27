using Extension.Data.SqlBuilder.SqlExtensions;
using Extension.Data.SqlBuilder.Tests.POCOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Xunit;

namespace Extension.Data.SqlBuilder.Tests
{
    public class SqlStatementTests
    {
        [Fact]
        public void SelectQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .Select(person => new { person.Id })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id] FROM [Person]", statement);
            }
        }
        [Fact]
        public void SelectWhereQuery()
        {
            using (var connection = new SqlConnection())
            {
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Salary > 5000)
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Salary] > @P1", command.CommandText);
                Assert.Equal(5000D,command.Parameters["@P1"].Value);
            }
            using (var connection = new SqlConnection())
            {
                double val = 5000;
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Salary > val)
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Salary] > @P1", command.CommandText);
                Assert.Equal(val, command.Parameters["@P1"].Value);
            }
            using (var connection = new SqlConnection())
            {
                var testObj = new { val = 5000D };
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Salary > testObj.val)
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Salary] > @P1", command.CommandText);
                Assert.Equal(testObj.val, command.Parameters["@P1"].Value);
            }
        }
        [Fact]
        public void SelectWhereInQuery()
        {
            using (var connection = new SqlConnection())
            {
                IEnumerable<int> ages = new int[] { 1, 2 };
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.In(ages))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
            using (var connection = new SqlConnection())
            {
                var testObj = new { t = new int[] { 1, 2 } };
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.In(testObj.t))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
            using (var connection = new SqlConnection())
            {
                int age2 = 2;
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.In(new int[] { 1, age2 }))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
        }

        [Fact]
        public void SelectWhereNotInQuery()
        {
            using (var connection = new SqlConnection())
            {
                IEnumerable<int> ages = new int[] { 1, 2 };
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.NotIn(ages))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] NOT IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
            using (var connection = new SqlConnection())
            {
                var testObj = new { t = new int[] { 1, 2 } };
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.NotIn(testObj.t))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] NOT IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
            using (var connection = new SqlConnection())
            {
                int age2 = 2;
                var command = connection.Build()
                                .From<Person>()
                                .Where((person) => person.Age.NotIn(new int[] { 1, age2 }))
                                .Select(person => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] WHERE [Person].[Age] NOT IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
        }

        [Fact]
        public void SelectGroupByQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .GroupBy((person) => new { person.Age })
                                .Select(person => new { person.Id, person.Name })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] GROUP BY [Person].[Age]", statement);
            }
        }
        [Fact]
        public void SelectMultipleGroupByQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .GroupBy((person) => new { person.Age , person.Name })
                                .Select(person => new { person.Id, person.Name })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] GROUP BY [Person].[Age], [Person].[Name]", statement);
            }
        }
        [Fact]
        public void SelectOrderByQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .OrderBy((person) => new { person.Age })
                                .Select(person => new { person.Id, person.Name })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] ORDER BY [Person].[Age]", statement);
            }
        }
        [Fact]
        public void SelectMultipleOrderByQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .OrderBy((person) => new { person.Age , person.Salary })
                                .Select(person => new { person.Id, person.Name })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person] ORDER BY [Person].[Age], [Person].[Salary]", statement);
            }
        }
        [Fact]
        public void SelectJoinQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .Join<Work>((person,work) => person.WorkId == work.WorkId)
                                .Select((person, work) => new { person.Id, person.Name, work.Address })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name], [Work].[Address] FROM [Person] JOIN [Work] AS [Work] ON [Person].[WorkId] = [Work].[WorkId]", statement);
            }
        }
        [Fact]
        public void SelectSelfJoinQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .Join<Person>((person, person2) => person.WorkId == person2.WorkId)
                                .Select((person, person2) => new { person.Id, person2.Name })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person1].[Name] FROM [Person] JOIN [Person] AS [Person1] ON [Person].[WorkId] = [Person1].[WorkId]", statement);
            }
        }
        [Fact]
        public void SelectMultipleSelfJoinQuery()
        {
            using (var connection = new SqlConnection())
            {
                var statement = connection.Build()
                                .From<Person>()
                                .Join<Person>((person, person2) => person.WorkId == person2.WorkId)
                                .Join<Person>((person, person2,person3) => person.Name == person3.Name)
                                .Select((person, person2, person3) => new { person.Id, person2.Name , person3.Age })
                                .GetSqlStatement();
                Assert.Equal("SELECT [Person].[Id], [Person1].[Name], [Person2].[Age] FROM [Person] " +
                    "JOIN [Person] AS [Person1] ON [Person].[WorkId] = [Person1].[WorkId] " +
                    "JOIN [Person] AS [Person2] ON [Person].[Name] = [Person2].[Name]", statement);
            }
        }
        [Fact]
        public void QueryAll()
        {
            using (var connection = new SqlConnection())
            {
                int age2 = 2;
                var command = connection.Build()
                                .From<Person>()
                                .Join<Work>((person,work) => person.WorkId == work.WorkId)
                                .Join<Person>((person, work,person2)=> person.Id == person2.Id)
                                .Where((person, work, person2) => person2.Age.In(new int[] { 1, age2 }))
                                .Select((person, work, person2) => new { person.Id, person.Name })
                                .ToSqlCommand();
                Assert.Equal("SELECT [Person].[Id], [Person].[Name] FROM [Person]" +
                    " JOIN [Work] AS [Work] ON [Person].[WorkId] = [Work].[WorkId]" +
                    " JOIN [Person] AS [Person1] ON [Person].[Id] = [Person1].[Id]" +
                    " WHERE [Person1].[Age] IN (@P1,@P2)", command.CommandText);
                Assert.Equal(1, command.Parameters["@P1"].Value);
                Assert.Equal(2, command.Parameters["@P2"].Value);
            }
        }
    }
}
