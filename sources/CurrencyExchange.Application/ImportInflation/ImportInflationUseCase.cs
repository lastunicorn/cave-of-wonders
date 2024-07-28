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

using DustInTheWind.CurrencyExchange.Ports.InsAccess;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Application.ImportInflation;

internal class ImportInflationUseCase : IRequestHandler<ImportInflationRequest, ImportInflationResponse>
{
    private readonly IIns ins;

    public ImportInflationUseCase(IIns ins)
    {
        this.ins = ins ?? throw new ArgumentNullException(nameof(ins));
    }

    public async Task<ImportInflationResponse> Handle(ImportInflationRequest request, CancellationToken cancellationToken)
    {
        List<InflationRecord> inflationRecords = (await ins.GetInflationValuesFrom(request.SourceFilePath))
            .Select(x => new InflationRecord(x))
            .ToList();

        return new ImportInflationResponse
        {
            Records = inflationRecords
        };
    }
}