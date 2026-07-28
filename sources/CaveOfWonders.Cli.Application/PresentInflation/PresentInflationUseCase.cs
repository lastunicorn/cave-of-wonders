using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;

internal class PresentInflationUseCase : IUseCase<PresentInflationRequest, PresentInflationResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public PresentInflationUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<PresentInflationResponse> Execute(PresentInflationRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<Cpi> inflationRecords = await RetrieveCpiRecordsFromStorage(cancellationToken);

		return new PresentInflationResponse
		{
			InflationRecords = inflationRecords
				.Select(x => new InflationDto(x))
				.ToList()
		};
	}

	private async Task<IEnumerable<Cpi>> RetrieveCpiRecordsFromStorage(CancellationToken cancellationToken)
	{
		List<Cpi> cpis = await unitOfWork.CpiRepository.GetAllAsync(cancellationToken)
			.ToListAsync(cancellationToken);

		return cpis
			.OrderBy(x => x.Year);
	}
}