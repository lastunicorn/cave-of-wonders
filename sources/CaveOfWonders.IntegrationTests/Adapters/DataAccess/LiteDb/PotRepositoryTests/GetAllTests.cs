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
using FluentAssertions;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.PotRepositoryTests;

public class GetAllTests : DatabaseTests
{
    private readonly PotRepository potRepository;

    public GetAllTests()
    {
        potRepository = new PotRepository(DbContext);
    }

    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection()
    {
        // Act
        List<Pot> pots = (await potRepository.GetAll()).ToList();

        // Assert
        pots.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WithOnePot_ShouldReturnOnePot()
    {
        // Arrange
        PotDbEntity potDbEntity = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Pot",
            Description = "This is a test pot",
            DisplayOrder = 1,
            StartDate = new DateTime(2023, 1, 1),
            Currency = "USD"
        };

        DbContext.Pots.Insert(potDbEntity);

        // Act
        List<Pot> pots = (await potRepository.GetAll()).ToList();

        // Assert
        pots.Should().HaveCount(1);
        Pot pot = pots.First();
        pot.Id.Should().Be(potDbEntity.Id);
        pot.Name.Should().Be(potDbEntity.Name);
        pot.Description.Should().Be(potDbEntity.Description);
        pot.DisplayOrder.Should().Be(potDbEntity.DisplayOrder);
        pot.StartDate.Should().Be(potDbEntity.StartDate);
        pot.Currency.Should().Be(potDbEntity.Currency);
    }

    [Fact]
    public async Task GetAll_WithMultiplePots_ShouldReturnAllPots()
    {
        // Arrange
        var pot1 = new PotDbEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Pot 1",
            DisplayOrder = 1,
            StartDate = new DateTime(2023, 1, 1),
            Currency = "USD"
        };

        var pot2 = new PotDbEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Pot 2",
            DisplayOrder = 2,
            StartDate = new DateTime(2023, 2, 1),
            Currency = "EUR"
        };

        var pot3 = new PotDbEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Pot 3",
            DisplayOrder = 3,
            StartDate = new DateTime(2023, 3, 1),
            Currency = "GBP"
        };

        DbContext.Pots.Insert(pot1);
        DbContext.Pots.Insert(pot2);
        DbContext.Pots.Insert(pot3);

        // Act
        var result = await potRepository.GetAll();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainSingle(x => x.Id == pot1.Id && x.Name == pot1.Name);
        result.Should().ContainSingle(x => x.Id == pot2.Id && x.Name == pot2.Name);
        result.Should().ContainSingle(x => x.Id == pot3.Id && x.Name == pot3.Name);
    }

    [Fact]
    public async Task GetAll_WithPotsContainingGems_ShouldReturnPotsWithGems()
    {
        // Arrange
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

        DbContext.Pots.Insert(potDbEntity);

        // Act
        List<Pot> pots = (await potRepository.GetAll()).ToList();

        // Assert
        pots.Should().HaveCount(1);
        Pot pot = pots.First();
        pot.Gems.Should().HaveCount(2);
        pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 1, 15) && x.Value == 100.50m);
        pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 2, 15) && x.Value == 120.75m);
    }

    [Fact]
    public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels()
    {
        // Arrange
        var potDbEntity = new PotDbEntity
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

        DbContext.Pots.Insert(potDbEntity);

        // Act
        List<Pot> pots = (await potRepository.GetAll()).ToList();

        // Assert
        pots.Should().HaveCount(1);
        Pot pot = pots.First();
        pot.Labels.Should().HaveCount(3);
        pot.Labels.Should().Contain("Savings");
        pot.Labels.Should().Contain("Long-term");
        pot.Labels.Should().Contain("Important");
    }
}
