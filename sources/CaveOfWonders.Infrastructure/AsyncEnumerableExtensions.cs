namespace DustInTheWind.CaveOfWonders.Infrastructure;

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        List<T> list = [];

        await foreach (T item in source)
            list.Add(item);

        return list;
    }

    public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        await foreach (TSource item in source)
            yield return selector.Invoke(item);
    }

    public static async Task<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        IAsyncEnumerator<TSource> enumerator = source.GetAsyncEnumerator();

        bool collectionHasItems = await enumerator.MoveNextAsync();

        if (collectionHasItems)
            return enumerator.Current;

        throw new InvalidOperationException("The source sequence is empty.");
    }

    public static async Task<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        bool collectionHasItems = false;

        await foreach (TSource item in source)
        {
            collectionHasItems = true;

            bool isSelected = predicate.Invoke(item);

            if (isSelected)
                return item;
        }

        if (collectionHasItems)
            throw new InvalidOperationException("No element satisfies the condition in predicate");

        throw new InvalidOperationException("The source sequence is empty.");
    }

    public static async Task<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        IAsyncEnumerator<TSource> enumerator = source.GetAsyncEnumerator();

        bool collectionHasItems = await enumerator.MoveNextAsync();

        if (!collectionHasItems)
            throw new InvalidOperationException("The source sequence is empty.");

        TSource item = enumerator.Current;

        bool collectionHasMoreItems = await enumerator.MoveNextAsync();

        if (collectionHasMoreItems)
            throw new InvalidOperationException("The input sequence contains more than one element.");

        return item;
    }

    public static async Task<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        bool collectionHasItems = false;
        bool itemWasFound = false;
        TSource foundItem = default;

        await foreach (TSource item in source)
        {
            collectionHasItems = true;

            bool isMatch = predicate.Invoke(item);

            if (isMatch)
            {
                if (itemWasFound)
                    throw new InvalidOperationException("The input sequence contains more than one element.");

                itemWasFound = true;
                foundItem = item;
            }
        }

        if (itemWasFound)
            return foundItem;

        if (collectionHasItems)
            throw new InvalidOperationException("No element satisfies the condition in predicate");

        throw new InvalidOperationException("The source sequence is empty.");
    }

    public static async Task<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        bool itemWasFound = false;
        TSource foundItem = default;

        await foreach (TSource item in source)
        {
            if (itemWasFound)
                throw new InvalidOperationException("The input sequence contains more than one element.");

            itemWasFound = true;
            foundItem = item;
        }

        return itemWasFound
            ? foundItem
            : default;
    }

    public static async Task<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        bool itemWasFound = false;
        TSource foundItem = default;

        await foreach (TSource item in source)
        {
            bool isMatch = predicate.Invoke(item);

            if (isMatch)
            {
                if (itemWasFound)
                    throw new InvalidOperationException("The input sequence contains more than one element.");

                itemWasFound = true;
                foundItem = item;
            }
        }

        return itemWasFound
            ? foundItem
            : default;
    }
}