﻿// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

internal class PotView : ViewBase<PotCommandViewModel>
{
    public override void Display(PotCommandViewModel viewModel)
    {
        bool isFirst = true;

        if (viewModel.PotDetailsViewModels == null || viewModel.PotDetailsViewModels.Count == 0)
        {
            CustomConsole.WriteLineWarning("There is no pot with the specified name or id.");
        }
        else
        {
            foreach (PotDetailsViewModel potDetailsViewModel in viewModel.PotDetailsViewModels)
            {
                if (isFirst)
                    isFirst = false;
                else
                    Console.WriteLine();

                DisplayPotDetails(potDetailsViewModel);
            }
        }
    }

    private static void DisplayPotDetails(PotDetailsViewModel potDetailsViewModel)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = potDetailsViewModel.Name;

        if (!potDetailsViewModel.IsActive)
        {
            DataGridTemplate.Disable(dataGrid);
        }
        else
        {
            dataGrid.Columns.Add(new Column
            {
                ForegroundColor = ConsoleColor.White
            });
        }

        dataGrid.Rows.Add("Id", potDetailsViewModel.Id);
        dataGrid.Rows.Add("Description", potDetailsViewModel.Description);
        dataGrid.Rows.Add("StartDate", potDetailsViewModel.StartDate.ToString("d"));
        dataGrid.Rows.Add("EndDate", potDetailsViewModel.EndDate?.ToString("d") ?? string.Empty);
        dataGrid.Rows.Add("Currency", potDetailsViewModel.Currency);
        dataGrid.Rows.Add("Labels", string.Join(", ", potDetailsViewModel.Labels));
        dataGrid.Rows.Add("Gem Count", potDetailsViewModel.GemCount);
        dataGrid.Rows.Add("Last Gem", $"{potDetailsViewModel.LastGemDate:d} ({potDetailsViewModel.Value.ToDisplayString()})");

        dataGrid.Display();
    }
}