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

using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

internal static class ColumnDescriptorExtensions
{
    public static ColumnDescriptor ToColumnDescriptor(this JColumnDescriptor jColumnDescriptor)
    {
        return new ColumnDescriptor
        {
            Index = jColumnDescriptor.Index,
            DateIndex = jColumnDescriptor.DateIndex,
            Key = jColumnDescriptor.PotId
        };
    }

    public static SheetDescriptor ToSheetDescriptor(this JSheetDescriptor jSheetDescriptor)
    {
        return new SheetDescriptor
        {
            Name = jSheetDescriptor.Name,
            ColumnDescriptors = jSheetDescriptor.Columns?
                .Select(x => x.ToColumnDescriptor())
                .ToList()
        };
    }
}