using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests;

public class GetTests
{
	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WhenDatabaseIsEmpty_ShouldReturnNull(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;
				averageWage.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithMatchingYear_ShouldReturnThatAverageWageRecord(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.Year.Should().Be(2023);
				averageWage.GrossValue.Should().Be(6789.50m);
				averageWage.NetValue.Should().Be(4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithNoMatchingYear_ShouldReturnNull(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2024, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;
				averageWage.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithMultipleAverageWageRecords_ShouldReturnOnlyTheMatchingOne(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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

				await backDoor.SeedAverageWagesAsync([averageWage2021, averageWage2022, averageWage2023]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2022, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.Year.Should().Be(2022);
				averageWage.GrossValue.Should().Be(6100.25m);
				averageWage.NetValue.Should().Be(3700.50m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithAverageWageRecordsAddedOutOfOrder_ShouldReturnTheMatchingOne(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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

				await backDoor.SeedAverageWagesAsync([averageWage2023, averageWage2021, averageWage2022]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2021, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.Year.Should().Be(2021);
				averageWage.GrossValue.Should().Be(5500.0m);
				averageWage.NetValue.Should().Be(3300.0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithDecimalValues_ShouldPreserveDecimalPrecision(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.123456m,
					NetValue = 4123.654321m
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.GrossValue.Should().Be(6789.123456m);
				averageWage.NetValue.Should().Be(4123.654321m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithZeroValues_ShouldReturnZeroValues(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 0m,
					NetValue = 0m
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.GrossValue.Should().Be(0m);
				averageWage.NetValue.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithNullGrossValue_ShouldReturnNullGrossValue(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = 4123.75m
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.GrossValue.Should().BeNull();
				averageWage.NetValue.Should().Be(4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithNullNetValue_ShouldReturnNullNetValue(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = null
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.GrossValue.Should().Be(6789.50m);
				averageWage.NetValue.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, ITestBackDoor>]
	public async Task GetAsync_WithBothValuesNull_ShouldReturnRecordWithNullValues(ITestEnvironment<IAverageWageRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = null
				};

				await backDoor.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.AverageWage = await repository.GetAsync(2023, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				AverageWage averageWage = context.AverageWage as AverageWage;

				averageWage.Should().NotBeNull();
				averageWage.Year.Should().Be(2023);
				averageWage.GrossValue.Should().BeNull();
				averageWage.NetValue.Should().BeNull();
				averageWage.IsEmpty.Should().BeTrue();
			})
			.ExecuteAsync();
	}
}