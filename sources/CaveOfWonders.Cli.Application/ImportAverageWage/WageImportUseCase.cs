using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportAverageWage;

internal class WageImportUseCase : IUseCase<WageImportRequest, WageImportResponse>
{
	private readonly IInsService insService;
	private readonly IUnitOfWork unitOfWork;

	public WageImportUseCase(IInsService insService, IUnitOfWork unitOfWork)
	{
		this.insService = insService ?? throw new ArgumentNullException(nameof(insService));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<WageImportResponse> Execute(WageImportRequest request, CancellationToken cancellationToken)
	{
		IEnumerable<AverageWage> averageWages = await insService.GetAverageWagesAsync();

		ImportResult importResult = new();
		foreach (AverageWage averageWage in averageWages)
		{
			importResult.TotalCount++;

			AverageWage existingAverageWage = await unitOfWork.AverageWageRepository.GetAsync(averageWage.Year, cancellationToken);

			if (existingAverageWage != null)
			{
				if (averageWage.IsEmpty)
				{
					importResult.DeletedCount++;

					unitOfWork.AverageWageRepository.Delete(existingAverageWage);
				}
				else if (existingAverageWage != averageWage)
				{
					importResult.UpdatedCount++;

					existingAverageWage.GrossValue = averageWage.GrossValue;
					existingAverageWage.NetValue = averageWage.NetValue;
				}
			}
			else if (!averageWage.IsEmpty)
			{
				importResult.AddedCount++;

				existingAverageWage = new AverageWage
				{
					Year = averageWage.Year,
					GrossValue = averageWage.GrossValue,
					NetValue = averageWage.NetValue
				};

				unitOfWork.AverageWageRepository.Add(existingAverageWage);
			}
		}

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return new WageImportResponse
		{
			TotalCount = importResult.TotalCount,
			AddedCount = importResult.AddedCount,
			UpdatedCount = importResult.UpdatedCount,
			DeletedCount = importResult.DeletedCount
		};
	}
}