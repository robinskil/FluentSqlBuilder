using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Extension.Data.SqlBuilder.ExpressionResolvers
{
    public class ExpresionTreeSelectResolver
    {
        private readonly Dictionary<string, string> typeAs;
        private readonly Dictionary<string, string> variableTypeName;

        public ExpresionTreeSelectResolver(Dictionary<string, string> typeAs)
        {
            this.typeAs = typeAs;
            this.variableTypeName = new Dictionary<string, string>();
        }

        public string ResolveSelectLambda(LambdaExpression lambdaExpression)
        {
            if (typeof(NewExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                string result = "";
                var selectedProperties = (lambdaExpression.Body as NewExpression).Arguments;
                if (selectedProperties.Count > 0)
                {
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
                return result;
            }
            else
            {
                throw new SqlBuilderException("Select expressions are designed to only handle anonymous objects.");
            }
        }

    }
}
