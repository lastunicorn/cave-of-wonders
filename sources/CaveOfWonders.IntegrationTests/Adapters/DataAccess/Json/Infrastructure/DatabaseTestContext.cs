// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using System.Dynamic;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.Json.Infrastructure;

internal class DatabaseTestContext : DynamicObject
{
    private readonly Dictionary<string, object> values = [];
 
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return values.TryGetValue(binder.Name, out result);
    }
    
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        bool keyExists = values.ContainsKey(binder.Name);

        if (keyExists)
            values[binder.Name] = value;
        else
            values.Add(binder.Name, value);

        return true;
    }
}