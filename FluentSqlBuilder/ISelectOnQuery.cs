using System;
using System.Linq.Expressions;

namespace FluentSqlBuilder
{
    public interface ISelectOnQuery<T>
    {
        IFinishedSqlQuery Select(Expression<Func<T, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin, TJoin2>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin, TJoin2, TJoin3>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select, int percentage);
    }
    public interface ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
        IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select);
        IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select, int topAmount);
        IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select, int percentage);
    }
}
