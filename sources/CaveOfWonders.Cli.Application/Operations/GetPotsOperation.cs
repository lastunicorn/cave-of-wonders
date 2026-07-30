using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.ClockAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;

namespace DustInTheWind.CaveOfWonders.Cli.Application.Operations;

internal class GetPotsOperation : IOperation<IAsyncEnumerable<Pot>>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISystemClock systemClock;

	public PotFlexId PotId { get; set; }

	public bool IncludeInactive { get; set; }

	public DateOnly? Today { get; set; }

	public GetPotsOperation(IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	/// <summary>
	/// Builds the enumeration of pots. The pots are actually retrieved from the storage later, when the
	/// returned enumeration is iterated.
	/// </summary>
	public Task<IAsyncEnumerable<Pot>> ExecuteAsync(CancellationToken cancellationToken = default)
	{
		bool isIdentifierSpecified = PotId?.HasValue == true;

		IAsyncEnumerable<Pot> pots = isIdentifierSpecified
			? unitOfWork.PotRepository.GetAsync(PotId, cancellationToken)
			: unitOfWork.PotRepository.GetAllAsync(cancellationToken);

		if (!IncludeInactive)
		{
			DateOnly today = Today ?? systemClock.Today;
			pots = pots.Where(x => x.IsActive(today));
		}

		pots = pots.OrderBy(x => x.DisplayOrder);

		return Task.FromResult(pots);
	}
}