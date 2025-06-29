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

using System.Reflection;
using CaveOfWonders.WebApi.Presentation.Controllers;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.FileAccess;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Adapters.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;
using DustInTheWind.CaveOfWonders.Adapters.SystemAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.FileAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using Microsoft.OpenApi.Models;

namespace DustInTheWind.CaveOfWonders.WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers().AddApplicationPart(typeof(PotController).Assembly);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Cave of Wonders API",
                Description = "Web API for the Cave of Wonders financial management system"
            });

            // Set the comments path for the Swagger JSON and UI.
            Assembly assembly = typeof(PotController).Assembly;
            string xmlFilename = $"{assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // Register MediatR
        builder.Services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(PresentPotsRequest).Assembly));

        // Register application services
        builder.Services.AddSingleton(sp =>
        {
            string connectionString = builder.Configuration.GetValue<string>("Database:Path");
            return new Database(connectionString);
        });
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddSingleton<ISystemClock, SystemClock>();
        builder.Services.AddSingleton<IBnr, Bnr>();
        builder.Services.AddSingleton<IIns, Ins>();
        builder.Services.AddSingleton<ISheets, Sheets>();
        builder.Services.AddScoped<ILog, Log>();
        builder.Services.AddSingleton<IFileSystem, FileSystem>();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}