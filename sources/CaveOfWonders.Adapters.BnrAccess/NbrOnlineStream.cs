namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

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