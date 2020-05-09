using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Repository.Pattern.Repositories;
using AirOut.Web.Models;

namespace AirOut.Web.Repositories
{
    public static class PARA_AirLineRepository
    {
        /// <summary>
        /// 普通Linq方法
        /// </summary>
        /// <param name="source"></param>
        /// <param name="topNum"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static IQueryable<PARA_AirLine> GetTop1<TKey>(IQueryable<PARA_AirLine> source, Expression<Func<PARA_AirLine, TKey>> OrderSelter, int topNum = 10)
        {
            source = source.AsQueryable().OrderBy(OrderSelter).Take(topNum);
            var param = OrderSelter.Parameters;
            return source;
        }

        /// <summary>
        /// 扩展IQueryable<PARA_AirLine>方法
        /// 可以直接.出来
        /// </summary>
        /// <param name="source"></param>
        /// <param name="topNum"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static IQueryable<T> GetTop<T,TKey>(this IQueryable<T> source,
            Expression<Func<T, TKey>> OrderSelter,
            Expression<Func<T, bool>> WhereSelter, int topNum = 10)
        {
            ParameterExpression param = (ParameterExpression)WhereSelter.Parameters[0];
            //NodeType 条件 AndAlso,OrElse,GrateThen,LessThen,Call...
            BinaryExpression operation = (BinaryExpression)WhereSelter.Body;

            #region 动态Expression
            
            //不等于 - x.Id != 2
            var leftExp = Expression.Property(param, "Id");
            var RightExp = Expression.Constant(2, typeof(int));
            var NotEqualExp = Expression.NotEqual(leftExp, RightExp);

            var propertyExp = Expression.Property(param, "EDITWHO");
            //Contains - x.EDITWHO.Contains("admin")
            var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            var constExp = Expression.Constant("admin");
            var ContainsExp = Expression.Call(propertyExp,containsMethod, constExp);
            //IsNullOrEmpty string.IsNullOrEmpty(x.EDITWHO)
            var strMethod = typeof(string).GetMethod("IsNullOrEmpty", new Type[] { typeof(string) });
            MethodCallExpression IsNullOrEmptyExp = Expression.Call(strMethod, propertyExp);
            var NotIsNullOrEmptyExp = Expression.Not(IsNullOrEmptyExp);

            var NewExp = Expression.OrElse(operation, ContainsExp);
            Expression<Func<T, bool>> NewOrderSelter = Expression.Lambda<Func<T, bool>>(NewExp, new ParameterExpression[] { param });

            #endregion

            source = source.AsQueryable().Where(NewOrderSelter).OrderBy(OrderSelter).Take(topNum);

            return source;
        }

        //public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        //{
        //}

        //public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
        //{
        //    return condition ? source.Where(predicate) : source;
        //}

        //public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, int, bool>> predicate, bool condition)
        //{
        //    return condition ? source.Where(predicate) : source;
        //}

        //public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        //{
        //    return condition ? source.Where(predicate) : source;
        //}

        //public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, bool condition)
        //{
        //    return condition ? source.Where(predicate) : source;
        //}
    }
}