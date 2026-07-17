using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;

internal class GainUseCase : IRequestHandler<GainRequest, GainResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public GainUseCase(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
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

		return new GainResponse
		{
			Items = items
		};
	}

	private MonthDate DecideMonth(GainRequest request)
	{
		return request.Month.HasValue
			? request.Month
			: new MonthDate(systemClock.Today);
	}
}