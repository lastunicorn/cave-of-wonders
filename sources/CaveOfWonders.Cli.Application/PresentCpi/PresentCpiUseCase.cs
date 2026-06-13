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
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;

internal class PresentCpiUseCase : IRequestHandler<PresentCpiRequest, PresentCpiResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public PresentCpiUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PresentCpiResponse> Handle(PresentCpiRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Cpi> inflationRecords = await RetrieveInflationRecordsFromStorage();

        return new PresentCpiResponse
        {
            InflationRecords = inflationRecords
                .Select(x => new CpiDto(x))
                .ToList()
        };
    }

    private async Task<IEnumerable<Cpi>> RetrieveInflationRecordsFromStorage()
    {
        IEnumerable<Cpi> inflationRecords = await unitOfWork.CpiRepository.GetAll();

        return inflationRecords
            .OrderBy(x => x.Year);
    }
}
