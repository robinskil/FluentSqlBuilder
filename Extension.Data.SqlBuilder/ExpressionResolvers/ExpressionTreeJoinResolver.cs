using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
namespace Extension.Data.SqlBuilder
{
    public class ExpressionTreeJoinResolver
    {
        private readonly Dictionary<string, string> typeAs;
        private readonly Dictionary<string, string> variableTypeName;

        public ExpressionTreeJoinResolver(Dictionary<string,string> typeAs)
        {
            this.typeAs = typeAs;
            this.variableTypeName = new Dictionary<string, string>();
        }

        public string ResolveBinaryJoinExpression(LambdaExpression lambdaExpression, Type type)
        {
            if (typeof(BinaryExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                return $" JOIN [{type.Name}] AS [{typeAs.ElementAt(param.Count-1).Value}] ON " + ResolveRecursiveBinaryExpression(lambdaExpression.Body as BinaryExpression);
            }
            throw new SqlBuilderException("Failed to do join, expression cannot be translated.");
        }

        public string ResolveRecursiveBinaryExpression(BinaryExpression binaryExpression)
        {
            string result = "";
            if(typeof(MemberExpression).IsAssignableFrom(binaryExpression.Left.GetType()) && typeof(MemberExpression).IsAssignableFrom(binaryExpression.Right.GetType()))
            {
                var left = binaryExpression.Left as MemberExpression;
                var leftProp = left.Expression as ParameterExpression;
                var right = binaryExpression.Right as MemberExpression;
                var rightProp = right.Expression as ParameterExpression;
                result += $"[{variableTypeName[leftProp.Name]}].[{left.Member.Name}]";
                result += " = ";
                result += $"[{variableTypeName[rightProp.Name]}].[{right.Member.Name}]";
            }
            else
            {
                if (typeof(BinaryExpression).IsAssignableFrom(binaryExpression.Left.GetType()) && typeof(BinaryExpression).IsAssignableFrom(binaryExpression.Right.GetType()))
                {
                    result += ResolveRecursiveBinaryExpression(binaryExpression.Left as BinaryExpression);
                    result += " AND ";
                    result += ResolveRecursiveBinaryExpression(binaryExpression.Right as BinaryExpression);
                }
            }
            return result;
        }
    }
}
