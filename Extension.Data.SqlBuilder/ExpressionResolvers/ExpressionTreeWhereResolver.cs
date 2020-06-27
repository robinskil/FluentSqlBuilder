using Extension.Data.SqlBuilder.ExpressionResolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Extension.Data.SqlBuilder
{
    public class ExpressionTreeWhereResolver : ExpressionTreeResolver
    {
        protected readonly IDictionary<string, string> variableTypeName;

        public ExpressionTreeWhereResolver(IDictionary<string, object> parameters,IDictionary<string,string> typeAs) : base(parameters,typeAs)
        {
            variableTypeName = new Dictionary<string, string>();
        }

        /// <summary>
        /// Splits lambda expression into seperate clauses to solve and combine them into a where statement.
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public string ResolveLambdaExpressionToWhereClause(LambdaExpression lambdaExpression)
        {
            if (typeof(BinaryExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                return ResolveRecursiveBoolExpression(lambdaExpression.Body as BinaryExpression);
            }
            //If its just a method call within the expression that results in a bool result than resolve it as a method call expression.
            else if (typeof(MethodCallExpression).IsAssignableFrom(lambdaExpression.Body.GetType()))
            {
                var param = lambdaExpression.Parameters;
                for (int i = 0; i < param.Count; i++)
                {
                    variableTypeName.Add(param[i].Name, typeAs.ElementAt(i).Value);
                }
                return ResolveMethodCallExpressionToBoolExpression(lambdaExpression.Body as MethodCallExpression);
            }
            throw new SqlBuilderException("Couldn't resolve where expression");
        }
        private string ResolveRecursiveBoolExpression(BinaryExpression binaryExpression)
        {
            var builder = new StringBuilder();
            var left = binaryExpression.Left;
            var right = binaryExpression.Right;
            if ((typeof(MemberExpression).IsAssignableFrom(left.GetType()) && typeof(ConstantExpression).IsAssignableFrom(right.GetType())))
            {
                builder.Append(ResolveConstantMemberExpressionToBoolExpression(binaryExpression, left as MemberExpression, right as ConstantExpression));
            }
            else if ((typeof(MemberExpression).IsAssignableFrom(right.GetType()) && typeof(ConstantExpression).IsAssignableFrom(left.GetType())))
            {
                builder.Append(ResolveConstantMemberExpressionToBoolExpression(binaryExpression, right as MemberExpression, left as ConstantExpression));
            }
            else if (typeof(MemberExpression).IsAssignableFrom(left.GetType()) && typeof(MemberExpression).IsAssignableFrom(right.GetType()))
            {
                builder.Append(ResolveMemberExpressionsToBoolExpression(binaryExpression,left as MemberExpression,right as MemberExpression));
            }
            else
            {
                if (typeof(BinaryExpression).IsAssignableFrom(left.GetType()) && typeof(BinaryExpression).IsAssignableFrom(right.GetType()))
                {
                    builder.Append(ResolveRecursiveBoolExpression(left as BinaryExpression));
                    builder.Append(WhereConcatenation(binaryExpression));
                    builder.Append(ResolveRecursiveBoolExpression(right as BinaryExpression));
                }
                else if (typeof(MethodCallExpression).IsAssignableFrom(left.GetType()) && typeof(BinaryExpression).IsAssignableFrom(right.GetType()))
                {
                    builder.Append(ResolveMethodCallExpressionToBoolExpression(left as MethodCallExpression));
                    builder.Append(WhereConcatenation(binaryExpression));
                    builder.Append(ResolveRecursiveBoolExpression(right as BinaryExpression));
                }
                else if (typeof(BinaryExpression).IsAssignableFrom(left.GetType()) && typeof(MethodCallExpression).IsAssignableFrom(right.GetType()))
                {
                    builder.Append(ResolveRecursiveBoolExpression(left as BinaryExpression));
                    builder.Append(WhereConcatenation(binaryExpression));
                    builder.Append(ResolveMethodCallExpressionToBoolExpression(right as MethodCallExpression));
                }
            }
            return builder.ToString();
        }
        private string WhereConcatenation(BinaryExpression baseExpr)
        {
            switch (baseExpr.NodeType)
            {
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.OrElse:
                    return " OR ";
                default:
                    throw new ApplicationException("Failed to create at where concatenation, incorrect where statement");
            }
        }
        private string GetComparerStringFromBinaryExpression(BinaryExpression baseExpresison)
        {
            switch (baseExpresison.NodeType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    throw new NotSupportedException("This expression type is not supported");
            }
        }
        private string ResolveConstantMemberExpressionToBoolExpression(BinaryExpression baseExpresison, MemberExpression memberExpression, ConstantExpression constantExpression)
        {
            string compareType = GetComparerStringFromBinaryExpression(baseExpresison);
            string right = CreateParameterForValue((constantExpression as ConstantExpression).Value);
            return $"{ResolveMemberExpressionParameterName(memberExpression,variableTypeName)} {compareType} {right}";
        }

        private string ResolveMemberExpressionsToBoolExpression(BinaryExpression baseExpression,MemberExpression memberLeft, MemberExpression memberRight)
        {
            string compareType = GetComparerStringFromBinaryExpression(baseExpression);
            string propertyName;
            string paramName;
            var leftExpression = GetBaseMemberExpressionFromParentMember(memberLeft);
            var rightExpression = GetBaseMemberExpressionFromParentMember(memberRight);
            if(typeof(ConstantExpression).IsAssignableFrom(leftExpression.Expression.GetType()) && typeof(ParameterExpression).IsAssignableFrom(rightExpression.Expression.GetType()))
            {
                paramName = CreateParameterForValue(ResolveMemberExpressionToValue(memberLeft));
                propertyName = ResolveMemberExpressionParameterName(rightExpression,variableTypeName);
            }
            else
            {
                paramName = CreateParameterForValue(ResolveMemberExpressionToValue(memberRight));
                propertyName = ResolveMemberExpressionParameterName(leftExpression,variableTypeName);
            }
            return $"{propertyName} {compareType} {paramName}";
        }

        #region Method call expression resolver
        /// <summary>
        /// Resolve method call expression to a single where clause
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        private string ResolveMethodCallExpressionToBoolExpression(MethodCallExpression methodCallExpression)
        {
            var arg = methodCallExpression.Arguments[0] as MemberExpression;
            var param = arg.Expression as ParameterExpression;
            //check methods names and use the correct where statement
            switch (methodCallExpression.Method.Name.ToUpper())
            {
                //In and NOTIN have to resolve the array provided by the function caller.
                case "IN":
                    if (typeof(NewArrayExpression).IsAssignableFrom(methodCallExpression.Arguments[1].GetType()))
                    {
                        return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] IN ({ResolveArrayInitToVal(methodCallExpression.Arguments[1] as NewArrayExpression)})";
                    }
                    else
                    {
                        return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] IN ({ResolveMemberAccessOnArrayExpressionToVal(methodCallExpression.Arguments[1] as MemberExpression)})";
                    }
                case "NOTIN":
                    if (typeof(NewArrayExpression).IsAssignableFrom(methodCallExpression.Arguments[1].GetType()))
                    {
                        return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] NOT IN ({ResolveArrayInitToVal(methodCallExpression.Arguments[1] as NewArrayExpression)})";
                    }
                    else
                    {
                        return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] NOT IN ({ResolveMemberAccessOnArrayExpressionToVal(methodCallExpression.Arguments[1] as MemberExpression)})";
                    }
                case "LIKE":
                    return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] LIKE '${ResolveMethodCallParameter(methodCallExpression.Arguments[1])}'";
                case "NOTLIKE":
                    return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] NOT LIKE '${ResolveMethodCallParameter(methodCallExpression.Arguments[1])}'";
                case "BETWEEN":
                    return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] BETWEEN {ResolveMethodCallParameter(methodCallExpression.Arguments[1])} AND {ResolveMethodCallParameter(methodCallExpression.Arguments[2])}";
                case "NOTBETWEEN":
                    return $"[{variableTypeName[param.Name]}].[{arg.Member.Name}] NOT BETWEEN {ResolveMethodCallParameter(methodCallExpression.Arguments[1])} AND {ResolveMethodCallParameter(methodCallExpression.Arguments[2])}";
                default:
                    throw new NotSupportedException("This where statement is not supported");
            }
        }
        private object ResolveMemberExpressionToValue(MemberExpression memberExpression)
        {
            if(typeof(MemberExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                var value = ResolveMemberExpressionToValue(memberExpression.Expression as MemberExpression);
                //bug here
                var field = value.GetType().GetField(memberExpression.Member.Name);
                if(field != null)
                {
                    return field.GetValue(value);
                }
                var property = value.GetType().GetProperty(memberExpression.Member.Name);
                if(property != null)
                {
                    return property.GetValue(value);
                }
                throw new Exception();
            }
            else if(typeof(ConstantExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
            {
                var constant = (memberExpression.Expression as ConstantExpression);
                var field = constant.Value.GetType().GetField(memberExpression.Member.Name);
                if(field != null)
                {
                    return field.GetValue(constant.Value);
                }
                var property = constant.Value.GetType().GetProperty(memberExpression.Member.Name);
                if(property != null)
                {
                    return property.GetValue(constant.Value);
                }
                throw new Exception();
            }
            throw new Exception("Cannot handle this expression");
        }
        private string ResolveArrayInitToVal(NewArrayExpression newArrayExpression)
        {
            var builder = new StringBuilder();
            var values = new List<object>();
            foreach (var valExpression in newArrayExpression.Expressions)
            {
                if (typeof(ConstantExpression).IsAssignableFrom(valExpression.GetType()))
                {
                    var constant = valExpression as ConstantExpression;
                    values.Add(constant.Value);
                }
                else if (typeof(MemberExpression).IsAssignableFrom(valExpression.GetType()))
                {
                    var member = valExpression as MemberExpression;
                    var constant = member.Expression as ConstantExpression;
                    values.Add(member.Member.ReflectedType.GetField(member.Member.Name).GetValue(constant.Value));
                }
            }
            for (int i = 0; i < values.Count(); i++)
            {
                if (i < values.Count() - 1)
                {
                    builder.Append($"{CreateParameterForValue(values.ElementAt(i))},");
                }
                else
                {
                    builder.Append($"{CreateParameterForValue(values.ElementAt(i))}");
                }
            }
            return builder.ToString();
        }
        private string ResolveMemberAccessOnArrayExpressionToVal(MemberExpression memberExpression)
        {
            var builder = new StringBuilder();
            var values = (ResolveMemberExpressionToValue(memberExpression) as IEnumerable).Cast<object>();
            for (int i = 0; i < values.Count(); i++)
            {
                if (i < values.Count() - 1)
                {
                    builder.Append($"{CreateParameterForValue(values.ElementAt(i))},");
                }
                else
                {
                    builder.Append($"{CreateParameterForValue(values.ElementAt(i))}");
                }
            }
            return builder.ToString();
        }
        // /// <summary>
        // /// Resolves ex: p.y.t where t is an array that has been used in the original where function expression and returns the array that was given by the function caller.
        // /// </summary>
        // /// <param name="memberExpression"></param>
        // /// <returns></returns>
        // private object ResolveMemberArrayAccessToObject(MemberExpression memberExpression)
        // {
        //     if (typeof(MemberExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
        //     {
        //         var objRef = ResolveMemberArrayAccessToObject(memberExpression.Expression as MemberExpression);
        //         return objRef.GetType().GetProperty(memberExpression.Member.Name).GetValue(objRef);
        //     }
        //     else if (typeof(ConstantExpression).IsAssignableFrom(memberExpression.Expression.GetType()))
        //     {
        //         var constant = (memberExpression.Expression as ConstantExpression);
        //         return constant.Value.GetType().GetField(memberExpression.Member.Name).GetValue(constant.Value);
        //     }
        //     else throw new Exception("coudln't handle member expression conversion");
        // }
        private string ResolveMethodCallParameter(Expression expression)
        {
            if (typeof(ConstantExpression).IsAssignableFrom(expression.GetType()))
            {
                return CreateParameterForValue((expression as ConstantExpression).Value);
            }
            throw new NotSupportedException("Method call parameter couldn't be resolved");
        }
        #endregion
    }
}
