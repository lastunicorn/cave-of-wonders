using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

[NamedCommand("gem", Description = "Display a list of gems filtered by pot.")]
internal class GemCommand : IConsoleCommand<GemCommandViewModel>
{
	private readonly IMediator mediator;

	[AnonymousParameter(Order = 1, IsMandatory = false)]
	public string PotIdDefault { get; set; }

	[NamedParameter("pot", IsMandatory = false, Description = "The pot id for which to display the gems.")]
	public string PotId { get; set; }

	[NamedParameter("start-date", IsMandatory = false, Description = "The start date for which to display the gems.")]
	public DateOnly? StartDate { get; set; }

	[NamedParameter("end-date", IsMandatory = false, Description = "The end date for which to display the gems.")]
	public DateOnly? EndDate { get; set; }

	[NamedParameter("date", IsMandatory = false, Description = "The date for which to display the gems.")]
	public DateOnly? Date { get; set; }

	[NamedParameter("month", IsMandatory = false, Description = "The month for which to display the gems.")]
	public string Month { get; set; }

	[NamedParameter("current-month", IsMandatory = false, Description = "Display the gems for the current month.")]
	public bool CurrentMonth { get; set; }

	[NamedParameter("last-month", IsMandatory = false, Description = "Display the gems for the last month.")]
	public bool LastMonth { get; set; }

	[NamedParameter("latest-month", IsMandatory = false, Description = "Display the gems for the latest month that has gems.")]
	public bool LatestMonth { get; set; }

	[NamedParameter("current-year", IsMandatory = false, Description = "Display the gems for the current year.")]
	public bool CurrentYear { get; set; }

	[NamedParameter("last-year", IsMandatory = false, Description = "Display the gems for the last year.")]
	public bool LastYear { get; set; }

	[NamedParameter("latest-year", IsMandatory = false, Description = "Display the gems for the latest year that has gems.")]
	public bool LatestYear { get; set; }

	[NamedParameter("exclude-internal", IsMandatory = false, Description = "Exclude internal gems.")]
	public bool ExcludeInternal { get; set; }

	public GemCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<GemCommandViewModel> Execute()
	{
		PresentGemsRequest request = new()
		{
			PotId = PotIdDefault ?? PotId,
			StartDate = StartDate,
			EndDate = EndDate,
			Date = Date,
			Month = Month,
			CurrentMonth = CurrentMonth,
			LastMonth = LastMonth,
			LatestMonth = LatestMonth,
			CurrentYear = CurrentYear,
			LastYear = LastYear,
			LatestYear = LatestYear,
			ExcludeInternal = ExcludeInternal
		};

		PresentGemsResponse response = await mediator.Send(request);

		return new GemCommandViewModel
		{
			Gems = response.Gems,
			TotalAmount = response.TotalAmount
		};
	}
}