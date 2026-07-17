using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.AverageWageRepositoryTests;

public class DeleteTests
{
	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WithNullAverageWage_ShouldThrowArgumentNullException(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.Delete(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WithExistingRecord_ShouldRemoveIt(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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
			.Act((repository, context) =>
			{
				AverageWage averageWageToDelete = new()
				{
					Year = 2023,
					GrossValue = 6789.50m,
					NetValue = 4123.75m
				};

				repository.Delete(averageWageToDelete);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WithRecordFetchedThroughGetAsync_ShouldRemoveIt(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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
				AverageWage fetchedAverageWage = await repository.GetAsync(2023, CancellationToken.None);
				repository.Delete(fetchedAverageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WhenRecordDoesNotExist_ShouldNotThrowAndLeaveDatabaseUnchanged(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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
			.Act((repository, context) =>
			{
				AverageWage nonExistentAverageWage = new()
				{
					Year = 1999,
					GrossValue = 1000m,
					NetValue = 700m
				};

				repository.Delete(nonExistentAverageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(1);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WhenDatabaseIsEmpty_ShouldNotThrow(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				AverageWage nonExistentAverageWage = new()
				{
					Year = 2023,
					GrossValue = 1000m,
					NetValue = 700m
				};

				repository.Delete(nonExistentAverageWage);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WithMultipleRecords_ShouldRemoveOnlyTheSpecifiedRecord(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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
			.Act((repository, context) =>
			{
				AverageWage averageWageToDelete = new()
				{
					Year = 2022,
					GrossValue = 6100.25m,
					NetValue = 3700.50m
				};

				repository.Delete(averageWageToDelete);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();

				averageWages.Should().HaveCount(2);
				averageWages.Should().ContainSingle(x => x.Year == 2021 && x.GrossValue == 5500.0m && x.NetValue == 3300.0m);
				averageWages.Should().ContainSingle(x => x.Year == 2023 && x.GrossValue == 6789.50m && x.NetValue == 4123.75m);
				averageWages.Should().NotContain(x => x.Year == 2022);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WithRecordHavingNullValues_ShouldRemoveIt(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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
			.Act((repository, context) =>
			{
				AverageWage averageWageToDelete = new()
				{
					Year = 2023,
					GrossValue = null,
					NetValue = null
				};

				repository.Delete(averageWageToDelete);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IAverageWageRepository, IAverageWageStorageGateway>]
	public async Task Delete_WhenAllRecordsAreDeleted_ShouldLeaveDatabaseEmpty(ITestEnvironment<IAverageWageRepository, IAverageWageStorageGateway> environment)
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

				await gateway.SeedAverageWagesAsync([averageWage2021, averageWage2022]);
			})
			.Act(async (repository, context) =>
			{
				AverageWage fetched2021 = await repository.GetAsync(2021, CancellationToken.None);
				AverageWage fetched2022 = await repository.GetAsync(2022, CancellationToken.None);

				repository.Delete(fetched2021);
				repository.Delete(fetched2022);
			})
			.Assert(async (gateway, context) =>
			{
				List<AverageWage> averageWages = await gateway.GetAllAverageWagesAsync();
				averageWages.Should().BeEmpty();
			})
			.ExecuteAsync();
	}
}
