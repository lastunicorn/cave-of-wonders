namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotCreate;

internal class PotCreateViewModel
{
	public string PotName { get; set; }

	public string Currency { get; set; }

	public Guid PotId { get; set; }

	public string Description { get; set; }

	public DateOnly StartDate { get; set; }
}