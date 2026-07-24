using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots;

public class ImportPotSnapshotsRequest : IRequest<ImportPotSnapshotsResponse>
{
	public string SourceFilePath { get; set; }

	public string MappingsFilePath { get; set; }

	public bool Overwrite { get; set; }
}