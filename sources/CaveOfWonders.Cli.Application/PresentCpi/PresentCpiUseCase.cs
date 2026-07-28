using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;

internal class PresentCpiUseCase : IUseCase<PresentCpiRequest, PresentCpiResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public PresentCpiUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<PresentCpiResponse> Execute(PresentCpiRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<Cpi> inflationRecords = await RetrieveInflationRecordsFromStorage(cancellationToken);

		return new PresentCpiResponse
		{
			InflationRecords = inflationRecords
				.Select(x => new CpiDto(x))
				.ToList()
		};
	}

	private async Task<IEnumerable<Cpi>> RetrieveInflationRecordsFromStorage(CancellationToken cancellationToken)
	{
		List<Cpi> inflationRecords = await unitOfWork.CpiRepository.GetAllAsync(cancellationToken)
			.ToListAsync(cancellationToken);

		return inflationRecords
			.OrderBy(x => x.Year);
	}
}