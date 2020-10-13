using System;
using System.Collections.Generic;

namespace Fluent.SqlBuilder.SqlQueryEngine
{
    public class TypeMapTarget : IEqualityComparer<TypeMapTarget>
    {
        /// <summary>
        /// The variable name within the query.
        /// </summary>
        public string QueryVariable { get; set; }
        /// <summary>
        /// The name of the generic parameter like T , TJoin , TJoin2 etc.
        /// </summary>
        public string RepresentsGeneric { get; set; }
        public Type Type { get; set; }

        public bool Equals(TypeMapTarget x, TypeMapTarget y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.RepresentsGeneric == y.RepresentsGeneric;
        }

        public int GetHashCode(TypeMapTarget obj)
        {
            return (obj.RepresentsGeneric != null ? obj.RepresentsGeneric.GetHashCode() : 0);
        }
    }
}