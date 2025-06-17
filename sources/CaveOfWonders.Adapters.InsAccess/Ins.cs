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

using DustInTheWind.CaveOfWonders.Adapters.InsAccess.InflationWebPage;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

/// <summary>
/// INS = Institutul Național de Statistică
/// </summary>
public class Ins : IIns
{
    private readonly Lazy<InsConfig> insConfig = new(() => new InsConfig());

    public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromFile(string filePath)
    {
        string[] lines = await File.ReadAllLinesAsync(filePath);
        YearlyInflationDocument yearlyInflationDocument = new(lines);

        return yearlyInflationDocument.Records;
    }

    public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromWeb()
    {
        string url = insConfig.Value.InflationPageUrl;

        if (url == null)
            throw new MissingInflationUrlException();

        InflationWebPageRequest webRequest = new(url);

        InflationWebPageDocument inflationWebPageDocument = await webRequest.Execute();
        return inflationWebPageDocument.EnumerateInflationRecords();
    }
}
