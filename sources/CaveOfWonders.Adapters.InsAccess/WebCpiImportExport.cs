using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.Ins.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

public class WebCpiImportExport : ICpiImportExport
{
	private readonly Lazy<InsConfig> insConfig = new(() => new InsConfig());

	public Guid Id => new Guid("3ff33b30-a149-4f08-b545-e524fd3e4384");

	public string Name => "CPI Web Import/Export";

	public bool CanImport => true;

	public bool CanExport => false;

	public async IAsyncEnumerable<CpiRecordDto> ImportAsync(IDictionary<string, object> parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		Uri url = insConfig.Value.CpiPageUrl;

		if (url == null)
			throw new MissingCpiUrlException();

		YearlyCpiWebPage webPage = new(url);
		IEnumerable<YearlyCpiRecord> yearlyCpiRecords = await webPage.EnumerateRecords(cancellationToken);

		foreach (YearlyCpiRecord yearlyCpiRecord in yearlyCpiRecords)
		{
			yield return new CpiRecordDto
			{
				Year = yearlyCpiRecord.Year,
				Value = yearlyCpiRecord.Value
			};
		}
	}

	public Task ExportAsync(IDictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
	{
		throw new NotSupportedException("Exporting CPI records to the web is not supported.");
	}
}