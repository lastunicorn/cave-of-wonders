using DustInTheWind.CaveOfWonders.Adapters.QuanloopAccess;
using DustInTheWind.Quanloop.Toolkit;
using FluentAssertions;

namespace CaveOfWonders.Tests.QuanloopServiceTests;

public class BuildExternalIdTests
{
	[Fact]
	public void HavingSameTransactionRecord_WhenExternalIdIsBuiltTwice_ThenBothIdsAreEqual()
	{
		TransactionRecord transactionRecord = new()
		{
			Date = new DateOnly(2023, 12, 15),
			Description = "Interest revenue 11/23",
			Amount = 22.06m
		};

		string externalId1 = QuanloopService.BuildExternalId(transactionRecord);
		string externalId2 = QuanloopService.BuildExternalId(transactionRecord);

		externalId1.Should().Be(externalId2);
	}

	[Fact]
	public void HavingTwoRecordsOnSameDateWithDifferentAmountsAndDescriptions_WhenExternalIdIsBuilt_ThenIdsAreDifferent()
	{
		TransactionRecord transactionRecord1 = new()
		{
			Date = new DateOnly(2023, 5, 11),
			Description = "Sign-up bonus",
			Amount = 5.00m
		};

		TransactionRecord transactionRecord2 = new()
		{
			Date = new DateOnly(2023, 5, 11),
			Description = "Interest revenue 04/23",
			Amount = 10.00m
		};

		string externalId1 = QuanloopService.BuildExternalId(transactionRecord1);
		string externalId2 = QuanloopService.BuildExternalId(transactionRecord2);

		externalId1.Should().NotBe(externalId2);
	}
}
