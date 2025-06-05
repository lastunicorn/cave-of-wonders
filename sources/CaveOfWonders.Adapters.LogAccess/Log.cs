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

using DustInTheWind.CaveOfWonders.Ports.LogAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.LogAccess;

public sealed class Log : ILog, IDisposable, IAsyncDisposable
{
    private readonly StreamWriter streamWriter;

    public Log()
    {
        string fileName = DateTime.Today.ToString("yyyy-MM-dd") + ".log";
        string logsDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Logs");

        if (!Directory.Exists(logsDirectoryPath))
            Directory.CreateDirectory(logsDirectoryPath);

        string filePath = Path.Combine(logsDirectoryPath, fileName);

        FileStreamOptions fileStreamOptions = new()
        {
            Mode = FileMode.Append,
            Access = FileAccess.Write
        };
        streamWriter = new StreamWriter(filePath, fileStreamOptions);
    }

    public void WriteInfo(string text)
    {
        DateTime now = DateTime.Now;
        string message = $"[{now:yyyy-MM-dd HH:mm:ss.fff}] {text}";

        streamWriter.WriteLine(message);
    }

    public void Dispose()
    {
        streamWriter?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (streamWriter != null)
            await streamWriter.DisposeAsync();
    }
}