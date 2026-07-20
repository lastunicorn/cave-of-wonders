using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.PeerBerryAccess;
using DustInTheWind.PeerBerry.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.PeerBerryAccess;

public class PeerBerryService : IPeerBerryService
{
	public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		TransactionsDocument transactionsDocument = await TransactionsDocument.LoadFromFileAsync(filePath, cancellationToken);

		foreach (TransactionRecord transactionRecord in transactionsDocument.TransactionsSection.Transactions)
		{
			cancellationToken.ThrowIfCancellationRequested();
			yield return ToGem(transactionRecord);
		}
	}

	private static Gem ToGem(TransactionRecord transactionRecord)
	{
		return new Gem
		{
			Id = Guid.NewGuid(),
			ExternalId = transactionRecord.Id,
			Date = transactionRecord.Date,
			Amount = Math.Abs(transactionRecord.Amount),
			Category = CalculateCategory(transactionRecord.Type),
			Parameters =
			{
				{
					nameof(transactionRecord.Type), transactionRecord.Type?.ToString()
				},
				{
					nameof(transactionRecord.Currency), transactionRecord.Currency?.ToString()
				},
				{
					nameof(transactionRecord.LoanId), transactionRecord.LoanId
				},
				{
					nameof(transactionRecord.Country), transactionRecord.Country
				},
				{
					nameof(transactionRecord.LoanStatus), transactionRecord.LoanStatus?.ToString()
				}
			}
		};
	}

	internal static GemCategory CalculateCategory(TransactionType transactionType)
	{
		if (transactionType == TransactionType.Deposit) return GemCategory.Deposit;
		if (transactionType == TransactionType.Investment) return GemCategory.Internal;
		if (transactionType == TransactionType.RepaymentInterest) return GemCategory.Gain;
		if (transactionType == TransactionType.RepaymentPrincipal) return GemCategory.Internal;
		if (transactionType == TransactionType.BuybackInterest) return GemCategory.Gain;
		if (transactionType == TransactionType.BuybackPrincipal) return GemCategory.Internal;
		if (transactionType == TransactionType.Withdrawal) return GemCategory.Withdrawal;

		return GemCategory.Unknown;
	}
}