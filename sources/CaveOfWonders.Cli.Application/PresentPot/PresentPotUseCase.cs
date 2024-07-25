// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

internal class PresentPotUseCase : IRequestHandler<PresentPotRequest, PresentPotResponse>
{
    private readonly IPotRepository potRepository;

    public PresentPotUseCase(IPotRepository potRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
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
                Pot pot = await potRepository.GetById(potGuid);
                return new[] { pot };
            }

            return await potRepository.GetByPartialId(request.PotId);
        }

        if (request.PotName != null)
            return await potRepository.Get(request.PotName);

        return await potRepository.GetAll();
    }
}