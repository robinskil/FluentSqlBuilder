using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Extension.Data.SqlBuilder.ExpressionResolvers
{
    public abstract class ExpressionTreeResolver
    {
        protected readonly IDictionary<string, object> parameters;
        protected readonly IDictionary<string, string> typeAs;
        public ExpressionTreeResolver(IDictionary<string, object> parameters, IDictionary<string, string> typeAs)
        {
            this.parameters = parameters;
            this.typeAs = typeAs;
        }

        /// <summary>
        /// Returns constant value from a member access method.
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        protected object ResolveMemberExpressionValue(MemberExpression memberExpression)
        {
            if (typeof(MemberExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                return ResolveMemberExpressionValue(memberExpression.Expression as MemberExpression);
            }
            else if (typeof(ConstantExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                return (memberExpression.Expression as ConstantExpression).Value;
            }
            throw new SqlBuilderException("Couldn't resolve this expression");
        }
        /// <summary>
        /// Returns a constant expression to resolve a value
        /// Or returns a parameters expression to resolve the type access
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        protected MemberExpression GetBaseMemberExpressionFromParentMember(MemberExpression memberExpression)
        {
            if (typeof(MemberExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                return GetBaseMemberExpressionFromParentMember(memberExpression.Expression as MemberExpression);
            }
            else if (typeof(ConstantExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                return memberExpression;
            }
            else if (typeof(ParameterExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                return memberExpression;
            }
            throw new SqlBuilderException("Couldn't resolve this expression");
        }

        protected string ResolveMemberExpressionParameterName(MemberExpression memberExpression, IDictionary<string, string> variableTypeName)
        {
            var memberName = memberExpression.Member.Name;
            var typeExpr = memberExpression.Expression as ParameterExpression;
            return $"[{variableTypeName[typeExpr.Name]}].[{memberName}]";
        }

        protected string CreateParameterForValue(object parameterVal)
        {
            var paramCount = parameters.Count;
            var param = new KeyValuePair<string, object>($"@P{paramCount + 1}", parameterVal);
            parameters.Add(param.Key, param.Value);
            return param.Key;
        }
    }
}
