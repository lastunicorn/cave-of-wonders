using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.CpiRepositoryTests;

public class GetAllTests
{
	[Theory]
	[CpiRepositoryProviders]
	public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ISutFixture<ICpiRepository> sutFixture)
	{
		await new GenericTest<ICpiRepository>(sutFixture)
			.Act(async (repository, context) =>
			{
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;
				cpiRecords.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetAll_WithOneCpiRecord_ShouldReturnOneCpiRecord(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[CpiRepositoryProviders]
	public async Task GetAll_WithMultipleCpiRecords_ShouldReturnAllCpiRecords(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[CpiRepositoryProviders]
	public async Task GetAll_WithCpiRecordsAddedOutOfOrder_ShouldReturnAllCpiRecords(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[CpiRepositoryProviders]
	public async Task GetAll_WithDecimalValue_ShouldPreserveDecimalPrecision(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetAll_WithZeroValue_ShouldReturnZeroValue(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(0m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[CpiRepositoryProviders]
	public async Task GetAll_WithNegativeValue_ShouldReturnNegativeValue(ISutFixture<ICpiRepository> sutFixture)
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
				context.CpiRecords = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Cpi> cpiRecords = context.CpiRecords as List<Cpi>;

				cpiRecords.Should().HaveCount(1);
				cpiRecords.First().Value.Should().Be(-2.4m);
			})
			.ExecuteAsync();
	}
}
