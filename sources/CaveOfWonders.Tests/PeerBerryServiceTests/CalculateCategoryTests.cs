using DustInTheWind.CaveOfWonders.Adapters.PeerBerryAccess;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.PeerBerry.Toolkit;
using FluentAssertions;

namespace CaveOfWonders.Tests.PeerBerryServiceTests;

public class CalculateCategoryTests
{
	[Fact]
	public void HavingDepositTransactionType_WhenCategoryIsCalculated_ThenCategoryIsDeposit()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.Deposit);

		category.Should().Be(GemCategory.Deposit);
	}

	[Fact]
	public void HavingInvestmentTransactionType_WhenCategoryIsCalculated_ThenCategoryIsInternal()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.Investment);

		category.Should().Be(GemCategory.Internal);
	}

	[Fact]
	public void HavingRepaymentInterestTransactionType_WhenCategoryIsCalculated_ThenCategoryIsGain()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.RepaymentInterest);

		category.Should().Be(GemCategory.Gain);
	}

	[Fact]
	public void HavingRepaymentPrincipalTransactionType_WhenCategoryIsCalculated_ThenCategoryIsInternal()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.RepaymentPrincipal);

		category.Should().Be(GemCategory.Internal);
	}

	[Fact]
	public void HavingBuybackInterestTransactionType_WhenCategoryIsCalculated_ThenCategoryIsGain()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.BuybackInterest);

		category.Should().Be(GemCategory.Gain);
	}

	[Fact]
	public void HavingBuybackPrincipalTransactionType_WhenCategoryIsCalculated_ThenCategoryIsInternal()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.BuybackPrincipal);

		category.Should().Be(GemCategory.Internal);
	}

	[Fact]
	public void HavingWithdrawalTransactionType_WhenCategoryIsCalculated_ThenCategoryIsWithdrawal()
	{
		GemCategory category = PeerBerryService.CalculateCategory(TransactionType.Withdrawal);

		category.Should().Be(GemCategory.Withdrawal);
	}

	[Fact]
	public void HavingUnrecognizedTransactionType_WhenCategoryIsCalculated_ThenCategoryIsUnknown()
	{
		TransactionType transactionType = new("SOME_OTHER_VALUE");

		GemCategory category = PeerBerryService.CalculateCategory(transactionType);

		category.Should().Be(GemCategory.Unknown);
	}
}