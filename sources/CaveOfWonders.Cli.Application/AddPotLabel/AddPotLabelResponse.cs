namespace DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

public class AddPotLabelResponse
{
	public string Label { get; set; }

	public List<LabelAddResult> Items { get; set; } = [];
}
