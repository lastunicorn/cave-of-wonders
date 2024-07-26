// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CsvParser.Application;

public class ImportReport : IEnumerable<PotImportReport>
{
    private readonly List<PotImportReport> potImportReports = new();

    public PotImportReport this[Pot pot]
    {
        get
        {
            PotImportReport potImportReport = potImportReports
                .FirstOrDefault(x => x.Pot == pot);

            if (potImportReport == null)
            {
                potImportReport = new PotImportReport
                {
                    Pot = pot
                };

                potImportReports.Add(potImportReport);
            }

            return potImportReport;
        }
    }

    public IEnumerator<PotImportReport> GetEnumerator()
    {
        return potImportReports.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}