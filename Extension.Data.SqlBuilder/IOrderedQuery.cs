namespace Extension.Data.SqlBuilder
{
    public interface IOrderedQuery<T> : ISelectOnQuery<T>
    {
    }
    public interface IOrderedQuery<T, TJoin> : ISelectOnQuery<T, TJoin>
    {
    }
    public interface IOrderedQuery<T, TJoin, TJoin2> : ISelectOnQuery<T, TJoin, TJoin2>
    {
    }
    public interface IOrderedQuery<T, TJoin, TJoin2, TJoin3> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3>
    {
    }
    public interface IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
    }
    public interface IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
    }
    public interface IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> : ISelectOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
    }
}
