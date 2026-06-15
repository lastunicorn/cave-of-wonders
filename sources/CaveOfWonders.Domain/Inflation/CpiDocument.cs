namespace DustInTheWind.CaveOfWonders.Domain.Inflation;

public sealed class CpiDocument : IDisposable
{
    private readonly StreamWriter streamWriter;

    public CpiDocument(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        streamWriter = new StreamWriter(stream);
    }

    public async Task Write(CpiRecordLine cpiRecordLine)
    {
        await cpiRecordLine.Write(streamWriter);
    }

    public async Task Write(IEnumerable<CpiRecordLine> cpiRecordLines)
    {
        foreach (CpiRecordLine cpiRecordLine in cpiRecordLines)
            await cpiRecordLine.Write(streamWriter);
    }

    public void Dispose()
    {
        if (streamWriter != null)
        {
            streamWriter.Flush();
            streamWriter.Dispose();
        }
    }
}
