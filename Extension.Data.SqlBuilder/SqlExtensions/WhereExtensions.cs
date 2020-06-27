using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.Data.SqlBuilder
{
    public static class WhereExtensions
    {
        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static bool NotIn<T>(this T obj, IEnumerable<T> collection)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static bool Like(this string obj, string pattern)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static bool NotLike(this string obj, string pattern)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static bool Between<T>(this T obj, T lowerBound, T upperBound)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static bool NotBetween<T>(this T obj, T lowerBound, T upperBound)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
    }
}
