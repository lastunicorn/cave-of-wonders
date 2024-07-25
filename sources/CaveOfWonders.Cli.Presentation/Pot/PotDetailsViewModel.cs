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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Pot;

internal class PotDetailsViewModel
{
    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateTime StartDate { get; }

    public DateTime? EndDate { get; }

    public string Currency { get; }

    public int GemCount { get; }

    public bool IsActive { get; set; }

    public DateTime? LastGemDate { get; set; }

    public PotDetailsViewModel(PotDetails potDetails)
    {
        if (potDetails == null)
            return;

        Id = potDetails.Id;
        Name = potDetails.Name;
        Description = potDetails.Description;
        StartDate = potDetails.StartDate;
        EndDate = potDetails.EndDate;
        Currency = potDetails.Currency;
        GemCount = potDetails.GemCount;
        LastGemDate = potDetails.LastGemDate;

        IsActive = potDetails.EndDate == null || potDetails.EndDate >= DateTime.Today;
    }
}