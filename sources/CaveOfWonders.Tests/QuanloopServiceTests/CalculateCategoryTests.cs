using DustInTheWind.CaveOfWonders.Adapters.QuanloopAccess;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.Quanloop.Toolkit;
using FluentAssertions;

namespace CaveOfWonders.Tests.QuanloopServiceTests;

public class CalculateCategoryTests
{
	[Fact]
	public void HavingInterestRevenueDescription_WhenCategoryIsCalculated_ThenCategoryIsGain()
	{
		TransactionRecord transactionRecord = new()
		{
			Description = "Interest revenue 11/23",
			Amount = 22.06m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Gain);
	}

	[Fact]
	public void HavingCashbackDescription_WhenCategoryIsCalculated_ThenCategoryIsBonus()
	{
		TransactionRecord transactionRecord = new()
		{
			Description = "Cashback 11/23",
			Amount = 4.28m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Bonus);
	}

	[Fact]
	public void HavingSignUpBonusDescription_WhenCategoryIsCalculated_ThenCategoryIsBonus()
	{
		TransactionRecord transactionRecord = new()
		{
			Description = "Sign-up bonus",
			Amount = 5.00m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Bonus);
	}

	[Fact]
	public void HavingPlainReferenceDescriptionWithPositiveAmount_WhenCategoryIsCalculated_ThenCategoryIsDeposit()
	{
		TransactionRecord transactionRecord = new()
		{
			Counterpart = "Alexandru Nicolae Iuga",
			Account = "LT933250084840074515",
			Description = "QNL16176936",
			Amount = 500.00m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Deposit);
	}

	[Fact]
	public void HavingWithdrawalDescriptionWithNegativeAmount_WhenCategoryIsCalculated_ThenCategoryIsWithdrawal()
	{
		TransactionRecord transactionRecord = new()
		{
			Counterpart = "Alexandru Nicolae Iuga",
			Account = "LT933250084840074515",
			Description = "QS000LLLVN QNL16176936 Withdrawal",
			Amount = -900.00m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Withdrawal);
	}

	[Fact]
	public void HavingZeroAmountAndUnrecognizedDescription_WhenCategoryIsCalculated_ThenCategoryIsUnknown()
	{
		TransactionRecord transactionRecord = new()
		{
			Description = "Some unrecognized description",
			Amount = 0m
		};

		GemCategory category = QuanloopService.CalculateCategory(transactionRecord);

		category.Should().Be(GemCategory.Unknown);
	}
}
