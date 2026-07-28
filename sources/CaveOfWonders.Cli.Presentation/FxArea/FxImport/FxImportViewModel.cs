using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

internal class FxImportViewModel
{
	public int TotalCount { get; }

	public int AddedCount { get; }

	public int ExistingUpdatedCount { get; }

	public int ExistingIdenticalCount { get; }

	public int NewDuplicateIdenticalCount { get; }

	public int NewDuplicateDifferentCount { get; }

	public List<UpdateValueViewModel> Updates { get; }

	public List<DuplicateValueViewModel> Duplicates { get; }

	internal FxImportViewModel(ImportExchangeRatesResponse report)
	{
		TotalCount = report.TotalCount;
		AddedCount = report.AddedCount;
		ExistingUpdatedCount = report.ExistingUpdatedCount;
		ExistingIdenticalCount = report.ExistingIdenticalCount;
		NewDuplicateIdenticalCount = report.NewDuplicateIdenticalCount;
		NewDuplicateDifferentCount = report.NewDuplicateDifferentCount;
		Updates = report.Updates
			.Select(x => new UpdateValueViewModel(x))
			.ToList();
		Duplicates = report.Duplicates
			.Select(x => new DuplicateValueViewModel(x))
			.ToList();
	}
}