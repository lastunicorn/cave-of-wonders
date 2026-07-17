using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests;

public class GetAllTests
{
	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;
				cpiRecords.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithOneCpiRecord_ShouldReturnOneCpiRecord(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				Cpi cpi = cpiRecords.First();
				cpi.Year.Should().Be(2023);
				cpi.Value.Should().Be(105.5m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithMultipleCpiRecords_ShouldReturnAllCpiRecords(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(3);
				cpiRecords.Should().ContainSingle(x => x.Year == 2021 && x.Value == 100.0m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2022 && x.Value == 112.3m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2023 && x.Value == 118.9m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithCpiRecordsAddedOutOfOrder_ShouldReturnAllCpiRecords(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(3);
				cpiRecords.Should().ContainSingle(x => x.Year == 2021 && x.Value == 100.0m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2022 && x.Value == 112.3m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2023 && x.Value == 118.9m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithZeroValue_ShouldReturnZeroValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task GetAll_WithNegativeValue_ShouldReturnNegativeValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(-2.4m);
			})
			.ExecuteAsync();
	}
}
