using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Fluent.SqlQuery.SqlExtensions;
using Xunit;

namespace Fluent.SqlQuery.Tests
{
    public class WhereTests
    {
        [Fact]
        public void TestSqlWhereMemberParamQuery()
        {
            int minAge = 5;
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > minAge)
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(minAge,par.Value);
        }

        [Fact]
        public void TestSqlWhereParamQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > 5)
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(5, par.Value);
        }

        [Fact]
        public void TestSqlWhereParamInObjectQuery()
        {
            var obj = new { age = 5 };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > obj.age)
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(obj.age, par.Value);
        }

        [Fact]
        public void TestSqlWhereParamInNestedObjectQuery()
        {
            var obj = new { person = new {age = 5} };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > obj.person.age)
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(obj.person.age, par.Value);
        }

        [Fact]
        public void TestSqlWhereParamFunctionCallQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > GetAge())
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(GetAge(), par.Value);
        }

        [Fact]
        public void TestSqlWhereParamStaticFunctionCallQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > GetStaticAge())
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(GetStaticAge(), par.Value);
        }

        [Fact]
        public void TestSqlWhereParamFunctionCallWithParametersQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > ReturnAge(5))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(ReturnAge(5), par.Value);
        }

        [Fact]
        public void TestSqlWhereParamStaticFunctionCallWithParametersQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age < ReturnStaticAge(5))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] < @param0", result.CommandText);
            Assert.Equal(ReturnStaticAge(5), par.Value);
        }

        [Fact]
        public void TestSqlWhereParamFunctionCallWithObjectParametersQuery()
        {
            var obj = new {age = 5};
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > ReturnAge(obj.age))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(ReturnAge(obj.age), par.Value);
        }

        [Fact]
        public void TestSqlWhereParamFunctionCallWithNestedFunctionParametersQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age == ReturnAge(GetAge()))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] = @param0", result.CommandText);
            Assert.Equal(ReturnAge(GetAge()), par.Value);
        }


        [Fact]
        public void TestSqlWhereParamFunctionCallWithNestedObjectParametersQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age > ReturnAge(obj.person.age))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] > @param0", result.CommandText);
            Assert.Equal(ReturnAge(obj.person.age), par.Value);
        }


        [Fact]
        public void TestSqlWhereIsNullQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.IsNull())
                .Select(a => new { a })
                .GetDbCommand();
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] IS NULL", result.CommandText);
        }

        [Fact]
        public void TestSqlWhereIsNotNullQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.IsNotNull())
                .Select(a => new { a })
                .GetDbCommand();
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] IS NOT NULL", result.CommandText);
        }

        [Fact]
        public void TestSqlWhereLikeQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Name.Like("%s%"))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Name] LIKE @param0", result.CommandText);
            Assert.Equal("%s%", par.Value);
        }

        [Fact]
        public void TestSqlWhereNotLikeQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Name.NotLike("%s%"))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Name] NOT LIKE @param0", result.CommandText);
            Assert.Equal("%s%", par.Value);
        }

        [Fact]
        public void TestSqlWhereBetweenQuery()
        {
            var obj = new { person = new { age = 5 } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.Between(25,45))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] BETWEEN @param0 AND @param1", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
        }

        [Fact]
        public void TestSqlWhereNotBetweenQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.NotBetween(25, 45))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] NOT BETWEEN @param0 AND @param1", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
        }

        [Fact]
        public void TestSqlWhereInQuery()
        {
            var obj = new { person = new { ages = new int[]{25,45} } };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.In(obj.person.ages))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] IN (@param0,@param1)", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
        }

        [Fact]
        public void TestSqlWhereInParameterQuery()
        {
            var obj = new { person = new { age = 5 } };
            var param = new[] {25, 45};
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.In(param))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] IN (@param0,@param1)", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
        }

        [Fact]
        public void TestSqlWhereNotInParameterQuery()
        {
            var param = new[] { 25, 45 };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.NotIn(param))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] NOT IN (@param0,@param1)", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
        }

        [Fact]
        public void TestSqlWhereNotParameterQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age != 25)
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] != @param0", result.CommandText);
            Assert.Equal(25, par.Value);
        }

        [Fact]
        public void TestSqlWhereAndParameterQuery()
        {
            var obj = new { person = new { age = 5 } };
            var param = new[] { 25, 45 };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age >= 25 && a.Name == "test")
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] >= @param0 AND [Person].[Name] = @param1", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal("test", par1.Value);
        }

        [Fact]
        public void TestSqlWhereAndIsNotNullParameterQuery()
        {
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age == 25 && a.Id.IsNotNull())
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] = @param0 AND [Person].[Id] IS NOT NULL", result.CommandText);
            Assert.Equal(25, par.Value);
        }

        [Fact]
        public void TestSqlWhereInAndParameterQuery()
        {
            var obj = new { person = new { age = 5 } };
            var param = new[] { 25, 45 };
            var connection = new SqlConnection("");
            var result = connection.From<Person>()
                .Where(a => a.Age.In(param) && a.Name.Like("%t%"))
                .Select(a => new { a })
                .GetDbCommand();
            var par = result.Parameters["@param0"] as SqlParameter;
            var par1 = result.Parameters["@param1"] as SqlParameter;
            var par2 = result.Parameters["@param2"] as SqlParameter;
            Assert.Equal("SELECT [Person].[Name] , [Person].[Age] , [Person].[Id] \n FROM [Person] \n WHERE [Person].[Age] IN (@param0,@param1) AND [Person].[Name] LIKE @param2", result.CommandText);
            Assert.Equal(25, par.Value);
            Assert.Equal(45, par1.Value);
            Assert.Equal("%t%", par2.Value);
        }

        private int GetAge()
        {
            return 5;
        }

        private static int GetStaticAge()
        {
            return 5;
        }

        private int ReturnAge(int x)
        {
            return x;
        }

        private static int ReturnStaticAge(int x)
        {
            return x;
        }

    }
}
