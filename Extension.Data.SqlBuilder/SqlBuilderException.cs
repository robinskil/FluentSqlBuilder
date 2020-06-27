using System;
using System.Runtime.Serialization;

namespace Extension.Data.SqlBuilder
{
    [Serializable]
    internal class SqlBuilderException : Exception
    {
        public SqlBuilderException()
        {
        }

        public SqlBuilderException(string message) : base(message)
        {
        }

        public SqlBuilderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}