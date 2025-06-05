// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Import;

[NamedCommand("import-exchange", Description = "Execute exchange rates from local files or directly from the BNR website.")]
internal class ImportCommand : IConsoleCommand<ImportResultViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("source-type", ShortName = 's', IsOptional = false, Description = "The source of the imported data. (bnr - bnr file; nbr - nbr file; web - nbr file from BNR website)")]
    public ImportSourceType SourceType { get; set; }

    [NamedParameter("file", ShortName = 'f', IsOptional = true, Description = "The full name of the file containing the exchange rates. Used by bnr and nbr imports.")]
    public string SourceFilePath { get; set; }

    [NamedParameter("year", ShortName = 'y', IsOptional = true, Description = "The year to be imported. Used by web import.")]
    public int? Year { get; set; }

    public ImportCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportResultViewModel> Execute()
    {
        ImportExchangeRatesRequest request = new()
        {
            ImportSource = SourceType switch
            {
                ImportSourceType.Bnr => ImportSource.BnrFile,
                ImportSourceType.Nbr => ImportSource.BnrNbrFile,
                ImportSourceType.Web => ImportSource.BnrWebsite,
                _ => throw new ArgumentOutOfRangeException()
            },
            SourceFilePath = SourceFilePath,
            Year = Year
        };

        ImportExchangeRatesResponse response = await mediator.Send(request);

        return new ImportResultViewModel(response);
    }
}