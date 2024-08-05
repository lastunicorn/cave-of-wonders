// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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

using System.Globalization;
using System.Net;
using System.Xml;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using HtmlAgilityPack;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

/// <summary>
/// INS = Institutul Național de Statistică
/// </summary>
public class Ins : IIns
{
    private readonly CultureInfo cultureInfo = new("ro-RO");

    public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromFile(string filePath)
    {
        string[] lines = await File.ReadAllLinesAsync(filePath);
        YearlyInflationDocument yearlyInflationDocument = new(lines);

        return yearlyInflationDocument.Records;
    }

    public async Task<IEnumerable<InflationRecordDto>> GetInflationValuesFromWeb()
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


        HttpClient httpClient = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, @"https://insse.ro/cms/ro/content/ipc%E2%80%93serie-de-date-anuala");

        httpRequestMessage.Headers.Add("Host", "insse.ro");
        httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");
        httpRequestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/png,image/svg+xml,*/*;q=0.8");
        httpRequestMessage.Headers.Add("Accept-Language", "en-US,en;q=0.5");
        httpRequestMessage.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        httpRequestMessage.Headers.Add("Connection", "keep-alive");
        //httpRequestMessage.Headers.Add("Cookie", "ADC_CONN_539B3595F4E=F0C552868E448A8C17C451DDC509AC1CE5D509A0D9A013ABC54785A15E85FEAE2FC74830E7741B54; ADC_CONN_539B3595F4E=5F5D2177A4418A8C7397499D5A054EF59390DE29F739B9483F6D3425C5CC9F0B8F67987D11696E35; has_js=1; real-accessability={\"fontSize\":0,\"effect\":null,\"linkHighlight\":false,\"regularFont\":false}; ADC_REQ_2E94AF76E7=291503890AB5A3FF1D40F86F10B01F29EA3C762001711768132D11A9BBFE227ECB26A5E81EC86021");
        httpRequestMessage.Headers.Add("Upgrade-Insecure-Requests", "1");
        httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "document");
        httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
        httpRequestMessage.Headers.Add("Sec-Fetch-Site", "none");
        httpRequestMessage.Headers.Add("Sec-Fetch-User", "?1");
        httpRequestMessage.Headers.Add("Sec-GPC", "1");
        httpRequestMessage.Headers.Add("Priority", "u=0, i");
        httpRequestMessage.Headers.Add("Pragma", "no-cache");
        httpRequestMessage.Headers.Add("Cache-Control", "no-cache");

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return EnumerateInflationRecords(stream);
        }

        return Enumerable.Empty<InflationRecordDto>();
    }

    private IEnumerable<InflationRecordDto> EnumerateInflationRecords(Stream stream)
    {
        HtmlAgilityPack.HtmlDocument htmlDocument = new();
        htmlDocument.Load(stream);

        HtmlNodeCollection trNodes = htmlDocument.DocumentNode.SelectNodes("//article//table/tbody/tr");

        foreach (HtmlNode trNode in trNodes)
        {
            InflationRecordDto inflationRecordDto = GetInflationRecord(trNode);

            if (inflationRecordDto != null)
                yield return inflationRecordDto;
        }






        //XmlTextReader xmlTextReader = new XmlTextReader(stream);

        //xmlTextReader.MoveToContent();

        //while (Read(xmlTextReader))
        //{
        //    if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name == "article")
        //    {
        //    }
        //}


        //XmlDocument xmlDocument = new();
        //xmlDocument.Load(stream);

        //XmlNodeList trNodes = xmlDocument.SelectNodes("//article//table/tbody/tr");

        //foreach (XmlNode trNode in trNodes)
        //{
        //    InflationRecordDto inflationRecordDto = GetInflationRecord(trNode);

        //    if (inflationRecordDto != null)
        //        yield return inflationRecordDto;
        //}

        //return Enumerable.Empty<InflationRecordDto>();
    }

    private static bool Read(XmlTextReader xmlTextReader)
    {
        try
        {
            return xmlTextReader.Read();
        }
        catch
        {
            return true;
        }
    }

    private InflationRecordDto GetInflationRecord(XmlNode trNode)
    {
        XmlNodeList divNodes = trNode.SelectNodes("div");

        if (divNodes.Count == 3)
        {
            string yearAsString = divNodes[0].InnerText;
            string valueAsString = divNodes[1].InnerText;

            int year = int.Parse(yearAsString, cultureInfo);
            decimal value = decimal.Parse(valueAsString, cultureInfo);

            InflationRecordDto inflationRecordDto = new()
            {
                Year = year,
                Value = value
            };

            return inflationRecordDto;
        }

        return null;
    }

    private InflationRecordDto GetInflationRecord(HtmlNode trNode)
    {
        HtmlNodeCollection divNodes = trNode.SelectNodes("td/div");

        if (divNodes.Count == 3)
        {
            string yearAsString = divNodes[0].InnerText;
            string valueAsString = divNodes[1].InnerText;

            int year = int.Parse(yearAsString, cultureInfo);
            decimal value = decimal.Parse(valueAsString, cultureInfo);

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