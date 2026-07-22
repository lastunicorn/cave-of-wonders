using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

/// <summary>
/// Information about a pot instance at a specific point in time
/// </summary>
public class PotInstanceInfoDto
{
	/// <summary>
	/// Unique identifier of the pot
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Name of the pot
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Current value of the pot in its original currency
	/// </summary>
	public CurrencyValueDto Value { get; set; }

	/// <summary>
	/// Normalized value of the pot (converted to a common currency)
	/// </summary>
	public CurrencyValueDto NormalizedValue { get; set; }

	/// <summary>
	/// Indicates whether the pot is currently active
	/// </summary>
	public bool IsActive { get; set; }

	/// <summary>
	/// Creates a DTO from the application layer PotInstanceInfo
	/// </summary>
	public static PotInstanceInfoDto From(PotInstanceInfo potInstanceInfo)
	{
		if (potInstanceInfo == null)
			return null;

		return new PotInstanceInfoDto
		{
			Id = potInstanceInfo.Id,
			Name = potInstanceInfo.Name,
			Value = CurrencyValueDto.From(potInstanceInfo.Value),
			NormalizedValue = CurrencyValueDto.From(potInstanceInfo.NormalizedValue),
			IsActive = potInstanceInfo.IsActive
		};
	}
}