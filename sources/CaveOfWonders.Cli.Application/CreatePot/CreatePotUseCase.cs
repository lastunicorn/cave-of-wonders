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

namespace DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;

public class CreatePotUseCase : IRequestHandler<CreatePotRequest, CreatePotResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public CreatePotUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<CreatePotResponse> Handle(CreatePotRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new PotNameNotSpecifiedException();

        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new PotCurrencyNotSpecifiedException();

        DateTime startDate = request.StartDate ?? systemClock.Today;
        
        Pot pot = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartDate = startDate,
            Currency = request.Currency
        };

        try
        {
            await unitOfWork.PotRepository.Add(pot);
            await unitOfWork.SaveChanges();
            
            return new CreatePotResponse
            {
                PotId = pot.Id,
                Name = pot.Name,
                Description = pot.Description,
                StartDate = pot.StartDate,
                Currency = pot.Currency
            };
        }
        catch (Exception ex)
        {
            throw new StorageInaccessibleException(ex);
        }
    }
}