using System.Data.SqlClient;

namespace FluentSqlBuilder
{
    public static class SqlBuilderEntry
    {
        public static IBuilder Build(this SqlConnection sqlConnection)
        {
            return new SqlBuilder(sqlConnection);
        }
    }
}
