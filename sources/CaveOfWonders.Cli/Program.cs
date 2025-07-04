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

using DustInTheWind.CaveOfWonders.Cli.Presentation;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Setup.Autofac;

namespace DustInTheWind.CaveOfWonders.Cli;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
            .ConfigureServices(DependenciesSetup.Configure)
            .RegisterCommandsFrom(typeof(PresentationAssemblyHandle).Assembly)
            .HandleExceptions(HandlerApplicationException)
            .Build();

        await application.RunAsync(args);
    }

    private static void HandlerApplicationException(object o, UnhandledApplicationExceptionEventArgs e)
    {
        CustomConsole.WriteError(e.Exception);
    }
}