// Cave of Wonders
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

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.CreatePot;

internal class CreatePotView : ViewBase<CreatePotViewModel>
{
    public override void Display(CreatePotViewModel viewModel)
    {
        CustomConsole.WriteLineSuccess($"Pot '{viewModel.PotName}' with currency '{viewModel.Currency}' created successfully.");
        
        DisplayPotDetails(viewModel);
    }
    
    private void DisplayPotDetails(CreatePotViewModel viewModel)
    {
        DataGrid dataGrid = DataGridTemplate.CreateNew();
        dataGrid.Title = "Pot Details";
        
        dataGrid.Columns.Add(new Column
        {
            ForegroundColor = ConsoleColor.White
        });
        
        dataGrid.Rows.Add("Id", viewModel.PotId);
        dataGrid.Rows.Add("Name", viewModel.PotName);
        
        if (!string.IsNullOrEmpty(viewModel.Description))
            dataGrid.Rows.Add("Description", viewModel.Description);
            
        dataGrid.Rows.Add("Start Date", viewModel.StartDate.ToString("d"));
        dataGrid.Rows.Add("Currency", viewModel.Currency);
        
        dataGrid.Display();
    }
}