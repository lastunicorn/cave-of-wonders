using System.Globalization;
using System.Runtime.CompilerServices;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.QuanloopAccess;
using DustInTheWind.Quanloop.Toolkit;

namespace DustInTheWind.CaveOfWonders.Adapters.QuanloopAccess;

public class QuanloopService : IQuanloopService
{
	public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		StatementDocument statementDocument = await StatementDocument.LoadFromFileAsync(filePath, cancellationToken);

		foreach (TransactionRecord transactionRecord in statementDocument)
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
			ExternalId = BuildExternalId(transactionRecord),
			Date = transactionRecord.Date.ToDateTime(TimeOnly.MinValue),
			Amount = Math.Abs(transactionRecord.Amount),
			Category = CalculateCategory(transactionRecord),
			Description = transactionRecord.Description,
			Parameters =
			{
				new GemParameter
				{
					Key = nameof(transactionRecord.Counterpart),
					Value = transactionRecord.Counterpart
				},
				new GemParameter
				{
					Key = nameof(transactionRecord.Account),
					Value = transactionRecord.Account
				},
				new GemParameter
				{
					Key = nameof(transactionRecord.BicSwift),
					Value = transactionRecord.BicSwift
				},
				new GemParameter
				{
					Key = nameof(transactionRecord.Balance),
					Value = transactionRecord.Balance.ToString(CultureInfo.InvariantCulture)
				}
			}
		};
	}

	internal static string BuildExternalId(TransactionRecord transactionRecord)
	{
		return $"{transactionRecord.Date:yyyyMMdd}-{transactionRecord.Amount.ToString(CultureInfo.InvariantCulture)}-{transactionRecord.Description}";
	}

	internal static GemCategory CalculateCategory(TransactionRecord transactionRecord)
	{
		string description = transactionRecord.Description?.Trim() ?? string.Empty;

		if (description.StartsWith("Interest revenue", StringComparison.OrdinalIgnoreCase)) return GemCategory.Gain;
		if (description.StartsWith("Cashback", StringComparison.OrdinalIgnoreCase)) return GemCategory.Bonus;
		if (description.StartsWith("Sign-up bonus", StringComparison.OrdinalIgnoreCase)) return GemCategory.Bonus;

		if (transactionRecord.Amount < 0) return GemCategory.Withdrawal;
		if (transactionRecord.Amount > 0) return GemCategory.Deposit;

		return GemCategory.Unknown;
	}
}
