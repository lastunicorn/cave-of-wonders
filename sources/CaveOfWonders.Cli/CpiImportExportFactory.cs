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

using Autofac;
using Autofac.Core;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.Cli;

internal class CpiImportExportFactory : ICpiImportExportFactory
{
	private readonly ILifetimeScope lifetimeScope;

	public CpiImportExportFactory(ILifetimeScope lifetimeScope)
	{
		this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
	}

	public ICpiImportExport Create(CpiImportType type, CpiImportParameters parameters)
	{
		return type switch
		{
			CpiImportType.File => lifetimeScope.Resolve<FileCpiImportExport>(new Parameter[]
			{
				new NamedParameter("filePath", (string)parameters["FilePath"])
			}),
			CpiImportType.Web => lifetimeScope.Resolve<WebCpiImportExport>(),
			_ => throw new NotSupportedException($"CPI import of type '{type}' is not supported.")
		};
	}
}