namespace DustInTheWind.CaveOfWonders.Domain.Inflation;

public sealed class InflationDocument : IDisposable
{
    private readonly StreamWriter streamWriter;

    public InflationDocument(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        streamWriter = new StreamWriter(stream);
    }

    public async Task Write(InflationRecordLine inflationRecordLine)
    {
        await inflationRecordLine.Write(streamWriter);
    }

    public async Task Write(IEnumerable<InflationRecordLine> inflationRecordLines)
    {
        foreach (InflationRecordLine inflationRecordLine in inflationRecordLines)
            await inflationRecordLine.Write(streamWriter);
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
