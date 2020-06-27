using System.Data.SqlClient;

namespace Extension.Data.SqlBuilder
{
    public static class SqlBuilderEntry
    {
        public static IBuilder Build(this SqlConnection sqlConnection)
        {
            return new SqlBuilder(sqlConnection);
        }
    }
}
