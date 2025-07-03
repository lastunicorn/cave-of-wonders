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

public class BcrSheetDescriptor : ISheetDescriptor
{
    public string Name => "BCR";

    public ColumnDescriptor[] ColumnDescriptors { get; } =
    {
        new()
        {
            Index = 2,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("e1e5eb84-5588-4521-9827-7818a265ba83")
        },
        new()
        {
            Index = 3,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("aeb2e61c-8aa3-4cae-9f99-bd1183932994")
        },
        new()
        {
            Index = 4,
            DateIndex = 0,
            Format = ValueFormat.Lei,
            Key = new Guid("8f938853-de17-4c07-a582-cf40bb7afd25")
        },
        new()
        {
            Index = 5,
            DateIndex = 0,
            Format = ValueFormat.Euro,
            Key = new Guid("ff463ade-ec2c-4c1e-b00c-9eb47c96914b")
        }
    };
}
