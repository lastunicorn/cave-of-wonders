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

using System.Xml.Serialization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles;

public class NbrDocument
{
    public NbrDataSet DataSet { get; private set; }

    public static NbrDocument Load(Stream stream)
    {
        XmlSerializer xmlSerializer = new(typeof(NbrDataSet));

        return new NbrDocument
        {
            DataSet = (NbrDataSet)xmlSerializer.Deserialize(stream)
        };
    }
}