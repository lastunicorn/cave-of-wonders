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

using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using DustInTheWind.CurrencyExchange.Application.PresentExchangeRate;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Application.PresentToday;

public class PresentTodayUseCase : IRequestHandler<PresentTodayRequest, PresentTodayResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public PresentTodayUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentTodayResponse> Handle(PresentTodayRequest request, CancellationToken cancellationToken)
    {
        DateTime today = systemClock.Today;

        return new PresentTodayResponse
        {
            Date = today,
            ExchangeRates = (await unitOfWork.ExchangeRateRepository.Get(today))
                .Select(x => new ExchangeRateResponseDto(x))
                .ToList()
        };
    }
}