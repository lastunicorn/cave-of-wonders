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
using DustInTheWind.CurrencyExchange.Application.ImportFromNbrFile;
using DustInTheWind.CurrencyExchange.Ports.BnrAccess;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Application.ImportFromNbrWebsite;

public class ImportFromNbrWebsiteUseCase : IRequestHandler<ImportFromNbrWebsiteRequest, ImportFromNbrWebsiteResponse>
{
    private readonly IBnr bnr;
    private readonly IUnitOfWork unitOfWork;

    public ImportFromNbrWebsiteUseCase(IBnr bnr, IUnitOfWork unitOfWork)
    {
        this.bnr = bnr ?? throw new ArgumentNullException(nameof(bnr));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ImportFromNbrWebsiteResponse> Handle(ImportFromNbrWebsiteRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<ExchangeRate> exchangeRates = (await bnr.GetExchangeRatesFromNbrOnline(request.Year, cancellationToken))
            .ToExchangeRates();

        ImportReport report = await unitOfWork.ExchangeRateRepository.Import(exchangeRates);

        await unitOfWork.SaveChanges();

        return new ImportFromNbrWebsiteResponse(report);
    }
}