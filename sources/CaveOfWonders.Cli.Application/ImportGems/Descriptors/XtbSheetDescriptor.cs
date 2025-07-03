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

internal class XtbSheetDescriptor : ISheetDescriptor
{
    public ColumnDescriptor[] ColumnDescriptors { get; } =
    {
        new()
        {
            Index = 3,
            DateIndex = 0,
            Format = ValueFormat.Euro,
            Key = new Guid("f39128a9-775b-4a9e-9196-5517d1f4b8a6")
        },
    };
}