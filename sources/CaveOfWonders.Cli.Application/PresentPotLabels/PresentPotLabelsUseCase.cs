using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

internal class PresentPotLabelsUseCase : IRequestHandler<PresentPotLabelsRequest, PresentPotLabelsResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public PresentPotLabelsUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<PresentPotLabelsResponse> Handle(PresentPotLabelsRequest request, CancellationToken cancellationToken)
	{
		List<Pot> pots = await RetrievePots(request.PotFlexId, cancellationToken);

		return new PresentPotLabelsResponse
		{
			Items = pots
				.Select(x => new PotLabelsItem
				{
					PotId = x.Id,
					PotName = x.Name,
					Labels = x.Labels
						.Select(l => l.Label)
						.ToList()
				})
				.ToList()
		};
	}

	private async Task<List<Pot>> RetrievePots(PotFlexId potFlexId, CancellationToken cancellationToken)
	{
		try
		{
			bool isIdentifierSpecified = potFlexId?.HasValue == true;

			IAsyncEnumerable<Pot> pots = isIdentifierSpecified
				? unitOfWork.PotRepository.GetAsync(potFlexId, cancellationToken)
				: unitOfWork.PotRepository.GetAllAsync(cancellationToken);

			return await pots
				.OrderBy(x => x.DisplayOrder)
				.ToListAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			throw new DataStorageException(ex);
		}
	}
}
