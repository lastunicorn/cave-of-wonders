// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

[NamedCommand("state", Description = "Displays the values stored in the cave for a specific date.")]
public class StateCommand : CommandBase
{
    private readonly IMediator mediator;

    [NamedParameter("date", ShortName = 'd', IsOptional = true, Description = "The date for which to display the state of the cave. Default value = today")]
    public DateTime? Date { get; set; }

    [NamedParameter("currency", ShortName = 'c', IsOptional = true)]
    public string Currency { get; set; }

    public StateCommand(EnhancedConsole console, IMediator mediator)
        : base(console)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task Execute()
    {
        PresentStateRequest request = new()
        {
            Date = Date,
            Currency = Currency
        };

        PresentStateResponse response = await mediator.Send(request);

        DisplayCaveInstance(new CaveViewModel(response));
    }

    private void DisplayCaveInstance(CaveViewModel caveViewModel)
    {
        Console.WriteTitle(caveViewModel.Date.ToString("d"));

        foreach (PotInstance potInstance in caveViewModel.Values)
        {
            string value = potInstance.Value?.ToString();

            if (potInstance.ConvertedValue != null)
                value += $" ({potInstance.ConvertedValue})";

            Console.WriteValue(potInstance.Name, value);
        }

        Console.WriteLine();
        Console.WriteValue("Total", caveViewModel.Total);

        Console.WriteLine();
        Console.WithIndentation("Conversion Rates", () =>
        {
            foreach (ConversionRate conversionRate in caveViewModel.ConversionRates)
            {
                string sourceCurrency = conversionRate.SourceCurrency;
                float conversionValue = conversionRate.Value;
                string destinationCurrency = conversionRate.DestinationCurrency;

                Console.WriteInfo($"1 {sourceCurrency} = {conversionValue} {destinationCurrency}");
            }
        });
    }

    //private static void DisplayCaveInstances(CaveInstance caveInstance)
    //{
    //    DataGrid dataGrid = new();

    //    dataGrid.Columns.Add("Date");

    //    foreach (CurrencyValue currencyValue in caveInstance.Values)
    //        dataGrid.Columns.Add(currencyValue.Currency);

    //    dataGrid.Columns.Add($"Total ({caveInstance.Total.Currency})");

    //    ContentRow contentRow = ToContentRow(caveInstance);
    //    dataGrid.Rows.Add(contentRow);

    //    dataGrid.Display();
    //}

    //private static ContentRow ToContentRow(CaveInstance caveInstance)
    //{
    //    ContentRow contentRow = new();

    //    contentRow.AddCell(caveInstance.Date.ToString("d"));

    //    foreach (CurrencyValue currencyValue in caveInstance.Values)
    //        contentRow.AddCell(currencyValue.Value + " " + currencyValue.Currency);

    //    contentRow.AddCell(caveInstance.Total);

    //    return contentRow;
    //}
}