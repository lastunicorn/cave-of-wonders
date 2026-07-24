namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots;

[Serializable]
internal class SourceFileNotProvidedException : Exception
{
	public SourceFileNotProvidedException()
		: base("Source file path must be provided for import.")
	{
	}
}