using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

internal class GainUseCase : IRequestHandler<GainRequest, GainResponse>
{
	private const string NormalizedCurrency = "EUR";

	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;
	private readonly CurrenciesConvertor currenciesConvertor;

	public GainUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));

		currenciesConvertor = new CurrenciesConvertor(unitOfWork);
	}

	public async Task<GainResponse> Handle(GainRequest request, CancellationToken cancellationToken)
	{
		MonthDate month = DecideMonth(request);

		List<Gem> gains = await unitOfWork.GemRepository
			.FindAsync(new GemFilter
			{
				Month = month,
				IncludeCategories =
				[
					GemCategory.Gain,
					GemCategory.Fee,
					GemCategory.Tax
				]
			}, cancellationToken)
			.ToListAsync(cancellationToken);

		DateOnly conversionDate = systemClock.Today;

		List<GainItem> items = await BuildGainItems(gains, conversionDate, cancellationToken);
		decimal totalGain = items.Sum(x => x.NormalizedGain.Value);

		return new GainResponse
		{
			Date = conversionDate,
			Items = items,
			ConversionRates = currenciesConvertor.UsedExchangeRates
				.Select(x => new ExchangeRateInfo(x))
				.ToList(),
			TotalGain = new DatedAmount
			{
				Value = totalGain,
				Currency = NormalizedCurrency,
				Date = conversionDate
			}
		};
	}

	private async Task<List<GainItem>> BuildGainItems(List<Gem> gains, DateOnly conversionDate, CancellationToken cancellationToken)
	{
		List<GainItem> items = [];

		IEnumerable<IGrouping<Guid, Gem>> gainsByPot = gains
			.Where(x => x.Pot != null)
			.GroupBy(x => x.Pot.Id);

		foreach (IGrouping<Guid, Gem> gainsForPot in gainsByPot)
		{
			string potCurrency = gainsForPot.First().Pot.Currency;
			decimal amount = gainsForPot.Sum(z => z.Category == GemCategory.Gain ? z.Amount : -z.Amount);

			DatedAmount itemValue = new()
			{
				Currency = potCurrency,
				Value = amount,
				Date = conversionDate
			};

			DatedAmount normalizedValue = await currenciesConvertor.Convert(itemValue, NormalizedCurrency, conversionDate, cancellationToken);

			items.Add(new GainItem
			{
				PotName = gainsForPot.First().Pot.Name,
				Currency = potCurrency,
				Gain = itemValue,
				NormalizedGain = normalizedValue,
				IsActual = string.Equals(potCurrency, NormalizedCurrency, StringComparison.OrdinalIgnoreCase)
			});
		}

		return items;
	}

	private MonthDate DecideMonth(GainRequest request)
	{
		return request.Month.HasValue
			? request.Month
			: new MonthDate(systemClock.Today);
	}
}