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
using DustInTheWind.CurrencyExchange.Ports.BnrAccess;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Application.ImportFromNbrFile;

internal class ImportFromNbrUseCase : IRequestHandler<ImportFromNbrRequest, ImportFromNbrResponse>
{
    private readonly IBnr bnr;
    private readonly IExchangeRateRepository exchangeRateRepository;

    public ImportFromNbrUseCase(IBnr bnr, IExchangeRateRepository exchangeRateRepository)
    {
        this.bnr = bnr ?? throw new ArgumentNullException(nameof(bnr));
        this.exchangeRateRepository = exchangeRateRepository ?? throw new ArgumentNullException(nameof(exchangeRateRepository));
    }

    public async Task<ImportFromNbrResponse> Handle(ImportFromNbrRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<ExchangeRate> exchangeRates = (await bnr.GetExchangeRatesFromNbr(request.SourceFilePath))
            .ToExchangeRates();

        ImportReport report = await exchangeRateRepository.Import(exchangeRates);

        return new ImportFromNbrResponse(report);
    }
}