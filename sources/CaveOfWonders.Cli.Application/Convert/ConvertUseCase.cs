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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.Convert;

internal class ConvertUseCase : IRequestHandler<ConvertRequest, ConvertResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;
    private ConvertResponse response;

    public ConvertUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<ConvertResponse> Handle(ConvertRequest request, CancellationToken cancellationToken)
    {
        response = new ConvertResponse();

        ExchangeRate exchangeRate = await RetrieveExchangeRate(request);
        Convert(request, exchangeRate);

        return response;
    }

    private async Task<ExchangeRate> RetrieveExchangeRate(ConvertRequest request)
    {
        CurrencyPair currencyPair = new()
        {
            Currency1 = request.SourceCurrency,
            Currency2 = request.DestinationCurrency
        };

        DateTime date = request.Date ?? systemClock.Today;

        ExchangeRate exchangeRate = await unitOfWork.ExchangeRateRepository.GetLatest(currencyPair, date, true);

        if (exchangeRate == null)
            throw new Exception($"There is no exchange rate for the specific currency pair: {request.SourceCurrency} {request.DestinationCurrency}");

        response.IsDateCurrent = exchangeRate.Date == date;

        return exchangeRate;
    }

    private void Convert(ConvertRequest request, ExchangeRate exchangeRate)
    {
        ConversionAbility conversionAbility = exchangeRate.AnalyzeConversionAbility(request.SourceCurrency, request.DestinationCurrency);

        switch (conversionAbility)
        {
            case ConversionAbility.None:
                throw new Exception($"An exchange rate was found in the database, but could not be used for conversion. The value could not be converted. Exchange rate: {exchangeRate}");

            case ConversionAbility.ConvertDirect:
                break;

            case ConversionAbility.ConvertReverse:
                exchangeRate = exchangeRate.Invert();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        response.SourceCurrency = exchangeRate.CurrencyPair.Currency1;
        response.DestinationCurrency = exchangeRate.CurrencyPair.Currency2;
        response.InitialValue = request.InitialValue;
        response.ConvertedValue = exchangeRate.Convert(request.InitialValue);
        response.ExchangeRate = new ExchangeRateInfo(exchangeRate);
    }
}