using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

public class PotDetailsApiDto
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public DateOnly StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public string Currency { get; set; }

	public int SnapshotCount { get; set; }

	public DateOnly? LastSnapshotDate { get; set; }

	public DatedAmount Value { get; set; }

	public List<string> Labels { get; set; }

	internal static PotDetailsApiDto From(PotDetails potDetails)
	{
		if (potDetails == null)
			return null;

		return new PotDetailsApiDto()
		{
			Id = potDetails.Id,
			Name = potDetails.Name,
			Description = potDetails.Description,
			StartDate = potDetails.StartDate,
			EndDate = potDetails.EndDate,
			Currency = potDetails.Currency,
			SnapshotCount = potDetails.SnapshotCount,
			Labels = potDetails.Labels?.ToList() ?? [],
			LastSnapshotDate = potDetails.LastSnapshotDate,
			Value = potDetails.Value
		};
	}
}