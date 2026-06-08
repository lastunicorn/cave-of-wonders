using System.Net;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess.InflationWebPage;

internal class InflationWebPageRequest
{
    private readonly string url;

    public InflationWebPageRequest(string url)
    {
        this.url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public async Task<InflationWebPageDocument> Execute()
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        HttpClient httpClient = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, url);

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

        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new Exception($"Failed to retrieve the inflation values from the web. Status code: {httpResponseMessage.StatusCode}");

        Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        return new InflationWebPageDocument(stream);
    }
}
