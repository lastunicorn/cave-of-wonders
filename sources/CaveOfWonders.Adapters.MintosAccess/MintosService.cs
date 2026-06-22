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
        if (paymentType == PaymentType.Deposits)
            return GemCategory.Deposit;

        if (paymentType == PaymentType.Investment)
            return GemCategory.Internal;

        if (paymentType == PaymentType.Principal)
            return GemCategory.Internal;

        if (paymentType == PaymentType.Interest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.LateFee)
            return GemCategory.Gain;

        if (paymentType == PaymentType.SecondaryMarketTransaction)
            return GemCategory.Internal;

        if (paymentType == PaymentType.LoanRepurchasePrincipal)
            return GemCategory.Internal;

        if (paymentType == PaymentType.LoanRepurchaseInterest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.TransitRebuyDelayedInterest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.PendingPaymentsInterest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.TaxWithholding)
            return GemCategory.Loss;

        if (paymentType == PaymentType.CashOutShareIncomeToSeller)
            return GemCategory.Internal;

        if (paymentType == PaymentType.MintosCoreFee)
            return GemCategory.Loss;

        if (paymentType == PaymentType.MintosCustomLoansFee)
            return GemCategory.Loss;

        if (paymentType == PaymentType.Withdrawal)
            return GemCategory.Withdrawal;

        return GemCategory.Unknown;
    }
}