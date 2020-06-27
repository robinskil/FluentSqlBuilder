using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FluentSqlBuilder.ExpressionResolvers
{
    public class ExpressionTreeGroupByResolver
    {
        private readonly Dictionary<string, string> typeAs;
        private readonly Dictionary<string, string> variableTypeName;

        public ExpressionTreeGroupByResolver(Dictionary<string, string> typeAs)
        {
            this.typeAs = typeAs;
            this.variableTypeName = new Dictionary<string, string>();
        }

        public string ResolveGroupByLambda(LambdaExpression lambdaExpression)
        {
            string result = "";
            if (typeof(NewExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                var selectedProperties = (lambdaExpression.Body as NewExpression).Arguments;
                if (selectedProperties.Count > 0)
                {
                    result += " GROUP BY";
                    for (int i = 0; i < selectedProperties.Count; i++)
                    {
                        var memberExpr = selectedProperties[i] as MemberExpression;
                        var typeExpr = memberExpr.Expression as ParameterExpression;
                        if (i < selectedProperties.Count - 1)
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}],";
                        }
                        else
                        {
                            result += $" [{variableTypeName[typeExpr.Name]}].[{memberExpr.Member.Name}]";
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
    }
}
