using System;
using System.Collections.Generic;
using System.Text;

namespace FluentSqlBuilder
{
    public static class SelectAndHavingExtensions
    {
        public static SelectHavingResult SqlSum<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static SelectHavingResult SqlCount<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static SelectHavingResult SqlAvg<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static SelectHavingResult SqlMin<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static SelectHavingResult SqlMax<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static SelectHavingResult SqlDistinct<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
    }

    public class SelectHavingResult
    {
        public object Result { get; set; }
    }
}
