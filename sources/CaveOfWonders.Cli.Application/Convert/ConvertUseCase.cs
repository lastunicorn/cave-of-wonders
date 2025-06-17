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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.Convert;

internal class ConvertUseCase : IRequestHandler<ConvertRequest, ConvertResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public ConvertUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<ConvertResponse> Handle(ConvertRequest request, CancellationToken cancellationToken)
    {
        DateTime dateOfExchangeRate = request.Date ?? systemClock.Today;
        ExchangeRate exchangeRate = await RetrieveExchangeRate(request.CurrencyPair, dateOfExchangeRate);

        return new ConvertResponse
        {
            InitialValue = request.InitialValue,
            ConvertedValue = exchangeRate.Convert(request.InitialValue),
            ExchangeRate = new ExchangeRateInfo(exchangeRate),
            IsDateCurrent = exchangeRate.Date == dateOfExchangeRate
        };
    }

    private async Task<ExchangeRate> RetrieveExchangeRate(CurrencyPair currencyPair, DateTime date)
    {
        ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository.GetForLatestDayAvailable(currencyPair, date, true);

        if (exchangeRate == null)
            throw new ExchangeRateNotFoundException(currencyPair, date);

        ConversionAbility conversionAbility = exchangeRate.AnalyzeConversionAbility(currencyPair.Currency1, currencyPair.Currency2);

        return conversionAbility switch
        {
            ConversionAbility.None => throw new ExchangeRateUnusableException(exchangeRate),
            ConversionAbility.ConvertDirect => exchangeRate,
            ConversionAbility.ConvertReverse => exchangeRate.Invert(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}