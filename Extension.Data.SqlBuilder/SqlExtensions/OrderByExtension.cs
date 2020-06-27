using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Extension.Data.SqlBuilder.SqlExtensions
{
    public static class OrderByExtension
    {
        public static object Ascending<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
        public static object Descending<T>(this T obj)
        {
            throw new NotSupportedException("This method shouldn't be invoked");
        }
    }
}
