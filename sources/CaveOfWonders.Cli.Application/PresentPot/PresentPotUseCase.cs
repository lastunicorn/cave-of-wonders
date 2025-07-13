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
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

internal class PresentPotUseCase : IRequestHandler<PresentPotRequest, PresentPotResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public PresentPotUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentPotResponse> Handle(PresentPotRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PotIdentifier))
            throw new PotIdentifierNotSpecifiedException();

        IEnumerable<Pot> pots = await RetrievePots(request);

        PresentPotResponse response = new()
        {
            Pots = pots
                .Select(x => new PotDetails(x))
                .ToList()
        };

        return response;
    }

    private async Task<IEnumerable<Pot>> RetrievePots(PresentPotRequest request)
    {
        try
        {
            IEnumerable<Pot> pots = await RetrievePotsByIdOrName(request.PotIdentifier);

            if (!request.IncludeInactivePots)
            {
                DateTime today = systemClock.Today;
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

    private async Task<IEnumerable<Pot>> RetrievePotsByIdOrName(string potIdentifier)
    {
        bool isIdentifierSpecified = !string.IsNullOrWhiteSpace(potIdentifier);

        return isIdentifierSpecified
            ? await unitOfWork.PotRepository.GetByIdOrName(potIdentifier)
            : await unitOfWork.PotRepository.GetAll();
    }
}