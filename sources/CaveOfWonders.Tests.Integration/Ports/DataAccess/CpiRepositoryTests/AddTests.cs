using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests;

public class AddTests
{
	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithValidCpi_ShouldPersistCpi(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				Cpi cpi = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				repository.Add(cpi);
			})
			.Assert(async (gateway, context) =>
			{
				List<Cpi> cpiRecords = await gateway.GetAllCpisAsync();

				cpiRecords.Should().HaveCount(1);
				Cpi cpi = cpiRecords.First();
				cpi.Year.Should().Be(2023);
				cpi.Value.Should().Be(105.5m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithNullCpi_ShouldThrowArgumentNullException(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
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
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithMultipleCpiRecords_ShouldPersistAllCpiRecords(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
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
			.Assert(async (gateway, context) =>
			{
				List<Cpi> cpiRecords = await gateway.GetAllCpisAsync();

				cpiRecords.Should().HaveCount(3);
				cpiRecords.Should().ContainSingle(x => x.Year == 2021 && x.Value == 100.0m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2022 && x.Value == 112.3m);
				cpiRecords.Should().ContainSingle(x => x.Year == 2023 && x.Value == 118.9m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithDuplicateYear_ShouldThrow(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Cpi cpi = new()
				{
					Year = 2023,
					Value = 105.5m
				};

				await gateway.SeedCpisAsync([cpi]);
			})
			.Act((repository, context) =>
			{
				Cpi duplicateCpi = new()
				{
					Year = 2023,
					Value = 999.9m
				};

				repository.Add(duplicateCpi);
			})
			.AssertThrow(ex =>
			{
				ex.Should().NotBeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				Cpi cpi = new()
				{
					Year = 2023,
					Value = 123.456789m
				};

				repository.Add(cpi);
			})
			.Assert(async (gateway, context) =>
			{
				List<Cpi> cpiRecords = await gateway.GetAllCpisAsync();

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithZeroValue_ShouldPersistZeroValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				Cpi cpi = new()
				{
					Year = 2023,
					Value = 0m
				};

				repository.Add(cpi);
			})
			.Assert(async (gateway, context) =>
			{
				List<Cpi> cpiRecords = await gateway.GetAllCpisAsync();

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<ICpiRepository, ICpiStorageGateway>]
	public async Task Add_WithNegativeValue_ShouldPersistNegativeValue(ITestEnvironment<ICpiRepository, ICpiStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				Cpi cpi = new()
				{
					Year = 2023,
					Value = -2.4m
				};

				repository.Add(cpi);
			})
			.Assert(async (gateway, context) =>
			{
				List<Cpi> cpiRecords = await gateway.GetAllCpisAsync();

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(-2.4m);
			})
			.ExecuteAsync();
	}
}
