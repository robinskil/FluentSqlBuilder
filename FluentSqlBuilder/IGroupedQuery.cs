using System;
using System.Linq.Expressions;

namespace FluentSqlBuilder
{
    public interface IGroupedQuery<T> : ISelectOnQuery<T>
    {
        IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin> : ISelectOnQuery<T, TJoin>
    {
        IOrderedQuery<T, TJoin> OrderBy(Expression<Func<T, TJoin, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin, TJoin2> : ISelectOnQuery<T, TJoin, TJoin2>
    {
        IOrderedQuery<T, TJoin, TJoin2> OrderBy(Expression<Func<T, TJoin, TJoin2, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin, TJoin2, TJoin3> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3>
    {
        IOrderedQuery<T, TJoin, TJoin2, TJoin3> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> orderBy);
    }
    public interface IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
        IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> orderBy);
    }
}
