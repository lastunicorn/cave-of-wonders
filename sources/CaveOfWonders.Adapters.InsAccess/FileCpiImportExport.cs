using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

public class FileCpiImportExport : ICpiImportExport
{
	private readonly string filePath;

	public FileCpiImportExport(string filePath)
	{
		this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
	}

	public Guid Id => new Guid("bb7590ef-6126-4529-8012-b6a8a4c6f903");

	public string Name => "CPI File Import/Export";

	public bool CanImport => true;

	public bool CanExport => false;

	public async IAsyncEnumerable<CpiRecordDto> ImportAsync(IDictionary<string, object> parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		IEnumerable<string> lines = await File.ReadLinesAsync(filePath, cancellationToken)
			.ToListAsync(cancellationToken);

		CpiRecordDtoEnumerator enumerator = new(lines);

		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}

	public Task ExportAsync(IDictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException("Exporting CPI records to a file is not supported.");
	}
}