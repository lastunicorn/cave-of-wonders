using System.Globalization;
using System.Runtime.CompilerServices;
using DustInTheWind.Bcr.Toolkit;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.BcrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BcrAccess;

public class BcrService : IBcrService
{
	public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		StatementDocument statementDocument = await StatementDocument.LoadFromFileAsync(filePath);

		foreach (BankTransaction bankTransaction in statementDocument)
		{
			cancellationToken.ThrowIfCancellationRequested();
			yield return ToGem(bankTransaction);
		}
	}

	private static Gem ToGem(BankTransaction bankTransaction)
	{
		return new Gem
		{
			Id = Guid.NewGuid(),
			ExternalId = BuildExternalId(bankTransaction),
			Date = bankTransaction.CompletionDate.ToDateTime(bankTransaction.CompletionHour),
			Amount = bankTransaction.CreditAmount > 0
				? bankTransaction.CreditAmount
				: bankTransaction.DebitAmount,
			Category = CalculateCategory(bankTransaction),
			Parameters =
			{
				{
					nameof(bankTransaction.Details), bankTransaction.Details
				},
				{
					nameof(bankTransaction.OpeningBalance), bankTransaction.OpeningBalance.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.DebitTotal), bankTransaction.DebitTotal.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.CreditTotal), bankTransaction.CreditTotal.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.FinalBalance), bankTransaction.FinalBalance.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.BlockedAmounts), bankTransaction.BlockedAmounts.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.AvailableBalance), bankTransaction.AvailableBalance.ToString(CultureInfo.InvariantCulture)
				},
				{
					nameof(bankTransaction.CreditAvailableLimit), bankTransaction.CreditAvailableLimit.ToString(CultureInfo.InvariantCulture)
				}
			}
		};
	}

	internal static string BuildExternalId(BankTransaction bankTransaction)
	{
		return string.IsNullOrEmpty(bankTransaction.OperationReference)
			? $"{bankTransaction.CompletionDate:yyyyMMdd}-{bankTransaction.CompletionHour:HHmmss}-{bankTransaction.DebitAmount}-{bankTransaction.CreditAmount}"
			: bankTransaction.OperationReference;
	}

	internal static GemCategory CalculateCategory(BankTransaction bankTransaction)
	{
		if (bankTransaction.CreditAmount > 0)
		{
			string details = bankTransaction.Details?.Trim() ?? string.Empty;

			return details.StartsWith("Dobanda", StringComparison.OrdinalIgnoreCase)
				? GemCategory.Gain
				: GemCategory.Deposit;
		}

		if (bankTransaction.DebitAmount > 0)
			return GemCategory.Withdrawal;

		return GemCategory.Unknown;
	}
}