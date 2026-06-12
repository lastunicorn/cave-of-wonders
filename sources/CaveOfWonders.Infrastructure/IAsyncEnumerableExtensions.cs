// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Infrastructure;

public static class IAsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
    {
        List<T> list = [];

        await foreach (T item in source.WithCancellation(cancellationToken))
            list.Add(item);

        return list;
    }

    public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));

        await foreach (TSource item in source.WithCancellation(cancellationToken))
            yield return selector.Invoke(item);
    }
}