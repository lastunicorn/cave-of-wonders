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
        IEnumerable<Pot> pots = await RetrievePot(request);

        PresentPotResponse response = new()
        {
            Pots = pots
                .Select(x => new PotDetails(x))
                .ToList()
        };

        return response;
    }

    private async Task<IEnumerable<Pot>> RetrievePot(PresentPotRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.PotId))
        {
            bool success = Guid.TryParse(request.PotId, out Guid potGuid);

            if (success)
            {
                Pot pot = await unitOfWork.PotRepository.GetById(potGuid);
                return new[] { pot };
            }

            return await unitOfWork.PotRepository.GetByPartialId(request.PotId);
        }

        if (request.PotName != null)
            return await unitOfWork.PotRepository.GetByName(request.PotName);

        bool isIdentifierSpecified = !string.IsNullOrWhiteSpace(request.PotIdentifier);

        if (isIdentifierSpecified)
        {
            IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetByIdOrName(request.PotIdentifier);

            if (!request.IncludeInactivePots)
            {
                DateTime today = systemClock.Today;

                pots = pots.Where(x => x.IsActive(today));
            }

            pots = pots.OrderBy(x => x.DisplayOrder);

            return pots;

            //bool success = Guid.TryParse(request.PotIdentifier, out Guid potGuid);

            //if (success)
            //{
            //    Pot pot = await unitOfWork.PotRepository.GetById(potGuid);
            //    return new[] { pot };
            //}

            //List<Pot> pots = (await unitOfWork.PotRepository.GetByPartialId(request.PotIdentifier))
            //    .ToList();

            //if (pots.Count == 0)
            //{
            //    pots = (await unitOfWork.PotRepository.GetByName(request.PotIdentifier))
            //        .ToList();
            //}

            //return pots;
        }

        return await unitOfWork.PotRepository.GetAll();
    }

    //private async Task<IEnumerable<Pot>> RetrievePot(PresentPotRequest request)
    //{
    //    if (!string.IsNullOrWhiteSpace(request.PotId))
    //    {
    //        bool success = Guid.TryParse(request.PotId, out Guid potGuid);

    //        if (success)
    //        {
    //            Pot pot = await unitOfWork.PotRepository.GetById(potGuid);
    //            return new[] { pot };
    //        }

    //        return await unitOfWork.PotRepository.GetByPartialId(request.PotId);
    //    }

    //    if (request.PotName != null)
    //        return await unitOfWork.PotRepository.GetByName(request.PotName);

    //    return await unitOfWork.PotRepository.GetAll();
    //}
}