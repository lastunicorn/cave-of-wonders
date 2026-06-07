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