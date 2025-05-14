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

using Microsoft.Extensions.Configuration;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

internal class InsConfig
{
    private const string AppSettingsFileName = "appsettings.json";
    private readonly IConfigurationSection insConfig;

    public string InflationPageUrl => insConfig["InflationPageUrl"];

    public InsConfig()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile(AppSettingsFileName, optional: true, reloadOnChange: true);

        IConfigurationRoot configurationRoot = builder.Build();

        insConfig = configurationRoot.GetRequiredSection("Ins");
    }
}