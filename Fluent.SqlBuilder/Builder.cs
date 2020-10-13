using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Fluent.SqlQuery.QueryStates;
using Fluent.SqlQuery.SqlQueryEngine;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace Fluent.SqlQuery
{
    public static class Builder
    {
        public static ISourceQuery<T> From<T>([NotNull]this SqlConnection dbConnection)
        {
            return new FluentSqlQuery<T>(dbConnection);
        }
    }

    public class FluentSqlQuery<T> : ISourceQuery<T>, ISelectedQuery<T>, IConditionalizedQuery<T>, IDisposable
    {
        private readonly IDbConnection _dbConnection;
        private readonly BaseQueryEngine<T> _queryEngine;

        public FluentSqlQuery(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            _queryEngine = new BaseQueryEngine<T>();
        }
        public ISelectedQuery<T> Select(Expression<Func<T, object>> select)
        {
            _queryEngine.Select(select);
            return this;
        }

        public IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderClause)
        {
            return null;
        }

        public IConditionalizedQuery<T> Where(Expression<Func<T, bool>> where)
        {
            _queryEngine.Where(where);
            return this;
        }

        //public IFluentSqlQuery<T> Top(int amount)
        //{
        //    _queryEngine.Top(amount);
        //    return this;
        //}

        //public IFluentSqlQuery<T> TopPercent(double percent)
        //{
        //    _queryEngine.TopPercent(percent);
        //    return this;
        //}

        public IDbCommand GetDbCommand()
        {
            //_dbConnection.Open();
            using var command = _dbConnection.CreateCommand();
            command.CommandText = _queryEngine.ToSqlQuery();
            foreach (var parameter in _queryEngine.GetParameters())
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public IDataReader ExecuteReader()
        {
            using var command = GetDbCommand();
            return command.ExecuteReader();
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }

        public IEnumerable<T> Execute()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteToReader()
        {
            throw new NotImplementedException();
        }
    }
}
