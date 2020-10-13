using System;
using System.Collections.Generic;
using System.Text;

namespace Fluent.SqlQuery.SqlExtensions
{
    public static class Extensions
    {
        /// <summary>
        /// Translates Count(*) of a table result
        /// </summary>
        /// <returns></returns>
        public static int Count()
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates Count of a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Count<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates Count of a variable.
        /// </summary>
        /// <returns></returns>
        public static int Count(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Avg<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Avg(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Sum<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Sum(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Max<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Max(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Min<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Min(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates Count of a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T Distinct<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <returns></returns>
        public static string Distinct(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        [Translate("%var IS NULL")]
        public static bool IsNull<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NULL check on a string variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [Translate("%var IS NULL")]
        public static bool IsNull(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        /// <summary>
        /// Translates a NOT NULL check on a variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        [Translate("%var IS NOT NULL")]
        public static bool IsNotNull<T>(this T self) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates IS NOT NULL check on a string variable.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [Translate("%var IS NOT NULL")]
        public static bool IsNotNull(this string self)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var LIKE %arg0")]
        public static bool Like<T>(this T self, string arg) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT LIKE %arg0")]
        public static bool NotLike<T>(this T self, string arg) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var LIKE %arg0")]
        public static bool Like(this string self, string arg)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT LIKE %arg0")]
        public static bool NotLike(this string self, string arg)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        /// <summary>
        /// Translates a BETWEEN check on a variable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        [Translate("%var BETWEEN %arg0 AND %arg1")]
        public static bool Between<T>(this T self, T arg0 , T arg1) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT BETWEEN %arg0 AND %arg1")]
        public static bool NotBetween<T>(this T self, T arg0, T arg1) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates a BETWEEN check on a string variable
        /// </summary>
        /// <param name="self"></param>
        /// <param name="arg0"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        [Translate("%var BETWEEN %arg0 AND %arg1")]
        public static bool Between(this string self, string arg0 , string arg2)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT BETWEEN %arg0 AND %arg1")]
        public static bool NotBetween(this string self, string arg0, string arg2)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        /// <summary>
        /// Translates an IN check on a variable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        [Translate("%var IN %arg0")]
        public static bool In<T>(this T self, T[] arg0) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT IN %arg0")]
        public static bool NotIn<T>(this T self, T[] arg0) where T : struct
        {
            throw new Exception("This method shouldn't be invoked.");
        }
        /// <summary>
        /// Translates an IN check on a string variable
        /// </summary>
        /// <param name="self"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        [Translate("%var IN %arg0")]
        public static bool In(this string self, string[] arg0)
        {
            throw new Exception("This method shouldn't be invoked.");
        }

        [Translate("%var NOT IN %arg0")]
        public static bool NotIn(this string self, string[] arg0)
        {
            throw new Exception("This method shouldn't be invoked.");
        }
    }
}
