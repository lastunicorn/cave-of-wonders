namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public interface ICpiImportExport
{
	Guid Id { get; }

	string Name { get; }

	bool CanImport { get; }

	bool CanExport { get; }

	IAsyncEnumerable<CpiRecordDto> ImportAsync(IDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);

	Task ExportAsync(IDictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
}