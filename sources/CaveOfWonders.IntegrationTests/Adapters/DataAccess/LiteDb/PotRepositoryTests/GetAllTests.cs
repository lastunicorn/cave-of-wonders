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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.Infrastructure;
using FluentAssertions;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.PotRepositoryTests;

public class GetAllTests
{
    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection()
    {
        await DatabaseTest.Create()
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;
                pots.Should().BeEmpty();
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithOnePot_ShouldReturnOnePot()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot",
                    Description = "This is a test pot",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Id.Should().NotBeEmpty();
                pot.Name.Should().Be("Test Pot");
                pot.Description.Should().Be("This is a test pot");
                pot.DisplayOrder.Should().Be(1);
                pot.StartDate.Should().Be(new DateTime(2023, 1, 1));
                pot.Currency.Should().Be("USD");
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithMultiplePots_ShouldReturnAllPots()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity pot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 1",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };

                PotDbEntity pot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 2",
                    DisplayOrder = 2,
                    StartDate = new DateTime(2023, 2, 1),
                    Currency = "EUR"
                };

                PotDbEntity pot3 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 3",
                    DisplayOrder = 3,
                    StartDate = new DateTime(2023, 3, 1),
                    Currency = "GBP"
                };

                dbContext.Pots.Insert(pot1);
                dbContext.Pots.Insert(pot2);
                dbContext.Pots.Insert(pot3);

                context.Pot1Id = pot1.Id;
                context.Pot2Id = pot2.Id;
                context.Pot3Id = pot3.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(3);

                Guid expectedPot1Id = (Guid)context.Pot1Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot1Id && x.Name == "Test Pot 1");

                Guid expectedPot2Id = (Guid)context.Pot2Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot2Id && x.Name == "Test Pot 2");

                Guid expectedPot3Id = (Guid)context.Pot3Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot3Id && x.Name == "Test Pot 3");
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithPotsContainingGems_ShouldReturnPotsWithGems()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Gems",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD",
                    Gems =
                    [
                        new GemDbEntity
                        {
                            Date = new DateTime(2023, 1, 15),
                            Value = 100.50m
                        },
                        new GemDbEntity
                        {
                            Date = new DateTime(2023, 2, 15),
                            Value = 120.75m
                        }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Gems.Should().HaveCount(2);
                pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 1, 15) && x.Value == 100.50m);
                pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 2, 15) && x.Value == 120.75m);
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Labels",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD",
                    Labels = [
                        "Savings",
                        "Long-term",
                        "Important"
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Labels.Should().HaveCount(3);
                pot.Labels.Should().Contain("Savings");
                pot.Labels.Should().Contain("Long-term");
                pot.Labels.Should().Contain("Important");
            })
            .Execute();
    }
}
