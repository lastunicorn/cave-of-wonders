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

using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using HtmlAgilityPack;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess.InflationWebPage;

internal class InflationWebPageDocument
{
    private readonly CultureInfo cultureInfo = new("ro-RO");
    private readonly Stream stream;

    public InflationWebPageDocument(Stream stream)
    {
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    public IEnumerable<InflationRecordDto> EnumerateInflationRecords()
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.Load(stream);

        HtmlNodeCollection trNodes = htmlDocument.DocumentNode.SelectNodes("//article//table/tbody/tr");

        foreach (HtmlNode trNode in trNodes)
        {
            InflationRecordDto inflationRecordDto = GetInflationRecord(trNode);

            if (inflationRecordDto != null)
                yield return inflationRecordDto;
        }
    }

    private InflationRecordDto GetInflationRecord(HtmlNode trNode)
    {
        HtmlNodeCollection divNodes = trNode.SelectNodes("td/div");

        if (divNodes.Count == 3)
        {
            string yearAsString = divNodes[0].InnerText;
            string valueAsString = divNodes[1].InnerText;

            int year = int.Parse(yearAsString, cultureInfo);
            decimal value = decimal.Parse(valueAsString, cultureInfo) - 100;

            InflationRecordDto inflationRecordDto = new()
            {
                Year = year,
                Value = value
            };

            return inflationRecordDto;
        }

        return null;
    }
}