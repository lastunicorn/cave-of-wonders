using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

internal class GainUseCase : IRequestHandler<GainRequest, GainResponse>
{
	private const string TotalGainCurrency = "EUR";

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
				Categories =
				[
					GemCategory.Gain,
					GemCategory.Fee,
					GemCategory.Tax
				]
			}, cancellationToken)
			.ToListAsync(cancellationToken);

		List<GainItem> items = gains
			.Where(x => x.Pot != null)
			.GroupBy(x => x.Pot.Id)
			.Select(x => new GainItem
			{
				PotName = x.First().Pot.Name,
				Currency = x.First().Pot.Currency,
				TotalGain = x.Sum(z => z.Category == GemCategory.Gain ? z.Amount : -z.Amount)
			})
			.ToList();

		decimal totalGain = await CalculateTotalGainInEur(items, month, cancellationToken);

		return new GainResponse
		{
			Items = items,
			TotalGain = totalGain
		};
	}

	private async Task<decimal> CalculateTotalGainInEur(List<GainItem> items, MonthDate month, CancellationToken cancellationToken)
	{
		DateOnly conversionDate = LastDayOf(month);
		decimal totalGain = 0;

		foreach (GainItem item in items)
		{
			CurrencyValue itemValue = new()
			{
				Currency = item.Currency,
				Value = item.TotalGain,
				Date = conversionDate
			};

			CurrencyValue normalizedValue = await currenciesConvertor.Convert(itemValue, TotalGainCurrency, conversionDate, cancellationToken);
			totalGain += normalizedValue.Value;
		}

		return totalGain;
	}

	private MonthDate DecideMonth(GainRequest request)
	{
		return request.Month.HasValue
			? request.Month
			: new MonthDate(systemClock.Today);
	}

	private static DateOnly LastDayOf(MonthDate month)
	{
		int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
		return new DateOnly(month.Year, month.Month, daysInMonth);
	}
}