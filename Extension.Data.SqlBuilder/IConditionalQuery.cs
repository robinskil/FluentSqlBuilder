using System;
using System.Linq.Expressions;

namespace Extension.Data.SqlBuilder
{
    public interface IConditionalQuery<T> : ISelectOnQuery<T>
    {
        IGroupedQuery<T> GroupBy(Expression<Func<T, object>> groupBy);
        IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin> : ISelectOnQuery<T, TJoin>
    {
        IGroupedQuery<T, TJoin> GroupBy(Expression<Func<T, TJoin, object>> groupBy);
        IOrderedQuery<T, TJoin> OrderBy(Expression<Func<T, TJoin, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin, TJoin2> : ISelectOnQuery<T, TJoin, TJoin2>
    {
        IGroupedQuery<T, TJoin, TJoin2> GroupBy(Expression<Func<T, TJoin, TJoin2, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2> OrderBy(Expression<Func<T, TJoin, TJoin2, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin, TJoin2, TJoin3> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3>
    {
        IGroupedQuery<T, TJoin, TJoin2, TJoin3> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> orderBy);
    }
    public interface IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
        IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> groupBy);
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> orderBy);
    }
}
