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
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

internal class ImportExchangeRatesUseCase : IRequestHandler<ImportExchangeRatesRequest, ImportExchangeRatesResponse>
{
    private readonly IBnr bnr;
    private readonly IUnitOfWork unitOfWork;
    private readonly ISystemClock systemClock;

    public ImportExchangeRatesUseCase(IBnr bnr, IUnitOfWork unitOfWork, ISystemClock systemClock)
    {
        this.bnr = bnr ?? throw new ArgumentNullException(nameof(bnr));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<ImportExchangeRatesResponse> Handle(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<ExchangeRate> exchangeRates = (await GetExchangeRates(request, cancellationToken))
            .ToExchangeRates();

        ImportReport report = await unitOfWork.ExchangeRateRepository.Import(exchangeRates, cancellationToken);

        await unitOfWork.SaveChanges();

        return new ImportExchangeRatesResponse(report);
    }

    private async Task<IEnumerable<BnrExchangeRate>> GetExchangeRates(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        return request.ImportSource switch
        {
            ImportSource.BnrWebsite => await ImportFromWebNbrFile(request, cancellationToken),
            ImportSource.BnrFile => await ImportFromLocalBnrFile(request, cancellationToken),
            ImportSource.BnrNbrFile => await ImportFromLocalNbrFile(request, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<IEnumerable<BnrExchangeRate>> ImportFromWebNbrFile(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        int year = request.Year ?? systemClock.Today.Year;

        try
        {
            return await bnr.GetExchangeRatesFromNbrOnline(year, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BnrWebsiteAccessException(year, ex);
        }
    }

    private async Task<IEnumerable<BnrExchangeRate>> ImportFromLocalBnrFile(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await bnr.GetExchangeRatesFrom(request.SourceFilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ImportFileAccessException(request.SourceFilePath, ex);
        }
    }

    private async Task<IEnumerable<BnrExchangeRate>> ImportFromLocalNbrFile(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await bnr.GetExchangeRatesFromNbr(request.SourceFilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ImportFileAccessException(request.SourceFilePath, ex);
        }
    }
}