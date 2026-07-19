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

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

internal class PresentGemsUseCase : IRequestHandler<PresentGemsRequest, PresentGemsResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public PresentGemsUseCase(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<PresentGemsResponse> Handle(PresentGemsRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotId, cancellationToken);
        IAsyncEnumerable<Gem> gems = RetrieveGems(pot.Id, request, cancellationToken);

        return new PresentGemsResponse
        {
            Gems = await gems
                .Select(x => new GemDto
                {
                    Date = x.Date,
                    Category = x.Category,
                    Amount = x.Amount
                })
                .ToListAsync(cancellationToken)
        };
    }

    private async Task<Pot> RetrievePot(PotFlexId potId, CancellationToken cancellationToken)
    {
	    IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAsync(potId, cancellationToken);

	    Pot matchedPot = null;
	    
	    await foreach(Pot pot in pots)
	    {
		    if(matchedPot != null)
			    throw new MultiplePotsException(potId);
		    
		    if (pot != null)
			    matchedPot = pot;
	    }
	    
	    if (matchedPot == null)
		    throw new NoPotException(potId);

	    return matchedPot;
    }

    private IAsyncEnumerable<Gem> RetrieveGems(Guid potId, PresentGemsRequest request, CancellationToken cancellationToken)
    {
        if (request.Date.HasValue)
            return unitOfWork.GemRepository.FindByDateAsync(potId, request.Date.Value, cancellationToken);

        if (request.Month.HasValue)
            return unitOfWork.GemRepository.FindByMonthAsync(potId, request.Month, cancellationToken);

        return unitOfWork.GemRepository.GetByPotIdAsync(potId, cancellationToken);
    }
}