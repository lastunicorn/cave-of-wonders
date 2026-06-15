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

using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

public class FileCpiImportExport : ICpiImportExport
{
	private readonly string filePath;

	public FileCpiImportExport(string filePath)
	{
		this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
	}

    public Guid Id => new Guid("bb7590ef-6126-4529-8012-b6a8a4c6f903");

    public string Name => "CPI File Import/Export";

    public bool CanImport => true;

    public bool CanExport => false;

    public async IAsyncEnumerable<CpiRecordDto> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		IEnumerable<string> lines = await File.ReadLinesAsync(filePath, cancellationToken)
            .ToListAsync(cancellationToken);

		CpiRecordDtoEnumerator enumerator = new(lines);

		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}

    public Task ExportAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}