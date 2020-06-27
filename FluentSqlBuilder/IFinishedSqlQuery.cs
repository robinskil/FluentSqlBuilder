using System.Collections.Generic;
using System.Data.SqlClient;

namespace FluentSqlBuilder
{
    public interface IFinishedSqlQuery
    {
        string GetSqlStatement(); 
        SqlCommand ToSqlCommand();
        IReadOnlyDictionary<string, object> GetParameters();
    }
}
