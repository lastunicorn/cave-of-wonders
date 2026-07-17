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

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

internal class PresentPotUseCase : IRequestHandler<PresentPotRequest, PresentPotResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IClock clock;

    public PresentPotUseCase(IUnitOfWork unitOfWork, IClock clock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<PresentPotResponse> Handle(PresentPotRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Pot> pots = await RetrievePots(request, cancellationToken);

        PresentPotResponse response = new();

        bool showDetails = request.ShowDetails is true || (!request.ShowDetails.HasValue && request.PotFlexId?.HasValue == true);
        if (showDetails)
        {
            response.PotDetails = pots
                .Select(x => new PotDetails(x))
                .ToList();
        }
        else
        {
            response.PotSummaries = pots
                .Select(x => new PotSummary(x))
                .ToList();
        }

        return response;
    }

    private async Task<IEnumerable<Pot>> RetrievePots(PresentPotRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Pot> pots = await RetrievePotsByIdOrName(request.PotFlexId, cancellationToken)
                .ToListAsync(cancellationToken);

            if (!request.IncludeInactivePots)
            {
                DateOnly today = clock.Today;
                pots = pots.Where(x => x.IsActive(today));
            }

            pots = pots.OrderBy(x => x.DisplayOrder);

            return pots;
        }
        catch (Exception ex)
        {
            throw new StorageInaccessibleException(ex);
        }
    }

    private IAsyncEnumerable<Pot> RetrievePotsByIdOrName(PotFlexId potFlexId, CancellationToken cancellationToken)
    {
        bool isIdentifierSpecified = potFlexId is
        {
            HasValue: true
        };

        return isIdentifierSpecified
            ? unitOfWork.PotRepository.GetAsync(potFlexId, cancellationToken)
            : unitOfWork.PotRepository.GetAllAsync(cancellationToken);
    }
}