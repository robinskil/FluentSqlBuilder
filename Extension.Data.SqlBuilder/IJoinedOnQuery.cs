using System;
using System.Linq.Expressions;

namespace Extension.Data.SqlBuilder
{
    public interface IJoinedOnQuery<T, TJoin> : ISelectOnQuery<T, TJoin>
    {
        IJoinedOnQuery<T, TJoin, TJoin2> Join<TJoin2>(Expression<Func<T, TJoin, TJoin2, bool>> joinOn);
        IConditionalQuery<T, TJoin> Where(Expression<Func<T, TJoin, bool>> expression);
        IGroupedQuery<T, TJoin> GroupBy(Expression<Func<T, TJoin, object>> groupBy);
        IOrderedQuery<T, TJoin> OrderBy(Expression<Func<T, TJoin, object>> orderBy);
    }
    public interface IJoinedOnQuery<T, TJoin, TJoin2> : ISelectOnQuery<T, TJoin, TJoin2>
    {
        IJoinedOnQuery<T, TJoin, TJoin2, TJoin3> Join<TJoin3>(Expression<Func<T, TJoin, TJoin2, TJoin3, bool>> joinOn);
        IConditionalQuery<T, TJoin, TJoin2> Where(Expression<Func<T, TJoin, TJoin2, bool>> expression);
        IGroupedQuery<T, TJoin, TJoin2> GroupBy(Expression<Func<T, TJoin, TJoin2, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2> OrderBy(Expression<Func<T, TJoin, TJoin2, object>> orderBy);
    }
    public interface IJoinedOnQuery<T, TJoin, TJoin2, TJoin3> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3>
    {
        IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4> Join<TJoin4>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, bool>> joinOn);
        IConditionalQuery<T, TJoin, TJoin2, TJoin3> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, bool>> expression);
        IGroupedQuery<T, TJoin, TJoin2, TJoin3> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> orderBy);
    }
    public interface IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
        IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> Join<TJoin5>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, bool>> joinOn);
        IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, bool>> expression);
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> orderBy);
    }
    public interface IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
        IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> Join<TJoin6>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, bool>> joinOn);
        IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, bool>> expression);
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> orderBy);
    }
    public interface IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
        IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, bool>> expression);
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> orderBy);
    }
}
