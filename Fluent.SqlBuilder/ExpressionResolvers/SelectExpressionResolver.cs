using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Fluent.SqlQuery.ExpressionResolvers
{
    public class SelectExpressionResolver : ExpressionResolver
    {

        public SelectExpressionResolver(IList<(string,Type)> typeMapTargets, IDictionary<string, object> parameters) : base(typeMapTargets, parameters)
        {
            
        }
        public string Select(LambdaExpression lambdaExpression)
        {
            var variableNames = new Dictionary<string,string>();
            if (lambdaExpression.Body is NewExpression expression)
            {
                var param = lambdaExpression.Parameters;
                for (var i = 0; i < param.Count; i++)
                {
                    variableNames.Add(param[i].Name, TypeMapTargets[i].variableName);
                }
                var result = "";
                var selectedProperties = expression.Arguments;
                if (selectedProperties.Count > 0)
                {
                    for (int i = 0; i < selectedProperties.Count; i++)
                    {
                        if (selectedProperties[i] is MemberExpression memberExpr)
                        {
                            if (i == 0)
                            {
                                result += ResolveExpression(memberExpr,variableNames);
                            }
                            else
                            {
                                result += $" , {ResolveExpression(memberExpr,variableNames)}";
                            }
                        }
                        //If a complete object is passed, then we map all the properties of it.
                        else if (selectedProperties[i] is ParameterExpression parameterExpression)
                        {
                            int counter = 0;
                            foreach (var property in parameterExpression.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (counter == 0)
                                {
                                    result += $"[{variableNames[parameterExpression.Name]}].[{property.Name}]";
                                }
                                else
                                {
                                    result += $" , [{variableNames[parameterExpression.Name]}].[{property.Name}]";
                                }
                                counter++;
                            }
                        }
                    }
                }
                else throw new Exception("Empty select statement.");
                return result;
            }
            else
            {
                throw new Exception("Select expressions are designed to only handle anonymous objects.");
            }
        }

        protected override string ResolveMemberExpression(MemberExpression memberExpression, IReadOnlyDictionary<string, string> queryVariableNames, bool decodeToConstant, Stack<MemberExpression> memberAccessStack)
        {
            return memberExpression.Expression switch
            {
                ConstantExpression constantExpression => ResolvedConstantExpressionToParameterizedString(ResolveConstantExpression(memberAccessStack, constantExpression)),
                ParameterExpression parameterExpression =>
                    $"[{queryVariableNames[parameterExpression.Name]}].[{memberExpression.Member.Name}]",
                MemberExpression expr => ResolveMemberExpression(expr, queryVariableNames, decodeToConstant, PushWithReturnReference(memberAccessStack, expr)),
                _ => throw new ArgumentNullException(nameof(memberExpression.Expression), "Failed to resolve member expression")
            };
        }

        private void IncludeMapFor(MemberExpression memberExpression)
        {
            memberExpression.Member.
        }
    }
}
