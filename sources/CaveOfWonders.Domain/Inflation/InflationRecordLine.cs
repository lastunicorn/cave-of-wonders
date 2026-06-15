namespace DustInTheWind.CaveOfWonders.Domain.Inflation;

public class InflationRecordLine
{
    private readonly int year;
    private readonly decimal value;

    public InflationRecordLine(int year, decimal value)
    {
        this.year = year;
        this.value = value;
    }

    public async Task Write(StreamWriter streamWriter)
    {
        await streamWriter.WriteAsync($"{year}: {value,6:N2} ");

        if (value > 0)
        {
            int roundedValue = (int)Math.Round(Math.Max(0, value));

            string chartLine = new('.', roundedValue);
            await streamWriter.WriteAsync(chartLine);
        }

        await streamWriter.WriteLineAsync();
    }
}