namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

public class LabelPotsDto
{
	public string Label { get; set; }
	
	public List<PotDto> Pots { get; init; } = [];
}