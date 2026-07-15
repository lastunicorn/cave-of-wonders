using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests;

public class GetByYearTests
{
	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WhenDatabaseIsEmpty_ShouldReturnNull(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;
				cpi.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithMatchingYear_ShouldReturnThatCpiRecord(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				repository.Add(cpiInDb);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2023);
				cpi.Value.Should().Be(105.5m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithNoMatchingYear_ShouldReturnNull(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				repository.Add(cpiInDb);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2024);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;
				cpi.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithMultipleCpiRecords_ShouldReturnOnlyTheMatchingOne(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpi2021 = new()
				{
					Year = 2021,
					Value = 100.0m
				};

				Cpi cpi2022 = new()
				{
					Year = 2022,
					Value = 112.3m
				};

				Cpi cpi2023 = new()
				{
					Year = 2023,
					Value = 118.9m
				};

				repository.Add(cpi2021);
				repository.Add(cpi2022);
				repository.Add(cpi2023);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2022);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2022);
				cpi.Value.Should().Be(112.3m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithCpiRecordsAddedOutOfOrder_ShouldReturnTheMatchingOne(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpi2023 = new()
				{
					Year = 2023,
					Value = 118.9m
				};

				Cpi cpi2021 = new()
				{
					Year = 2021,
					Value = 100.0m
				};

				Cpi cpi2022 = new()
				{
					Year = 2022,
					Value = 112.3m
				};

				repository.Add(cpi2023);
				repository.Add(cpi2021);
				repository.Add(cpi2022);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2021);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2021);
				cpi.Value.Should().Be(100.0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithDecimalValue_ShouldPreserveDecimalPrecision(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 123.456789m
				};

				repository.Add(cpiInDb);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithZeroValue_ShouldReturnZeroValue(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 0m
				};

				repository.Add(cpiInDb);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetByYear_WithNegativeValue_ShouldReturnNegativeValue(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = -2.4m
				};

				repository.Add(cpiInDb);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((repository, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(-2.4m);
			})
			.ExecuteAsync();
	}
}
