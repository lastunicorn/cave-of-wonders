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
using DustInTheWind.Ins.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

public class WebCpiImportExport : ICpiImportExport
{
    private readonly Lazy<InsConfig> insConfig = new(() => new InsConfig());

    public Guid Id => new Guid("3ff33b30-a149-4f08-b545-e524fd3e4384");

    public string Name => "CPI Web Import/Export";

    public bool CanImport => true;

    public bool CanExport => false;

    public async IAsyncEnumerable<CpiRecordDto> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Uri url = insConfig.Value.CpiPageUrl;

        if (url == null)
            throw new MissingCpiUrlException();

        YearlyCpiWebPage webPage = new(url);
        IEnumerable<YearlyCpiRecord> yearlyCpiRecords = await webPage.EnumerateRecords();

        foreach (YearlyCpiRecord yearlyCpiRecord in yearlyCpiRecords)
        {
            yield return new CpiRecordDto
            {
                Year = yearlyCpiRecord.Year,
                Value = yearlyCpiRecord.Value
            };
        }
    }

    public Task ExportAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}