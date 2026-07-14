using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.MintosAccess;
using DustInTheWind.Mintos.Toolkit;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.MintosAccess;

public class MintosService : IMintosService
{
	public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		StatementDocument statementDocument = await StatementDocument.LoadFromFileAsync(filePath);

		IEnumerable<Gem> gems = statementDocument
			.Select(ToGem);

		foreach (Gem gem in gems)
		{
			cancellationToken.ThrowIfCancellationRequested();
			yield return gem;
		}
	}

	private static Gem ToGem(TransactionRecord transactionRecord)
	{
		return new Gem
		{
			Id = Guid.NewGuid(),
			ExternalId = transactionRecord.TransactionId,
			Date = transactionRecord.Date,
			Amount = Math.Abs(transactionRecord.Turnover),
			Category = CalculateCategory(transactionRecord.PaymentType),
			Parameters =
			{
				{
					nameof(transactionRecord.Details), transactionRecord.Details
				},
				{
					nameof(transactionRecord.Balance), transactionRecord.Balance.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(transactionRecord.Currency), transactionRecord.Currency?.ToString()
				},
				{
					nameof(transactionRecord.PaymentType), transactionRecord.PaymentType.ToString()
				}
			}
		};
	}

	private static GemCategory CalculateCategory(PaymentType paymentType)
	{
		if (paymentType == PaymentType.BondInterestIncome) return GemCategory.Gain;
		if (paymentType == PaymentType.BondInvestmentPrincipalIncrease) return GemCategory.Internal;
		if (paymentType == PaymentType.BondTaxWithholding) return GemCategory.Tax;
		if (paymentType == PaymentType.Bonus) return GemCategory.Gain;
		if (paymentType == PaymentType.CashOutShareIncomeToSeller) return GemCategory.Internal;
		if (paymentType == PaymentType.DelayedInterestIncomeOnTransitRebuy) return GemCategory.Gain;
		if (paymentType == PaymentType.Deposits) return GemCategory.Deposit;
		if (paymentType == PaymentType.InterestReceived) return GemCategory.Gain;
		if (paymentType == PaymentType.InterestReceivedFromLoanRepurchase) return GemCategory.Gain;
		if (paymentType == PaymentType.InterestReceivedFromPendingPayments) return GemCategory.Gain;
		if (paymentType == PaymentType.Investment) return GemCategory.Internal;
		if (paymentType == PaymentType.LateFeesReceived) return GemCategory.Gain;
		if (paymentType == PaymentType.MintosCoreFee) return GemCategory.Fee;
		if (paymentType == PaymentType.MintosCustomLoansFee) return GemCategory.Fee;
		if (paymentType == PaymentType.PrincipalReceived) return GemCategory.Internal;
		if (paymentType == PaymentType.PrincipalReceivedFromLoanRepurchase) return GemCategory.Internal;
		if (paymentType == PaymentType.PrincipalReceivedFromRepurchaseOfSmallLoanParts) return GemCategory.Internal;
		if (paymentType == PaymentType.RealEstateInterestIncome) return GemCategory.Gain;
		if (paymentType == PaymentType.RealEstateInvestmentPrincipalIncrease) return GemCategory.Internal;
		if (paymentType == PaymentType.RealEstateTaxWithholding) return GemCategory.Tax;
		if (paymentType == PaymentType.SecondaryMarketTransaction) return GemCategory.Internal;
		if (paymentType == PaymentType.SecondaryMarketTransactionDiscountOrPremium) return GemCategory.Internal;
		if (paymentType == PaymentType.TaxWithholding) return GemCategory.Tax;
		if (paymentType == PaymentType.Withdrawal) return GemCategory.Withdrawal;

		return GemCategory.Unknown;
	}
}