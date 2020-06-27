using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentSqlBuilder.ExpressionResolvers
{
    public class ExpressionTreeOrderByResolver
    {
        private readonly Dictionary<string, string> typeAs;
        private readonly Dictionary<string, string> variableTypeName;

        public ExpressionTreeOrderByResolver(Dictionary<string, string> typeAs)
        {
            this.typeAs = typeAs;
            this.variableTypeName = new Dictionary<string, string>();
        }

        public string ResolveOrderByLambda(LambdaExpression lambdaExpression)
        {
            string result = "";
            if (typeof(NewExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                result += " ORDER BY";
                var expr = lambdaExpression.Body as NewExpression;
                var args = expr.Arguments;
                for (int i = 0; i < args.Count; i++)
                {
                    if (typeof(MemberExpression).IsAssignableFrom(args[i].GetType()))
                    {
                        var memberExpr = args[i] as MemberExpression;
                        var typeExpr = memberExpr.Expression as ParameterExpression;
                        if (i < args.Count - 1)
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}],";
                        }
                        else
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}]";
                        }
                    }
                    else if (typeof(MethodCallExpression).IsAssignableFrom(args[i].GetType()))
                    {
                        var methodExpr = args[i] as MethodCallExpression;
                        var memberExpr = methodExpr.Arguments[0] as MemberExpression;
                        var typeExpr = memberExpr.Expression as ParameterExpression;
                        if (i < args.Count - 1)
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}] {MethodNameToOrderByType(methodExpr.Method)},";
                        }
                        else
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}] {MethodNameToOrderByType(methodExpr.Method)}";
                        }
                    }
                }
            }
            else
            {
                throw new Exception();
            }
            return result;
        }

        private string MethodNameToOrderByType(MethodInfo methodInfo)
        {
            if(methodInfo.Name == "Descending")
            {
                return "DESC";
            }
            else if(methodInfo.Name == "Ascending")
            {
                return "ASC";
            }
            throw new Exception();
        }
    }
}
