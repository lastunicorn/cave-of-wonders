using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests;

public class GetByYearTests
{
	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WhenDatabaseIsEmpty_ShouldReturnNull(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;
				cpi.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithMatchingYear_ShouldReturnThatCpiRecord(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				await gateway.SeedCpisAsync([cpiInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2023);
				cpi.Value.Should().Be(105.5m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithNoMatchingYear_ShouldReturnNull(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				await gateway.SeedCpisAsync([cpiInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2024);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;
				cpi.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithMultipleCpiRecords_ShouldReturnOnlyTheMatchingOne(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
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

				await gateway.SeedCpisAsync([cpi2021, cpi2022, cpi2023]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2022);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2022);
				cpi.Value.Should().Be(112.3m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithCpiRecordsAddedOutOfOrder_ShouldReturnTheMatchingOne(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
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

				await gateway.SeedCpisAsync([cpi2023, cpi2021, cpi2022]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2021);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Year.Should().Be(2021);
				cpi.Value.Should().Be(100.0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 123.456789m
				};

				await gateway.SeedCpisAsync([cpiInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithZeroValue_ShouldReturnZeroValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = 0m
				};

				await gateway.SeedCpisAsync([cpiInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryEnvironments]
	public async Task GetByYear_WithNegativeValue_ShouldReturnNegativeValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpiInDb = new()
				{
					Year = 2023,
					Value = -2.4m
				};

				await gateway.SeedCpisAsync([cpiInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Cpi = await repository.GetByYear(2023);
			})
			.Assert((gateway, context) =>
			{
				Cpi cpi = context.Cpi as Cpi;

				cpi.Should().NotBeNull();
				cpi.Value.Should().Be(-2.4m);
			})
			.ExecuteAsync();
	}
}
