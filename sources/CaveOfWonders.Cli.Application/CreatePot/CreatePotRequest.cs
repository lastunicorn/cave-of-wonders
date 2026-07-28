namespace DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;

public class CreatePotRequest
{
	public string Name { get; set; }

	public string Description { get; set; }

	public DateOnly? StartDate { get; set; }

	public string Currency { get; set; }
}