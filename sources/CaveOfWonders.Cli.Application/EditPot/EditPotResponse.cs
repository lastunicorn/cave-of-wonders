namespace DustInTheWind.CaveOfWonders.Cli.Application.EditPot;

public class EditPotResponse
{
	public Guid PotId { get; set; }

	public string PotName { get; set; }

	public bool NameUpdated { get; set; }

	public string OldName { get; set; }

	public string NewName { get; set; }

	public bool DescriptionUpdated { get; set; }

	public string OldDescription { get; set; }

	public string NewDescription { get; set; }

	public bool CurrencyUpdated { get; set; }

	public string OldCurrency { get; set; }

	public string NewCurrency { get; set; }
}
