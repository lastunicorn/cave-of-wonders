// Currency Exchange
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

namespace DustInTheWind.CurrencyExchange.BnrAccess;

internal class NbrOnlineStream : Stream
{
    private const string UrlTemplate = "https://bnr.ro/files/xml/years/nbrfxrates{0}.xml";

    private readonly int year;
    private HttpClient httpClient;
    private HttpResponseMessage httpResponseMessage;
    private Stream stream;

    public override bool CanRead => stream.CanRead;

    public override bool CanSeek => stream.CanSeek;

    public override bool CanWrite => stream.CanWrite;

    public override long Length => stream.Length;

    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public NbrOnlineStream(int year)
    {
        this.year = year;
    }

    public async Task Open(CancellationToken cancellationToken)
    {
        httpClient?.Dispose();
        httpResponseMessage?.Dispose();
        stream?.Dispose();

        string url = string.Format(UrlTemplate, year);
        httpClient = new HttpClient();
        httpResponseMessage = await httpClient.GetAsync(url, cancellationToken);

        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new Exception($"Error reading exchange rates from BNR website. URL: {url}");

        stream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
    }

    public override void Flush()
    {
        stream?.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        httpClient?.Dispose();
        httpResponseMessage?.Dispose();
        stream?.Dispose();

        base.Dispose(disposing);
    }
}