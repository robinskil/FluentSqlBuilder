using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Fluent.SqlQuery.QueryStates
{
    public interface ISourceQuery<T>
    {
        IConditionalizedQuery<T> Where(Expression<Func<T, bool>> whereClause);
        ISelectedQuery<T> Select(Expression<Func<T, object>> selectClause);
        IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderClause);
    }

    public interface IConditionalizedQuery<T>
    {
        ISelectedQuery<T> Select(Expression<Func<T, object>> selectClause);
        IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderClause);
    }

    public interface IOrderedQuery<T>
    {
        ISelectedQuery<T> Select(Expression<Func<T, object>> selectClause);
    }

    public interface ISelectedQuery<out T>
    {
        IEnumerable<T> Execute();
        IDataReader ExecuteToReader();
    }
}
