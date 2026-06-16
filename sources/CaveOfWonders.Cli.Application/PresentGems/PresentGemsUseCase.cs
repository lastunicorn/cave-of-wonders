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
using DustInTheWind.CaveOfWonders.Infrastructure;
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
        IAsyncEnumerable<Pot> potEnumeration = unitOfWork.PotRepository.GetByPartialIdAsync(request.PotId, cancellationToken);
        Pot pot = await potEnumeration.SingleAsync();

        IAsyncEnumerable<Gem> gems = request.Date.HasValue
            ? unitOfWork.GemRepository.FindByDateAsync(pot.Id, request.Date.Value, cancellationToken)
            : unitOfWork.GemRepository.GetByPotIdAsync(pot.Id, cancellationToken);

        return new PresentGemsResponse
        {
            Gems = await gems
                .Select(x => new GemDto
                {
                    Date = x.Date,
                    Category = x.Category,
                    Amount = x.Amount
                })
                .ToListAsync()
        };
    }
}