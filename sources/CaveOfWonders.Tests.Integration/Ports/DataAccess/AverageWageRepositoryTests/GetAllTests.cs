using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests;

public class GetAllTests
{
	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithOneAverageWageRecord_ShouldReturnOneAverageWageRecord(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.Year.Should().Be(2023);
				averageWage.GrossValue.Should().Be(6789.50m);
				averageWage.NetValue.Should().Be(4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithMultipleAverageWageRecords_ShouldReturnAllAverageWageRecords(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWage2021 = new()
				{
					Year = 2021,
					GrossValue = 5500.0m,
					NetValue = 3300.0m
				};

				AverageWage averageWage2022 = new()
				{
					Year = 2022,
					GrossValue = 6100.25m,
					NetValue = 3700.50m
				};

				AverageWage averageWage2023 = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				await gateway.SeedAverageWagesAsync([averageWage2021, averageWage2022, averageWage2023]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(3);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2022 && x.GrossValue == 6100.25m && x.NetValue == 3700.50m);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithAverageWageRecordsAddedOutOfOrder_ShouldReturnAllAverageWageRecords(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWage2023 = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				AverageWage averageWage2021 = new()
				{
					Year = 2021,
					GrossValue = 5500.0m,
					NetValue = 3300.0m
				};

				AverageWage averageWage2022 = new()
				{
					Year = 2022,
					GrossValue = 6100.25m,
					NetValue = 3700.50m
				};

				await gateway.SeedAverageWagesAsync([averageWage2023, averageWage2021, averageWage2022]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(3);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2022 && x.GrossValue == 6100.25m && x.NetValue == 3700.50m);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithDecimalValues_ShouldPreserveDecimalPrecision(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.123456m,
					NetValue = 4123.654321m
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(6789.123456m);
				averageWage.NetValue.Should().Be(4123.654321m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithNullGrossValue_ShouldReturnNullGrossValue(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = 4123.75m
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().BeNull();
				averageWage.NetValue.Should().Be(4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithNullNetValue_ShouldReturnNullNetValue(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = null
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(6789.50m);
				averageWage.NetValue.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithBothValuesNull_ShouldReturnRecordWithNullValues(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = null
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.Year.Should().Be(2023);
				averageWage.GrossValue.Should().BeNull();
				averageWage.NetValue.Should().BeNull();
				averageWage.IsEmpty.Should().BeTrue();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithZeroValues_ShouldReturnZeroValues(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 0m,
					NetValue = 0m
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(0m);
				averageWage.NetValue.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task GetAll_WithMixOfCompleteAndPartialRecords_ShouldReturnAllRecordsAsSeeded(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage complete = new()
				{
					Year = 2021,
					GrossValue = 5500.0m,
					NetValue = 3300.0m
				};

				AverageWage grossOnly = new()
				{
					Year = 2022,
					GrossValue = 6100.25m,
					NetValue = null
				};

				AverageWage empty = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = null
				};

				await gateway.SeedAverageWagesAsync([complete, grossOnly, empty]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWages = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<AverageWage> averageWages = context.AverageWages as List<AverageWage>;

				averageWages.Should().HaveCount(3);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2022 && x.GrossValue == 6100.25m && x.NetValue == null);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == null && x.NetValue == null);
			})
			.ExecuteAsync();
	}
}
