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
				new GemParameter
				{
					Key = nameof(bankTransaction.Details), Value = bankTransaction.Details
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.OpeningBalance), Value = bankTransaction.OpeningBalance.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.DebitTotal), Value = bankTransaction.DebitTotal.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.CreditTotal), Value = bankTransaction.CreditTotal.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.FinalBalance), Value = bankTransaction.FinalBalance.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.BlockedAmounts), Value = bankTransaction.BlockedAmounts.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.AvailableBalance), Value = bankTransaction.AvailableBalance.ToString(CultureInfo.InvariantCulture)
				},
				new GemParameter
				{
					Key = nameof(bankTransaction.CreditAvailableLimit), Value = bankTransaction.CreditAvailableLimit.ToString(CultureInfo.InvariantCulture)
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