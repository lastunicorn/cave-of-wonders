using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.OperationEngine;

namespace DustInTheWind.CaveOfWonders.Cli.Application.Operations;

internal class GetOnePotOperation : IOperation<Pot>
{
	private readonly IUnitOfWork unitOfWork;

	public PotFlexId PotId { get; set; }

	/// <summary>
	/// Gets or sets a value specifying if a <see cref="PotNotFoundException"/> is thrown when no pot matches
	/// the <see cref="PotId"/>. When set to <c>false</c>, <c>null</c> is returned instead.
	/// Default value is <c>true</c>.
	/// </summary>
	public bool ThrowIfNotFound { get; set; } = true;

	public GetOnePotOperation(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<Pot> ExecuteAsync(CancellationToken cancellationToken = default)
	{
		IAsyncEnumerable<Pot> pots = unitOfWork.PotRepository.GetAsync(PotId, cancellationToken);

		Pot matchedPot = null;

		await foreach (Pot pot in pots)
		{
			if (matchedPot != null)
				throw new MultiplePotsException(PotId);

			if (pot != null)
				matchedPot = pot;
		}

		if (matchedPot == null && ThrowIfNotFound)
			throw new PotNotFoundException(PotId);

		return matchedPot;
	}
}