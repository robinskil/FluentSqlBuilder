using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using Fluent.SqlQuery.ExpressionResolvers;

namespace Fluent.SqlQuery.SqlQueryEngine
{
    public abstract class BaseQueryEngine
    {
        protected Func<string> SelectClause;
        protected Func<string> TopClause;
        protected Func<string> FromClause;
        protected Func<string> WhereClause;
        protected Queue<Func<string>> Joins;
        protected readonly IList<(string variableNameInQuery,Type objectType)> TypeMapTargets;
        protected readonly IDictionary<string, object> SqlParameters;

        //Expression Resolvers
        protected SelectExpressionResolver SelectExpressionResolver;
        protected WhereExpressionResolver WhereExpressionResolver;

        //End

        protected BaseQueryEngine(Type baseType)
        {
            TypeMapTargets = new List<(string, Type)>();
            SqlParameters = new Dictionary<string, object>();
            SelectExpressionResolver = new SelectExpressionResolver(TypeMapTargets, SqlParameters);
            WhereExpressionResolver = new WhereExpressionResolver(TypeMapTargets,SqlParameters);
            AddType(baseType);
            AddToFromCallback(baseType);
        }


        /// <summary>
        /// Adds a new type variable name to the query builder
        /// </summary>
        /// <param name="t">Type of object that has been added to the query</param>
        protected void AddType(Type t)
        {
            TypeMapTargets.Add((t.Name,t));
        }

        protected void AddToFromCallback(Type type)
        {
            FromClause = () => "["+TypeMapTargets[0].variableNameInQuery+"]";
        }

        protected void AddSelectClause(LambdaExpression lambdaExpression)
        {
            SelectClause = () =>  SelectExpressionResolver.Select(lambdaExpression);
        }

        protected void AddTopClause(int amount)
        {
            TopClause = () => $"TOP {amount}";
        }

        protected void AddTopPercentClause(double percentage)
        {
            TopClause = () => $"TOP {percentage} PERCENT";
        }

        protected void AddWhereClause(LambdaExpression lambdaExpression)
        {
            WhereClause = () => WhereExpressionResolver.Where(lambdaExpression);
        }

        internal string ToSqlQuery()
        {
            var builder = new StringBuilder();
            builder.Append($"SELECT ");
            if (TopClause != null)
            {
                builder.Append($"{TopClause()} ");
            }
            builder.Append($"{SelectClause()} \n FROM {FromClause()}");
            if (WhereClause != null) builder.Append($" \n WHERE {WhereClause()}");
            return builder.ToString();
        }

        internal IEnumerable<IDbDataParameter> GetParameters()
        {
            foreach (var parameter in SqlParameters)
            {
                yield return ParametersCreation(parameter.Key, parameter.Value);
            }
        }

        protected abstract IDbDataParameter ParametersCreation(string parameterName,object parameterValue);
    }

    public class BaseQueryEngine<T> : BaseQueryEngine
    {
        public BaseQueryEngine() : base(typeof(T))
        {
        }

        public void Select(Expression<Func<T, object>> select)
        {
            AddSelectClause(select);
        }

        public void Where(Expression<Func<T, bool>> where)
        {
            AddWhereClause(where);
        }

        public void Top(int amount)
        {
            AddTopClause(amount);
        }

        public void TopPercent(double percentage)
        {
            AddTopPercentClause(percentage);
        }

        protected override IDbDataParameter ParametersCreation(string parameterName, object parameterValue)
        {
            return new SqlParameter()
            {
                ParameterName = parameterName,
                Value = parameterValue
            };
        }
    }
}
