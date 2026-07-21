using Newtonsoft.Json;

namespace DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;

internal class JColumnMapping
{
	[JsonProperty("pot-id")]
	public Guid PotId { get; set; }

	[JsonProperty("value-index")]
	public int Index { get; set; }

	[JsonProperty("date-index")]
	public int DateIndex { get; set; }
}