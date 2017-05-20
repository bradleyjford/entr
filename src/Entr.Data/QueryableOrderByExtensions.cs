using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Entr.Data
{
    public static class QueryableOrderByExtensions
    {
        static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method => method.Name == "OrderBy" && method.GetParameters().Length == 2);

        static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method => method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        static readonly MethodInfo ThenByMethod =
            typeof(Queryable).GetMethods().Single(method => method.Name == "ThenBy" && method.GetParameters().Length == 2);

        static readonly MethodInfo ThenByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method => method.Name == "ThenByDescending" && method.GetParameters().Length == 2);

        public static IOrderedQueryable<TSource> OrderByProperty<TSource>(
            this IQueryable<TSource> source, 
            string propertyName)
        {
            return InvokeOrderByMethod(source, propertyName, OrderByMethod);
        }

        public static IOrderedQueryable<TSource> OrderByPropertyDescending<TSource>(
            this IQueryable<TSource> source,
            string propertyName)
        {
            return InvokeOrderByMethod(source, propertyName, OrderByDescendingMethod);
        }

        public static IOrderedQueryable<TSource> ThenByProperty<TSource>(
            this IOrderedQueryable<TSource> source,
            string propertyName)
        {
            return InvokeOrderByMethod(source, propertyName, ThenByMethod);
        }

        public static IOrderedQueryable<TSource> ThenByPropertyDescending<TSource>(
            this IOrderedQueryable<TSource> source,
            string propertyName)
        {
            return InvokeOrderByMethod(source, propertyName, ThenByDescendingMethod);
        }

        static IOrderedQueryable<TSource> InvokeOrderByMethod<TSource>(
            IQueryable<TSource> source,
            string propertyName,
            MethodInfo orderByMethod)
        {
            var parameter = Expression.Parameter(typeof(TSource), "x");

            var orderByProperty = CreatePropertyExpression(propertyName, parameter);

            var lambda = Expression.Lambda(orderByProperty, new[] { parameter });

            var genericMethod = orderByMethod.MakeGenericMethod(new[] { typeof(TSource), orderByProperty.Type });

            return (IOrderedQueryable<TSource>)genericMethod.Invoke(null, new object[] { source, lambda });
        }

        static Expression CreatePropertyExpression(string propertyName, Expression parameter)
        {
            var orderByProperty = parameter;

            foreach (var member in propertyName.Split('.'))
            {
                orderByProperty = Expression.PropertyOrField(orderByProperty, member);
            }

            return orderByProperty;
        }

        public static IOrderedQueryable<T> OrderBy<T>(
           this IQueryable<T> source,
           IEnumerable<SortDescriptor> sortDescriptors)
        {
            if (sortDescriptors == null) throw new ArgumentNullException(nameof(sortDescriptors));
            if (!sortDescriptors.Any()) throw new ArgumentException("No SortDescriptors specified", nameof(sortDescriptors));

            IOrderedQueryable<T> result = null;

            foreach (var sortDescriptor in sortDescriptors)
            {
                if (sortDescriptor.Direction == SortDirection.Ascending)
                {
                    result = result == null ?
                        source.OrderByProperty(sortDescriptor.PropertyName) :
                        result.ThenByProperty(sortDescriptor.PropertyName);
                }
                else
                {
                    result = result == null ?
                        source.OrderByPropertyDescending(sortDescriptor.PropertyName) :
                        result.ThenByPropertyDescending(sortDescriptor.PropertyName);
                }
            }

            return result;
        }
    }
}
