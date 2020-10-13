using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Fluent.SqlQuery.SqlExtensions;

namespace Fluent.SqlQuery.ExpressionResolvers
{
    public class ExpressionResolver
    {
        private readonly IDictionary<string, object> _parameters;
        protected readonly IList<(string variableName,Type objectType)> TypeMapTargets;
        protected ExpressionResolver(IList<(string variableName, Type objectType)> typeMapTargets, IDictionary<string, object> parameters)
        {
            this.TypeMapTargets = typeMapTargets;
            this._parameters = parameters;
        }

        protected string ResolveExpression(Expression expression, IReadOnlyDictionary<string,string> queryVariableNames)
        {
            return expression switch
            {
                ConstantExpression constantExpression => ResolvedConstantExpressionToParameterizedString(new []{constantExpression.Value}),
                MethodCallExpression methodCallExpression => IsInternalSqlTranslationMethod(methodCallExpression) ? ResolveInternalSqlTranslationMethod(methodCallExpression,queryVariableNames) : AddParameter(ResolveMethodCallExpression(methodCallExpression)),
                MemberExpression expr => ResolveMemberExpression(expr, queryVariableNames,true, PushWithReturnReference(new Stack<MemberExpression>(),expr)),
                _ => throw new Exception("Cannot handle this expression.")
            };
        }

        protected Stack<MemberExpression> PushWithReturnReference(Stack<MemberExpression> memberExpressionStack,
            MemberExpression memberExpression)
        {
            memberExpressionStack.Push(memberExpression);
            return memberExpressionStack;
        }

        protected virtual string ResolveMemberExpression(MemberExpression memberExpression, IReadOnlyDictionary<string, string> queryVariableNames, bool decodeToConstant , Stack<MemberExpression> memberAccessStack)
        {
            return memberExpression.Expression switch
            {
                ConstantExpression constantExpression => ResolvedConstantExpressionToParameterizedString(ResolveConstantExpression(memberAccessStack,constantExpression)),
                ParameterExpression parameterExpression =>
                    $"[{queryVariableNames[parameterExpression.Name]}].[{memberExpression.Member.Name}]",
                MemberExpression expr => ResolveMemberExpression(expr, queryVariableNames, decodeToConstant,PushWithReturnReference(memberAccessStack,expr)),
                _ => null
            };
        }

        private bool IsInternalSqlTranslationMethod(MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Method.GetCustomAttribute(typeof(Translate)) != null;
            //Check if this method contains a sql translation attribute
        }

        protected string ResolvedConstantExpressionToParameterizedString(IList<object> parameters)
        {
            if (parameters.Count > 1)
            {
                var builder = new StringBuilder();
                builder.Append("(");
                builder.Append(string.Join(",", parameters.Select(AddParameter)));
                builder.Append(")");
                return builder.ToString();
            }
            return AddParameter(parameters[0]);
        }

        private string ResolveInternalSqlTranslationMethod(MethodCallExpression methodCallExpression, IReadOnlyDictionary<string, string> queryVariableNames)
        {
            var attribute = (Translate)methodCallExpression.Method.GetCustomAttribute(typeof(Translate));
            string translated = attribute.Translation;
            var self = methodCallExpression.Arguments[0];
            var memberName = ResolveMemberExpression(self as MemberExpression, queryVariableNames, false, PushWithReturnReference(new Stack<MemberExpression>(), self as MemberExpression));
            for (int i = 1; i < methodCallExpression.Arguments.Count; i++)
            {
                var arg = methodCallExpression.Arguments[i];
                var queryArguments = ResolveArgumentFromExpression(arg);
                if (queryArguments.Count > 1)
                {
                    var builder = new StringBuilder();
                    builder.Append("(");
                    builder.Append(string.Join(",", queryArguments.Select(AddParameter)));
                    builder.Append(")");
                    translated = translated.Replace($"%arg{i - 1}",builder.ToString());
                }
                else
                {
                    translated = translated.Replace($"%arg{i - 1}",AddParameter(queryArguments[0]));
                }
            }
            return translated.Replace("%var",memberName);
        }


        private object ResolveMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.Count > 0)
            {
                if (methodCallExpression.Object != null)
                    return methodCallExpression.Object switch
                    {
                        ConstantExpression constantExpression => methodCallExpression.Method.Invoke(
                            GetObjectFromConstantExpression(constantExpression),
                            methodCallExpression.Arguments.SelectMany(ResolveArgumentFromExpression).ToArray()),
                        _ => throw new NotImplementedException()
                    };
                else
                   return methodCallExpression.Method.Invoke(GetObjectFromConstantExpression(null),
                        methodCallExpression.Arguments.SelectMany(ResolveArgumentFromExpression).ToArray());
            }
            return methodCallExpression.Method.Invoke(GetObjectFromConstantExpression(methodCallExpression.Object as ConstantExpression), null);
        }

        private IList<object> ResolveArgumentFromExpression(Expression expression)
        {
            return expression switch
            {
                MethodCallExpression methodCallExpression => new []{ResolveMethodCallExpression(methodCallExpression)},
                ConstantExpression constantExpression => new []{constantExpression.Value},
                MemberExpression memberExpression => ResolveMemberExpressionToValue(memberExpression, PushWithReturnReference(new Stack<MemberExpression>(), memberExpression)),
                NewArrayExpression newArrayExpression => newArrayExpression.Expressions.SelectMany(ResolveArgumentFromExpression).ToArray(),
                _ => throw new Exception("Null argument")
            };
        }

        private IList<object> ResolveMemberExpressionToValue(MemberExpression memberExpression, Stack<MemberExpression> memberAccessStack)
        {
            return memberExpression.Expression switch
            {
                ConstantExpression constantExpression => ResolveConstantExpression(memberAccessStack, constantExpression),
                MemberExpression expr => ResolveMemberExpressionToValue(expr, PushWithReturnReference(memberAccessStack, expr)),
                _ => null
            };
        }

        protected IList<object> ResolveConstantExpression(Stack<MemberExpression> memberExpressionStack,
            ConstantExpression constantExpression)
        {
            object result = constantExpression.Value;
            while (memberExpressionStack != null && memberExpressionStack.Count > 0)
            {
                var currentExpr = memberExpressionStack.Pop();
                var field = result.GetType().GetField(currentExpr.Member.Name);
                if (field != null)
                {
                    result = field.GetValue(result);
                }
                else
                {
                    var prop = result.GetType().GetProperty(currentExpr.Member.Name);
                    result = prop.GetValue(result);
                }
            }
            if (result is IEnumerable enumerable)
            {
                return enumerable.Cast<object>().ToList();
            }
            return new []{result};
        }

        /// <summary>
        /// Returns the constant value from a constant expression.
        /// </summary>
        /// <param name="constantExpression"></param>
        /// <returns>Returns a value in boxed object form. This can be null.</returns>
        private object GetObjectFromConstantExpression(ConstantExpression constantExpression)
        {
            return constantExpression?.Value;
        }

        protected string AddParameter(object value)
        {
            string paramName = $"@param{_parameters.Count}";
            _parameters.Add(paramName,value);
            return paramName;
        }
    }
}