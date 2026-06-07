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

using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

internal class PotInstanceRow : ContentRow
{
    private PotInstanceViewModel viewModel;

    public PotInstanceViewModel ViewModel
    {
        get => viewModel;
        set
        {
            viewModel = value;
            RegenerateCells();
        }
    }

    private void RegenerateCells()
    {
        string id = viewModel.Id.ToString("D")[..8];
        AddCell(id);

        string name = viewModel.Name;
        AddCell(name);

        string value = viewModel.OriginalValue?.ToDisplayString();
        ContentCell valueCell = AddCell(value);

        string date = viewModel.Date?.ToString("d");
        ContentCell dateCell = AddCell(date);

        string normalizedValue = viewModel.NormalizedValue?.ToDisplayString();
        ContentCell normalizedValueCell = AddCell(normalizedValue);

        if (viewModel.IsPotActive)
        {
            if (!viewModel.IsValueActual)
            {
                valueCell.ForegroundColor = ConsoleColor.DarkYellow;
                dateCell.ForegroundColor = ConsoleColor.DarkYellow;
            }

            if (!viewModel.IsValueAlreadyNormal)
                valueCell.ForegroundColor = ConsoleColor.DarkGray;
        }
        else
        {
            ForegroundColor = ConsoleColor.DarkGray;
        }

        bool isNormalizedValue = viewModel.NormalizedValue?.Value != 0 &&
            !viewModel.IsValueAlreadyNormal &&
            !viewModel.IsNormalizedCurrent;

        if (isNormalizedValue)
            normalizedValueCell.ForegroundColor = ConsoleColor.DarkYellow;
    }
}