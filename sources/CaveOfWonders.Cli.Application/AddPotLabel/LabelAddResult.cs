namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

public class LabelAddResult
{
	public Guid PotId { get; init; }

	public string PotName { get; init; }

	public bool WasAdded { get; init; }

	public bool IsActive { get; init; }
}
