using System;
using System.Linq.Expressions;

namespace FluentSqlBuilder
{
    public interface IBaseQuery<T> : ISelectOnQuery<T>
    {
        IJoinedOnQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> joinOn);
        IConditionalQuery<T> Where(Expression<Func<T, bool>> expression);
        IGroupedQuery<T> GroupBy(Expression<Func<T, object>> groupBy);
        IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderBy);
    }
}
