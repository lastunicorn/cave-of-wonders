namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;

public class ImportCpiRequest
{
	public string SourceFilePath { get; set; }

	public ImportSource ImportSource { get; set; }
}