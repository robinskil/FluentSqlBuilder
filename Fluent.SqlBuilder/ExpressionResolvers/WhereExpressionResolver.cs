using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Fluent.SqlQuery.ExpressionResolvers
{
    public class WhereExpressionResolver : ExpressionResolver
    {
        private readonly Dictionary<string, string> _variableNames;
        public WhereExpressionResolver(IList<(string variableName, Type objectType)> typeMapTargets, IDictionary<string, object> parameters) : base(typeMapTargets, parameters)
        {
            _variableNames = new Dictionary<string, string>();
        }

        public string Where(LambdaExpression lambdaExpression)
        {
            var param = lambdaExpression.Parameters;
            for (int i = 0; i < param.Count; i++)
            {
                _variableNames.Add(param[i].Name, TypeMapTargets[i].variableName);
            }
            return ResolveRecursiveBoolExpression(lambdaExpression.Body);
        }

        private string ResolveRecursiveBoolExpression(Expression expression)
        {
            var builder = new StringBuilder();
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                {
                    var left = binaryExpression.Left;
                    var right = binaryExpression.Right;
                    if (left is BinaryExpression leftBinary)
                    {
                        builder.Append(ResolveRecursiveBoolExpression(leftBinary));
                    }
                    else
                    {
                        builder.Append(ResolveExpression(left, _variableNames));
                    }
                    builder.Append($" {GetComparerStringFromBinaryExpression(binaryExpression)} ");
                    if (right is BinaryExpression rightBinary)
                    {
                        builder.Append(ResolveRecursiveBoolExpression(rightBinary));
                    }
                    else
                    {
                        builder.Append(ResolveExpression(right, _variableNames));
                    }
                    break;
                }
                case MethodCallExpression methodCallExpression:
                    builder.Append($"{ResolveExpression(methodCallExpression, _variableNames)}");
                    break;
                default: throw new Exception("Couldn't create where clause.");
            }
            return builder.ToString();
        }

        private string GetOperandFromUnary(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.NodeType)
            {
                case ExpressionType.Not:
                    return "NOT";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetComparerStringFromBinaryExpression(BinaryExpression baseExpression)
        {
            return baseExpression.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.LessThan => "<",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException("This expression type is not supported"),
            };
        }
    }
}
