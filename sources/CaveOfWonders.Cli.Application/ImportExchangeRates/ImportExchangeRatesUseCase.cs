using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

internal class ImportExchangeRatesUseCase : IUseCase<ImportExchangeRatesRequest, ImportExchangeRatesResponse>
{
	private readonly IBnrService bnrService;
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public ImportExchangeRatesUseCase(IBnrService bnrService, IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.bnrService = bnrService ?? throw new ArgumentNullException(nameof(bnrService));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async Task<ImportExchangeRatesResponse> Execute(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<ExchangeRate> exchangeRates = await GetExchangeRatesFromSource(request, cancellationToken);

		ImportOperation importOperation = new(unitOfWork.ExchangeRateRepository);
		ExchangeRateImportReport report = await importOperation.Execute(exchangeRates, cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return new ImportExchangeRatesResponse(report);
	}

	private async Task<IEnumerable<ExchangeRate>> GetExchangeRatesFromSource(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<BnrExchangeRate> bnrExchangeRates = request.ImportSource switch
		{
			ImportSource.BnrWebsite => await ImportFromWebNbrFile(request, cancellationToken),
			ImportSource.BnrNbrFile => await ImportFromLocalNbrFile(request, cancellationToken),
			_ => throw new ArgumentOutOfRangeException()
		};

		return bnrExchangeRates.ToExchangeRates();
	}

	private async Task<IEnumerable<BnrExchangeRate>> ImportFromWebNbrFile(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
	{
		int year = request.Year ?? systemClock.Today.Year;

		try
		{
			return await bnrService.GetExchangeRatesFromOnline(year, cancellationToken);
		}
		catch (Exception ex)
		{
			throw new BnrWebsiteAccessException(year, ex);
		}
	}

	private async Task<IEnumerable<BnrExchangeRate>> ImportFromLocalNbrFile(ImportExchangeRatesRequest request, CancellationToken cancellationToken)
	{
		try
		{
			return await bnrService.GetExchangeRatesFromFile(request.SourceFilePath, cancellationToken);
		}
		catch (Exception ex)
		{
			throw new ImportFileAccessException(request.SourceFilePath, ex);
		}
	}
}