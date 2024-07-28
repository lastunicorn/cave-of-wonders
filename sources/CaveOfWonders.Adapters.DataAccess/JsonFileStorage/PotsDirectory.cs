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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.JsonFileStorage;

internal class PotsDirectory
{
    private readonly string rootDirectoryPath;
    private const string DirectoryName = "pots";

    public PotsDirectory(string rootDirectoryPath)
    {
        this.rootDirectoryPath = rootDirectoryPath ?? throw new ArgumentNullException(nameof(rootDirectoryPath));
    }

    public IEnumerable<PotFile> EnumeratePotFiles()
    {
        string directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);

        return Directory.GetFiles(directoryPath)
            .Select(x => new PotFile(x));
    }

    public PotFile GetPotFile(Guid potId)
    {
        string directoryPath = Path.Combine(rootDirectoryPath, DirectoryName);

        string potFileName = $"{potId:D}.json";
        string potFilePath = Path.Combine(directoryPath, potFileName);

        return new PotFile(potFilePath);
    }
}