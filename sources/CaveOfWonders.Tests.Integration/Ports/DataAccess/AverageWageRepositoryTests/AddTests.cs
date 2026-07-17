using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests;

public class AddTests
{
	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithValidAverageWage_ShouldPersistAverageWage(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

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
	public async Task Add_WithNullAverageWage_ShouldThrowArgumentNullException(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.Add(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithMultipleAverageWageRecords_ShouldPersistAllAverageWageRecords(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
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

				repository.Add(averageWage2021);
				repository.Add(averageWage2022);
				repository.Add(averageWage2023);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(3);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2022 && x.GrossValue == 6100.25m && x.NetValue == 3700.50m);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_ToDatabaseWithExistingRecords_ShouldPersistBothOldAndNewRecords(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				AverageWage averageWageInDb = new()
				{
					Year = 2021,
					GrossValue = 5500.0m,
					NetValue = 3300.0m
				};

				await gateway.SeedAverageWagesAsync([averageWageInDb]);
			})
			.Act((repository, context) =>
			{
				AverageWage newAverageWage = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				repository.Add(newAverageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(2);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithDecimalValues_ShouldPreserveDecimalPrecision(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = 6789.123456m,
					NetValue = 4123.654321m
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(6789.123456m);
				averageWage.NetValue.Should().Be(4123.654321m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithZeroValues_ShouldPersistZeroValues(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = 0m,
					NetValue = 0m
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(0m);
				averageWage.NetValue.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithNullGrossValue_ShouldPersistNullGrossValue(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = 4123.75m
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().BeNull();
				averageWage.NetValue.Should().Be(4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithNullNetValue_ShouldPersistNullNetValue(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = null
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(6789.50m);
				averageWage.NetValue.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Add_WithBothValuesNull_ShouldPersistRecordWithNullValues(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = null
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

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
	public async Task Add_WithNegativeValues_ShouldPersistNegativeValues(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage averageWage = new()
				{
					Year = 2023,
					GrossValue = -100.25m,
					NetValue = -75.50m
				};

				repository.Add(averageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				AverageWage averageWage = averageWages.First();
				averageWage.GrossValue.Should().Be(-100.25m);
				averageWage.NetValue.Should().Be(-75.50m);
			})
			.ExecuteAsync();
	}
}
