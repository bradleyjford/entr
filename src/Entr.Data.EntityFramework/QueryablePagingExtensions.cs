using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Entr.Data.EntityFramework;

public static class QueryablePagingExtensions
{
    static readonly MethodInfo PagedMethod = typeof(QueryablePagingExtensions).GetMethods()
        .Single(method =>
            method.Name == "ToPagedResultAsync" &&
            method.GetParameters()[2].ParameterType == typeof(SortDescriptor[]));

    public static Task<IPagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        IPagingOptions options,
        SortDescriptor defaultSort)
        where T : class
    {
        return ToPagedResultAsync(source, options, new[] { defaultSort });
    }

    public static async Task<IPagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        IPagingOptions options,
        SortDescriptor[] defaultSort)
        where T : class
    {
        var firstResult = (options.PageNumber - 1) * options.PageSize;

        var sortDescriptors = options.SortDescriptors;

        if (!sortDescriptors.Any())
        {
            sortDescriptors = defaultSort;
        }

        var itemCount = await source.CountAsync();

        var items = await source
            .OrderBy(sortDescriptors)
            .Skip(firstResult)
            .Take(options.PageSize)
            .ToArrayAsync();

        return new PagedResult<T>(options.PageNumber, options.PageSize, items, itemCount);
    }

    public static dynamic ToPagedResultAsync(
        this IQueryable source,
        Type type,
        IPagingOptions options,
        SortDescriptor defaultSort)
    {
        return ToPagedResultAsync(source, type, options, new[] { defaultSort });
    }

    public static dynamic ToPagedResultAsync(
        this IQueryable source,
        Type type,
        IPagingOptions options,
        SortDescriptor[] defaultSort)
    {
        var result = PagedMethod.MakeGenericMethod(type)
            .Invoke(null, new object[] { source, options, defaultSort });

        return result!;
    }
}
