using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWage;

internal class PresentWageUseCase : IUseCase<PresentWageRequest, PresentWageResponse>
{
	private readonly IUnitOfWork unitOfWork;

	public PresentWageUseCase(IUnitOfWork unitOfWork)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<PresentWageResponse> Execute(PresentWageRequest request, CancellationToken cancellationToken)
	{
		IAsyncEnumerable<AverageWage> averageWages = unitOfWork.AverageWageRepository.GetAllAsync(cancellationToken);

		return new PresentWageResponse
		{
			Values = await averageWages
				.Select(x => new AverageWageDto
				{
					Year = x.Year,
					GrossValue = x.GrossValue,
					NetValue = x.NetValue
				})
				.ToListAsync(cancellationToken)
		};
	}
}