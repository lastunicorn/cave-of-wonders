using DustInTheWind.Bcr.Toolkit;
using DustInTheWind.CaveOfWonders.Adapters.BcrAccess;
using FluentAssertions;

namespace CaveOfWonders.Tests.BcrServiceTests;

public class BuildExternalIdTests
{
	[Fact]
	public void HavingOperationReferenceWithInternalWhitespace_WhenExternalIdIsBuilt_ThenReferenceIsReturnedByteIdentical()
	{
		const string operationReference = "2026050545182344 Nota contabila 2026050519519413";

		BankTransaction bankTransaction = new()
		{
			OperationReference = operationReference
		};

		string externalId = BcrService.BuildExternalId(bankTransaction);

		externalId.Should().Be(operationReference);
	}

	[Fact]
	public void HavingEmptyOperationReference_WhenExternalIdIsBuilt_ThenFallbackIsDerivedFromDateAndAmounts()
	{
		BankTransaction bankTransaction = new()
		{
			OperationReference = string.Empty,
			CompletionDate = new DateOnly(2026, 5, 4),
			CompletionHour = new TimeOnly(14, 30, 0),
			DebitAmount = 0,
			CreditAmount = 123.45m
		};

		string externalId = BcrService.BuildExternalId(bankTransaction);

		externalId.Should().Be("20260504-143000-0-123.45");
	}
}