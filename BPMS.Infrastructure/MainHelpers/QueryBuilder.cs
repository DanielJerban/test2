using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Infrastructure.MainHelpers;

/// <summary>
/// Generic IQueryable filter implementation.  Based upon configuration of FilterRules 
/// mapping to the data source.  When applied, acts as an advanced filter mechanism.
/// </summary>
public static class QueryBuilder
{

    public static IQueryable<T> BuildQuery<T>(this IList<T> queryable, FilterRuleDto filterRule, bool useIndexedProperty = false, string indexedPropertyName = null)
    {
        string parsedQuery;
        return BuildQuery(queryable.AsQueryable(), filterRule, out parsedQuery, useIndexedProperty, indexedPropertyName);
    }
    /// <summary>
    /// Gets or sets a value indicating whether incoming dates in the filter should be parsed as UTC.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [parse dates as UTC]; otherwise, <c>false</c>.
    /// </value>
    public static bool ParseDatesAsUtc { get; set; }
    public static IQueryable BuildQuery2(this IEnumerable queryable, FilterRuleDto filterRule, bool useIndexedProperty = false, string indexedPropertyName = null, Type type = null)
    {
        string parsedQuery;
        return BuildQuery2(queryable.AsQueryable(), filterRule, out parsedQuery, useIndexedProperty, indexedPropertyName, type);
    }

    public static IQueryable BuildQuery2(this IQueryable queryable, FilterRuleDto filterRule, out string parsedQuery,
        bool useIndexedProperty = false, string indexedPropertyName = null, Type type = null)
    {
        if (filterRule == null)
        {
            parsedQuery = "";
            return queryable;
        }
        var pe = Expression.Parameter(type, "item");
        var expressionTree = BuildExpressionTree(pe, filterRule, useIndexedProperty, indexedPropertyName);
        if (expressionTree == null)
        {
            parsedQuery = "";
            return queryable;
        }
        parsedQuery = expressionTree.ToString();
        var whereCallExpression = Expression.Call(
            typeof(Queryable),
            "Where",
            new[] { queryable.ElementType },
            queryable.Expression,
            Expression.Lambda(expressionTree, pe));

        var filteredResults = queryable.Provider.CreateQuery<dynamic>(whereCallExpression);

        return filteredResults;
    }
    /// <summary>
    /// Gets the filtered collection after applying the provided filter rules. 
    /// Returns the string representation for diagnostic purposes.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="filterRule">The filter rule.</param>
    /// <param name="parsedQuery">The parsed query.</param>
    /// <param name="useIndexedProperty">Whether or not to use indexed property</param>
    /// <param name="indexedPropertyName">The indexable property to use</param>
    /// <returns>Filtered IQueryable.</returns>
    public static IQueryable<T> BuildQuery<T>(this IQueryable<T> queryable, FilterRuleDto filterRule, out string parsedQuery, bool useIndexedProperty = false, string indexedPropertyName = null)
    {
        if (filterRule == null)
        {
            parsedQuery = "";
            return queryable;
        }

        var pe = Expression.Parameter(typeof(T), "item");
        var expressionTree = BuildExpressionTree(pe, filterRule, useIndexedProperty, indexedPropertyName);
        if (expressionTree == null)
        {
            parsedQuery = "";
            return queryable;
        }

        parsedQuery = expressionTree.ToString();

        var whereCallExpression = Expression.Call(
            typeof(Queryable),
            "Where",
            new[] { queryable.ElementType },
            queryable.Expression,
            Expression.Lambda<Func<T, bool>>(expressionTree, pe));

        var filteredResults = queryable.Provider.CreateQuery<T>(whereCallExpression);

        return filteredResults;

    }

    public static Expression BuildExpressionTree(ParameterExpression pe, FilterRuleDto rule, bool useIndexedProperty = false, string indexedPropertyName = null)
    {

        if (rule.Rules != null && rule.Rules.Any())
        {
            var expressions =
                rule.Rules.Select(childRule => BuildExpressionTree(pe, childRule, useIndexedProperty, indexedPropertyName))
                    .Where(expression => expression != null)
                    .ToList();

            var expressionTree = expressions.First();

            var counter = 1;
            while (counter < expressions.Count)
            {
                expressionTree = rule.Condition.ToLower() == "or"
                    ? Expression.Or(expressionTree, expressions[counter])
                    : Expression.And(expressionTree, expressions[counter]);
                counter++;
            }

            return expressionTree;
        }
        if (rule.Field != null)
        {
            Type type;

            switch (rule.Type)
            {
                case "integer":
                    type = typeof(int);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "string":
                    type = typeof(string);
                    break;
                case "date":
                case "datetime":
                    type = typeof(DateTime);
                    break;
                case "boolean":
                    type = typeof(bool);
                    break;
                default:
                    throw new Exception($"Unexpected data type {rule.Type}");
            }

            Expression propertyExp = null;
            if (useIndexedProperty)
            {
                propertyExp = Expression.Property(pe, indexedPropertyName, Expression.Constant(rule.Field));
            }
            else
            {
                try
                {

                    propertyExp = Expression.Property(pe, rule.Field);
                }
                catch (Exception e)
                {
                    throw new Exception();
                }
            }

            Expression expression;

            switch (rule.Operator.ToLower())
            {
                case "in":
                    expression = In(type, rule.Value, propertyExp);
                    break;
                case "not_in":
                    expression = NotIn(type, rule.Value, propertyExp);
                    break;
                case "equal":
                    expression = Equals(type, rule.Value, propertyExp);
                    break;
                case "not_equal":
                    expression = NotEquals(type, rule.Value, propertyExp);
                    break;
                case "between":
                    expression = Between(type, rule.Value, propertyExp);
                    break;
                case "not_between":
                    expression = NotBetween(type, rule.Value, propertyExp);
                    break;
                case "less":
                    expression = LessThan(type, rule.Value, propertyExp);
                    break;
                case "less_or_equal":
                    expression = LessThanOrEqual(type, rule.Value, propertyExp);
                    break;
                case "greater":
                    expression = GreaterThan(type, rule.Value, propertyExp);
                    break;
                case "greater_or_equal":
                    expression = GreaterThanOrEqual(type, rule.Value, propertyExp);
                    break;
                case "begins_with":
                    expression = BeginsWith(type, rule.Value, propertyExp);
                    break;
                case "not_begins_with":
                    expression = NotBeginsWith(type, rule.Value, propertyExp);
                    break;
                case "contains":
                    expression = Contains(type, rule.Value, propertyExp);
                    break;
                case "not_contains":
                    expression = NotContains(type, rule.Value, propertyExp);
                    break;
                case "ends_with":
                    expression = EndsWith(type, rule.Value, propertyExp);
                    break;
                case "not_ends_with":
                    expression = NotEndsWith(type, rule.Value, propertyExp);
                    break;
                case "is_empty":
                    expression = IsEmpty(propertyExp);
                    break;
                case "is_not_empty":
                    expression = IsNotEmpty(propertyExp);
                    break;
                case "is_null":
                    expression = IsNull(propertyExp);
                    break;
                case "is_not_null":
                    expression = IsNotNull(propertyExp);
                    break;
                default:
                    throw new Exception($"Unknown expression operator: {rule.Operator}");
            }

            return expression;


        }
        return null;

    }

    private static List<ConstantExpression> GetConstants(Type type, string value, bool isCollection)
    {
        if (type == typeof(DateTime) && ParseDatesAsUtc)
        {
            DateTime tDate;
            if (isCollection)
            {
                var vals =
                    value.Split(new[] { ",", "[", "]", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .Select(
                            p =>
                                DateTime.TryParse(p.Trim(), CultureInfo.InvariantCulture,
                                    DateTimeStyles.AdjustToUniversal, out tDate)
                                    ? (DateTime?)
                                    tDate
                                    : null).Select(p =>
                            Expression.Constant(p, type));
                return vals.ToList();
            }
            else
            {
                return new List<ConstantExpression>()
                {
                    Expression.Constant(DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal, out tDate)
                        ? (DateTime?)
                        tDate
                        : null)
                };
            }
        }
        else
        {
            if (isCollection)
            {
                var tc = TypeDescriptor.GetConverter(type);
                var vals =
                    value.Split(new[] { ",", "[", "]", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .Select(p => tc.ConvertFromString(p.Trim())).Select(p =>
                            Expression.Constant(p, type));
                return vals.ToList();
            }
            else
            {
                var tc = TypeDescriptor.GetConverter(type);
                return new List<ConstantExpression>()
                {
                    Expression.Constant(tc.ConvertFromString(value.Trim()))
                };
            }
        }



    }

    #region Expression Types

    public static Expression GetNullCheckExpression(Expression propertyExp)
    {
        var isNullable = !propertyExp.Type.IsValueType || Nullable.GetUnderlyingType(propertyExp.Type) != null;

        if (isNullable)
        {
            return Expression.NotEqual(propertyExp,
                Expression.Constant(propertyExp.Type.GetDefaultValue(), propertyExp.Type));

        }
        return Expression.Equal(Expression.Constant(true, typeof(bool)),
            Expression.Constant(true, typeof(bool)));
    }



    public static Expression IsNull(Expression propertyExp)
    {
        var isNullable = !propertyExp.Type.IsValueType || Nullable.GetUnderlyingType(propertyExp.Type) != null;

        if (isNullable)
        {
            var someValue = Expression.Constant(null, propertyExp.Type);

            Expression exOut = Expression.Equal(propertyExp, someValue);

            return exOut;
        }
        return Expression.Equal(Expression.Constant(true, typeof(bool)),
            Expression.Constant(false, typeof(bool)));
    }

    public static Expression IsNotNull(Expression propertyExp)
    {
        return Expression.Not(IsNull(propertyExp));
    }

    public static Expression IsEmpty(Expression propertyExp)
    {
        var someValue = Expression.Constant(0, typeof(int));

        var nullCheck = GetNullCheckExpression(propertyExp);

        Expression exOut;

        if (IsGenericList(propertyExp.Type))
        {

            exOut = Expression.Property(propertyExp, propertyExp.Type.GetProperty("Count"));

            exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
        }
        else
        {
            exOut = Expression.Property(propertyExp, typeof(string).GetProperty("Length"));

            exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
        }

        return exOut;
    }

    public static Expression IsNotEmpty(Expression propertyExp)
    {
        return Expression.Not(IsEmpty(propertyExp));
    }

    public static Expression Contains(Type type, string value, Expression propertyExp)
    {
        var someValue = Expression.Constant(value.ToLower(), typeof(string));

        var nullCheck = GetNullCheckExpression(propertyExp);

        var method = propertyExp.Type.GetMethod("Contains", new[] { type });

        Expression exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

        exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

        return exOut;
    }

    public static Expression NotContains(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(Contains(type, value, propertyExp));
    }

    public static Expression EndsWith(Type type, string value, Expression propertyExp)
    {
        var someValue = Expression.Constant(value.ToLower(), typeof(string));

        var nullCheck = GetNullCheckExpression(propertyExp);

        var method = propertyExp.Type.GetMethod("EndsWith", new[] { type });

        Expression exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

        exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

        return exOut;
    }

    public static Expression NotEndsWith(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(EndsWith(type, value, propertyExp));
    }

    public static Expression BeginsWith(Type type, string value, Expression propertyExp)
    {
        var someValue = Expression.Constant(value.ToLower(), typeof(string));

        var nullCheck = GetNullCheckExpression(propertyExp);

        var method = propertyExp.Type.GetMethod("StartsWith", new[] { type });

        Expression exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

        exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

        return exOut;
    }

    public static Expression NotBeginsWith(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(BeginsWith(type, value, propertyExp));
    }



    public static Expression NotEquals(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(Equals(type, value, propertyExp));
    }



    public static Expression Equals(Type type, string value, Expression propertyExp)
    {
        Expression someValue = GetConstants(type, value, false).First();

        Expression exOut;
        if (type == typeof(string))
        {
            var nullCheck = GetNullCheckExpression(propertyExp);

            exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            someValue = Expression.Call(someValue, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
        }
        else
        {
            exOut = Expression.Equal(propertyExp, Expression.Convert(someValue, propertyExp.Type));
        }

        return exOut;


    }

    public static Expression LessThan(Type type, string value, Expression propertyExp)
    {
        var someValue = GetConstants(type, value, false).First();

        Expression exOut = Expression.LessThan(propertyExp, Expression.Convert(someValue, propertyExp.Type));


        return exOut;


    }

    public static Expression LessThanOrEqual(Type type, string value, Expression propertyExp)
    {
        var someValue = GetConstants(type, value, false).First();
        Expression exOut = Expression.LessThanOrEqual(propertyExp, Expression.Convert(someValue, propertyExp.Type));


        return exOut;


    }

    public static Expression GreaterThan(Type type, string value, Expression propertyExp)
    {

        var someValue = GetConstants(type, value, false).First();



        Expression exOut = Expression.GreaterThan(propertyExp, Expression.Convert(someValue, propertyExp.Type));


        return exOut;


    }

    public static Expression GreaterThanOrEqual(Type type, string value, Expression propertyExp)
    {
        var someValue = GetConstants(type, value, false).First();

        Expression exOut = Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(someValue, propertyExp.Type));


        return exOut;


    }

    public static Expression Between(Type type, string value, Expression propertyExp)
    {
        var someValue = GetConstants(type, value, true);


        Expression exBelow = Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(someValue[0], propertyExp.Type));
        Expression exAbove = Expression.LessThanOrEqual(propertyExp, Expression.Convert(someValue[1], propertyExp.Type));

        return Expression.And(exBelow, exAbove);


    }

    public static Expression NotBetween(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(Between(type, value, propertyExp));
    }

    public static Expression In(Type type, string value, Expression propertyExp)
    {


        var someValues = GetConstants(type, value, true);

        var nullCheck = GetNullCheckExpression(propertyExp);

        if (IsGenericList(propertyExp.Type))
        {
            var genericType = propertyExp.Type.GetGenericArguments().First();
            var method = propertyExp.Type.GetMethod("Contains", new[] { genericType });
            Expression exOut;

            if (someValues.Count > 1)
            {
                exOut = Expression.Call(propertyExp, method, Expression.Convert(someValues[0], genericType));
                var counter = 1;
                while (counter < someValues.Count)
                {
                    exOut = Expression.Or(exOut,
                        Expression.Call(propertyExp, method, Expression.Convert(someValues[counter], genericType)));
                    counter++;
                }
            }
            else
            {
                exOut = Expression.Call(propertyExp, method, Expression.Convert(someValues.First(), genericType));
            }


            return Expression.AndAlso(nullCheck, exOut);
        }
        else
        {
            Expression exOut;

            if (someValues.Count > 1)
            {
                if (type == typeof(string))
                {

                    exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    exOut = Expression.Equal(exOut, Expression.Convert(someValues[0], propertyExp.Type));
                    var counter = 1;
                    while (counter < someValues.Count)
                    {
                        exOut = Expression.Or(exOut,
                            Expression.Equal(
                                Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes)),
                                Expression.Convert(someValues[counter], propertyExp.Type)));
                        counter++;
                    }
                }
                else
                {
                    exOut = Expression.Equal(propertyExp, Expression.Convert(someValues[0], propertyExp.Type));
                    var counter = 1;
                    while (counter < someValues.Count)
                    {
                        exOut = Expression.Or(exOut,
                            Expression.Equal(propertyExp, Expression.Convert(someValues[counter], propertyExp.Type)));
                        counter++;
                    }
                }



            }
            else
            {
                if (type == typeof(string))
                {

                    exOut = Expression.Call(propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    exOut = Expression.Equal(exOut, someValues.First());
                }
                else
                {
                    exOut = Expression.Equal(propertyExp, Expression.Convert(someValues.First(), propertyExp.Type));
                }
            }


            return Expression.AndAlso(nullCheck, exOut);
        }


    }

    public static Expression NotIn(Type type, string value, Expression propertyExp)
    {
        return Expression.Not(In(type, value, propertyExp));
    }

    #endregion

    public static bool IsGenericList(this Type o)
    {
        var isGenericList = false;

        var oType = o;

        if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
            isGenericList = true;

        return isGenericList;
    }

    public static object GetDefaultValue(this Type type)
    {
        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }
    public static IEnumerable<T> CastTo<T>(this IEnumerable<T> list)
    {
        var list2 = list.Cast<T>().ToList();
        return list2;
    }

}