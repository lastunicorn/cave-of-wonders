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
