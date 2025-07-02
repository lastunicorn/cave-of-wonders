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

internal class IngSheetDescriptor : ISheetDescriptor
{
    public ColumnDescriptor[] ColumnDescriptors { get; } =
    {
        new()
        {
            Index = 2,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("0d74863c-5e66-4b92-9c90-29e313807beb")
        },
        new()
        {
            Index = 3,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("be127619-7f87-47f6-ab6d-87598c721121")
        },
        new()
        {
            Index = 4,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("076ae4d3-a919-48ec-94f2-d1c295e5533f")
        },
        new()
        {
            Index = 5,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("03d7ac58-1bb5-447d-ae28-43a3e1120e69")
        }
    };
}