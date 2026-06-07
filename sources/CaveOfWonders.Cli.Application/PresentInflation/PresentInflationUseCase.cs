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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;

internal class PresentInflationUseCase : IRequestHandler<PresentInflationRequest, PresentInflationResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public PresentInflationUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PresentInflationResponse> Handle(PresentInflationRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<InflationRecord> inflationRecords = await RetrieveInflationRecordsFromStorage();

        return new PresentInflationResponse
        {
            InflationRecords = inflationRecords
                .Select(x => new InflationRecordDto(x))
                .ToList()
        };
    }

    private async Task<IEnumerable<InflationRecord>> RetrieveInflationRecordsFromStorage()
    {
        IEnumerable<InflationRecord> inflationRecords = await unitOfWork.InflationRecordRepository.GetAll();

        return inflationRecords
            .OrderBy(x => x.Year);
    }
}
