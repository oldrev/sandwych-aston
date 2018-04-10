using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Pidgin;
using Sandwych.Aston.Linq.Expressions;

namespace Sandwych.Aston.Linq
{

    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Where<TSource>(
            this IQueryable<TSource> source,
            string sexpr)
        {
            var predicate = ParseWherePredicate<TSource>(sexpr);
            var linqWhereType = Where_TSource_2(typeof(TSource));
            var callWhere = Expression.Call(null, linqWhereType, source.Expression, Expression.Quote(predicate));
            return source.Provider.CreateQuery<TSource>(callWhere);
        }

        public static IQueryable Where(this IQueryable source, Type sourceType, string sexpr)
        {
            var predicate = ParseUntypedWherePredicate(sourceType, sexpr);
            var linqWhereType = Where_TSource_2(sourceType);
            var callWhere = Expression.Call(null, linqWhereType, source.Expression, Expression.Quote(predicate));
            return source.Provider.CreateQuery(callWhere);
        }

        private static Expression<Func<TSource, bool>> ParseWherePredicate<TSource>(string sexpr)
        {
            var parseContext = new LinqExpressionParseContext(typeof(TSource));
            parseContext.RegisterParameter(typeof(TSource), "it");
            var parser = new AstonParser<Expression>(parseContext);
            var paramIt = (ParameterExpression)parseContext.Parameters.Values.Single();
            var parsedExpression = parser.RootParser.ParseOrThrow(sexpr);
            var evalContext = new EvaluationContext();
            var filterExpression = (new BindingEvaluationContextExpressionVisitor(evalContext)).Visit(parsedExpression);
            return Expression.Lambda<Func<TSource, bool>>(filterExpression, paramIt);
        }

        private static Expression ParseUntypedWherePredicate(
            Type sourceType, string sexpr)
        {
            var parser = new AstonParser<Expression>(new LinqExpressionParseContext(sourceType));
            var param = Expression.Parameter(sourceType);
            var filterExpr = parser.Parse(sexpr).Value;
            var lambdaType = typeof(Func<,>).GetTypeInfo().MakeGenericType(sourceType, typeof(bool));
            return Expression.Lambda(lambdaType, filterExpr, param);
        }

        private static MethodInfo Where_TSource_2(Type TSource)
        {
            var method =
                    new Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>(Queryable.Where)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition();
            return method.MakeGenericMethod(TSource);
        }



    }


}
