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

using System.Collections;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Utils;

internal class BucketCollection<TKey, TItem> : IEnumerable<TItem>
{
    private readonly Dictionary<TKey, List<TItem>> items = new();

    public int Count => items.Sum(x => x.Value.Count);

    public List<TItem> GetBucket(TKey key)
    {
        bool success = items.TryGetValue(key, out List<TItem> bucket);

        return success ? bucket : null;
    }

    public List<TItem> GetOrCreateBucket(TKey key)
    {
        bool success = items.TryGetValue(key, out List<TItem> bucket);

        if (!success)
        {
            bucket = new List<TItem>();
            items.Add(key, bucket);
        }

        return bucket;
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return items.Values.SelectMany(x => x).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}