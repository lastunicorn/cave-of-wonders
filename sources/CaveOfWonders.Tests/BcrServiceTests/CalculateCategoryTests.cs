using DustInTheWind.Bcr.Toolkit;
using DustInTheWind.CaveOfWonders.Adapters.BcrAccess;
using DustInTheWind.CaveOfWonders.DataTypes;
using FluentAssertions;

namespace CaveOfWonders.Tests.BcrServiceTests;

public class CalculateCategoryTests
{
	[Theory]
	[InlineData("Dobanda (dupa impozitare) Maxicont BCR RO06...")]
	[InlineData("  Dobanda (dupa impozitare) Maxicont BCR RO06...")]
	[InlineData("dobanda (dupa impozitare) Maxicont BCR RO06...")]
	public void HavingCreditTransactionWithDobandaDetails_WhenCategoryIsCalculated_ThenCategoryIsGain(string details)
	{
		BankTransaction bankTransaction = new()
		{
			CreditAmount = 100,
			DebitAmount = 0,
			Details = details
		};

		GemCategory category = BcrService.CalculateCategory(bankTransaction);

		category.Should().Be(GemCategory.Gain);
	}

	[Fact]
	public void HavingCreditTransactionWithOtherDetails_WhenCategoryIsCalculated_ThenCategoryIsDeposit()
	{
		BankTransaction bankTransaction = new()
		{
			CreditAmount = 100,
			DebitAmount = 0,
			Details = "Tranzactie efectuata prin George Banking BCR Referinta ..."
		};

		GemCategory category = BcrService.CalculateCategory(bankTransaction);

		category.Should().Be(GemCategory.Deposit);
	}

	[Fact]
	public void HavingDebitTransaction_WhenCategoryIsCalculated_ThenCategoryIsWithdrawal()
	{
		BankTransaction bankTransaction = new()
		{
			CreditAmount = 0,
			DebitAmount = 50,
			Details = "Tranzactie efectuata prin George Banking BCR Referinta ..."
		};

		GemCategory category = BcrService.CalculateCategory(bankTransaction);

		category.Should().Be(GemCategory.Withdrawal);
	}

	[Fact]
	public void HavingNeitherCreditNorDebitAmount_WhenCategoryIsCalculated_ThenCategoryIsUnknown()
	{
		BankTransaction bankTransaction = new()
		{
			CreditAmount = 0,
			DebitAmount = 0,
			Details = string.Empty
		};

		GemCategory category = BcrService.CalculateCategory(bankTransaction);

		category.Should().Be(GemCategory.Unknown);
	}
}