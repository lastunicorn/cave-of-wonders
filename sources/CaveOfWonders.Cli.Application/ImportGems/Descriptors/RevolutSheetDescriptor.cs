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

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems.Descriptors;

internal class RevolutSheetDescriptor : ISheetDescriptor
{
    public string Name => "Revolut";

    public ColumnDescriptor[] ColumnDescriptors { get; } =
    {
        new()
        {
            Index = 2,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("596569be-1a07-40eb-b86b-054738e0a4c3")
        },
        new()
        {
            Index = 3,
            DateIndex = 0,
            Format = ValueFormat.Euro,
            Key = new Guid("50e27d7e-9175-4144-a852-a317c3c3a4e3")
        },
        new()
        {
            Index = 5,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("5eca3af2-3fee-4636-80aa-626cf7bb7bb0")
        }
    };
}
