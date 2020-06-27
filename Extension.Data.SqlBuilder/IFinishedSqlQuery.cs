using System.Collections.Generic;
using System.Data.SqlClient;

namespace Extension.Data.SqlBuilder
{
    public interface IFinishedSqlQuery
    {
        string GetSqlStatement(); 
        SqlCommand ToSqlCommand();
        IReadOnlyDictionary<string, object> GetParameters();
    }
}
