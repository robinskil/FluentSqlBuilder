using Extension.Data.SqlBuilder.ExpressionResolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Extension.Data.SqlBuilder
{
    public interface IBuilder
    {
        IBaseQuery<T> From<T>();
    }

    public class SqlBuilder : IBuilder, IFinishedSqlQuery
    {
        protected readonly SqlConnection sqlConnection;
        protected readonly ExpressionTreeWhereResolver expressionTreeWhereResolver;
        protected readonly ExpressionTreeJoinResolver expressionTreeJoinResolver;
        protected readonly ExpressionTreeGroupByResolver expressionTreeGroupByResolver;
        protected readonly ExpressionTreeOrderByResolver expressionTreeOrderByResolver;
        protected readonly ExpresionTreeSelectResolver expresionTreeSelectResolver;

        protected readonly Dictionary<string, string> typeAsReplacement;

        #region Query props
        protected Dictionary<string, object> parameters;

        public string FromClause { get; private set; }

        protected string SelectClause { get; private set; }
        protected string WhereClause { get; private set; }
        protected string JoinClause { get; private set; }
        protected string GroupByClause { get; private set; }
        protected string OrderByClause { get; private set; }
        #endregion

        public SqlBuilder(SqlConnection sqlConnection)
        {
            parameters = new Dictionary<string, object>();
            typeAsReplacement = new Dictionary<string, string>();
            this.expressionTreeJoinResolver = new ExpressionTreeJoinResolver(typeAsReplacement);
            this.expressionTreeWhereResolver = new ExpressionTreeWhereResolver(parameters,typeAsReplacement);
            this.expressionTreeGroupByResolver = new ExpressionTreeGroupByResolver(typeAsReplacement);
            this.expressionTreeOrderByResolver = new ExpressionTreeOrderByResolver(typeAsReplacement);
            this.expresionTreeSelectResolver = new ExpresionTreeSelectResolver(typeAsReplacement);
            this.sqlConnection = sqlConnection;
        }
        protected SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs)
        {
            this.parameters = parameters;
            this.FromClause = fromClause;
            this.JoinClause = joinClause;
            this.typeAsReplacement = typeAs;
            this.expressionTreeJoinResolver = new ExpressionTreeJoinResolver(typeAsReplacement);
            this.expressionTreeWhereResolver = new ExpressionTreeWhereResolver(parameters, typeAsReplacement);
            this.expressionTreeGroupByResolver = new ExpressionTreeGroupByResolver(typeAsReplacement);
            this.expressionTreeOrderByResolver = new ExpressionTreeOrderByResolver(typeAsReplacement);
            this.expresionTreeSelectResolver = new ExpresionTreeSelectResolver(typeAsReplacement);
            this.sqlConnection = sqlConnection;
        }

        public virtual SqlCommand ToSqlCommand()
        {
            var command = sqlConnection.CreateCommand();
            command.CommandText = GetSqlStatement();
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
            return command;
        }
        public IReadOnlyDictionary<string, object> GetParameters()
        {
            return parameters;
        }
        /// <summary>
        /// Builds a string representation of the configured SQL statement.
        /// </summary>
        /// <returns></returns>
        public string GetSqlStatement()
        {
            var builder = new StringBuilder();
            if (string.IsNullOrEmpty(SelectClause))
            {
                throw new SqlBuilderException("Select clause is needed. Use the Select function to specify the returned columns.");
            }
            else
            {
                builder.Append(SelectClause);
                builder.Append(FromClause);
            }
            if (!string.IsNullOrEmpty(JoinClause))
            {
                builder.Append(JoinClause);
            }
            if (!string.IsNullOrEmpty(WhereClause))
            {
                builder.Append(WhereClause);
            }
            if (!string.IsNullOrEmpty(GroupByClause))
            {
                builder.Append(GroupByClause);
            }
            if (!string.IsNullOrEmpty(OrderByClause))
            {
                builder.Append(OrderByClause);
            }
            return builder.ToString();
        }
        public IBaseQuery<T> From<T>()
        {
            return new SqlBuilder<T>(sqlConnection);
        }

        protected void SetFromClause(string fromClause)
        {
            this.FromClause = fromClause;
        }

        protected void HandleSelectExpresion(LambdaExpression lambdaExpression, string selectBase)
        {
            SelectClause = selectBase + expresionTreeSelectResolver.ResolveSelectLambda(lambdaExpression);
        }
        protected void HandleWhereExpression(LambdaExpression lambdaExpression)
        {
            WhereClause = " WHERE " + expressionTreeWhereResolver.ResolveLambdaExpressionToWhereClause(lambdaExpression);
        }
        protected void HandleJoinExpression(LambdaExpression lambdaExpression, Type joinType)
        {
            JoinClause += expressionTreeJoinResolver.ResolveBinaryJoinExpression(lambdaExpression,joinType);
        }
        protected void HandleGroupByExpression(LambdaExpression lambdaExpression)
        {
            GroupByClause = expressionTreeGroupByResolver.ResolveGroupByLambda(lambdaExpression);
        }
        protected void HandleOrderByExpression(LambdaExpression lambdaExpression)
        {
            OrderByClause = expressionTreeOrderByResolver.ResolveOrderByLambda(lambdaExpression);
        }

        protected void AddTypeName(string key,Type t)
        {
            var name = t.Name;
            foreach (var typeVal in typeAsReplacement)
            {
                if(typeVal.Value == name)
                {
                    var lastChar = name.Last();
                    if(int.TryParse(lastChar.ToString(),out var number))
                    {
                        name = name.Remove(name.Length - 1) + (number + 1);
                    }
                    else
                    {
                        name = name + 1;
                    }
                }
            }
            typeAsReplacement.Add(key, name);
        }
    }

    public class SqlBuilder<T> : SqlBuilder, IBaseQuery<T> , IConditionalQuery<T>, IGroupedQuery<T> , IOrderedQuery<T>
    {
        public SqlBuilder(SqlConnection sqlConnection) : base(sqlConnection)
        {
            typeAsReplacement.Add("T", $"{typeof(T).Name}");
            SetFromClause($" FROM [{typeof(T).Name}]");
        }

        protected SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, Dictionary<string,string> typeAs, string fromClause, string joinClause) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }
        public IFinishedSqlQuery SelectTop(Expression<Func<T, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }
        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }
        public IJoinedOnQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> joinOn)
        {
            AddTypeName("TJoin",typeof(TJoin));
            HandleJoinExpression(joinOn, typeof(TJoin));
            //var joinOnString = $" JOIN [{typeof(TJoin).Name}] ON " + expressionTreeJoinResolver.ResolveBinaryJoinExpression(joinOn.Body as BinaryExpression);
            //return new SqlBuilder<T,TJoin>(sqlConnection,whereClause,selectClause,fromClause,joinOnString,parameters);
            return new SqlBuilder<T, TJoin>(this.sqlConnection, this.parameters,this.typeAsReplacement, FromClause, JoinClause);
        }
        public IConditionalQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IGroupedQuery<T> GroupBy(Expression<Func<T, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T> OrderBy(Expression<Func<T, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin> : SqlBuilder, IJoinedOnQuery<T,TJoin> , IConditionalQuery<T,TJoin> , IOrderedQuery<T,TJoin>, IGroupedQuery<T,TJoin>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, Dictionary<string, string> typeAs, string fromClause, string joinClause) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IJoinedOnQuery<T, TJoin, TJoin2> Join<TJoin2>(Expression<Func<T, TJoin, TJoin2, bool>> joinOn)
        {
            AddTypeName("TJoin2", typeof(TJoin2));
            HandleJoinExpression(joinOn, typeof(TJoin2));
            return new SqlBuilder<T, TJoin, TJoin2>(this.sqlConnection, this.parameters, FromClause, JoinClause, this.typeAsReplacement);
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin> Where(Expression<Func<T, TJoin, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IGroupedQuery<T, TJoin> GroupBy(Expression<Func<T, TJoin, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin> OrderBy(Expression<Func<T, TJoin, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin, TJoin2> : SqlBuilder, IJoinedOnQuery<T, TJoin, TJoin2>, IConditionalQuery<T, TJoin, TJoin2>, IGroupedQuery<T, TJoin, TJoin2>, IOrderedQuery<T, TJoin, TJoin2>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin, TJoin2> Where(Expression<Func<T, TJoin, TJoin2, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IJoinedOnQuery<T, TJoin, TJoin2, TJoin3> Join<TJoin3>(Expression<Func<T, TJoin, TJoin2, TJoin3, bool>> joinOn)
        {
            AddTypeName("TJoin3", typeof(TJoin3));
            HandleJoinExpression(joinOn, typeof(TJoin3));
            return new SqlBuilder<T, TJoin, TJoin2, TJoin3>(this.sqlConnection, this.parameters, FromClause, JoinClause, this.typeAsReplacement);
        }

        public IGroupedQuery<T, TJoin, TJoin2> GroupBy(Expression<Func<T, TJoin, TJoin2, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin, TJoin2> OrderBy(Expression<Func<T, TJoin, TJoin2, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin, TJoin2, TJoin3> : SqlBuilder, IJoinedOnQuery<T, TJoin, TJoin2, TJoin3>, IConditionalQuery<T, TJoin, TJoin2, TJoin3>, IGroupedQuery<T, TJoin, TJoin2, TJoin3>, IOrderedQuery<T, TJoin, TJoin2, TJoin3>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin, TJoin2, TJoin3> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4> Join<TJoin4>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, bool>> joinOn)
        {
            AddTypeName("TJoin4", typeof(TJoin4));
            HandleJoinExpression(joinOn, typeof(TJoin4));
            return new SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4>(this.sqlConnection, this.parameters, FromClause, JoinClause, this.typeAsReplacement);
        }

        public IGroupedQuery<T, TJoin, TJoin2, TJoin3> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin, TJoin2, TJoin3> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4> : SqlBuilder, IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4>, IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4>, IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4>, IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> Join<TJoin5>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, bool>> joinOn)
        {
            AddTypeName("TJoin5", typeof(TJoin5));
            HandleJoinExpression(joinOn, typeof(TJoin5));
            return new SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>(this.sqlConnection, this.parameters, FromClause, JoinClause, this.typeAsReplacement);
        }

        public IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> : SqlBuilder, IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>, IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>, IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>, IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> Join<TJoin6>(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, bool>> joinOn)
        {
            AddTypeName("TJoin6", typeof(TJoin6));
            HandleJoinExpression(joinOn, typeof(TJoin6));
            return new SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>(this.sqlConnection, this.parameters, FromClause, JoinClause, this.typeAsReplacement);
        }

        public IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }

    public class SqlBuilder<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> : SqlBuilder, IJoinedOnQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>, IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>, IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>, IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6>
    {
        public SqlBuilder(SqlConnection sqlConnection, Dictionary<string, object> parameters, string fromClause, string joinClause, Dictionary<string, string> typeAs) : base(sqlConnection, parameters, fromClause, joinClause,typeAs)
        {
        }

        public IFinishedSqlQuery Select(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select)
        {
            HandleSelectExpresion(select, "SELECT");
            return this;
        }

        public IConditionalQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> Where(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, bool>> expression)
        {
            HandleWhereExpression(expression);
            return this;
        }

        public IFinishedSqlQuery SelectTop(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select, int topAmount)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({topAmount})");
            return this;
        }

        public IFinishedSqlQuery SelectTopPercent(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> select, int percentage)
        {
            HandleSelectExpresion(select, $"SELECT TOP ({percentage}) PERCENT");
            return this;
        }

        public IGroupedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> GroupBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> groupBy)
        {
            HandleGroupByExpression(groupBy);
            return this;
        }

        public IOrderedQuery<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6> OrderBy(Expression<Func<T, TJoin, TJoin2, TJoin3, TJoin4, TJoin5, TJoin6, object>> orderBy)
        {
            HandleOrderByExpression(orderBy);
            return this;
        }
    }
}
