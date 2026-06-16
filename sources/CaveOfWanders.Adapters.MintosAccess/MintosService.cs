using DustInTheWind.CaveOfWanders.Ports.MintosAccess;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.Mintos.Toolkit;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWanders.Adapters.MintosAccess;

public class MintosService : IMintosService
{
    public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        StatementDocument statementDocument = await StatementDocument.LoadFromFileAsync(filePath);

        foreach (TransactionRecord transactionRecord in statementDocument)
        {
            GemCategory gemCategory = CalculateCategory(transactionRecord.PaymentType);
            
            if (gemCategory == GemCategory.Unknown)
                continue;
            
            yield return new Gem
            {
                Date = transactionRecord.Date,
                Amount = transactionRecord.Turnover,
                Category = gemCategory,
                Parameters =
                {
                    { nameof(transactionRecord.TransactionId), transactionRecord.TransactionId },
                    { nameof(transactionRecord.Details), transactionRecord.Details },
                    { nameof(transactionRecord.Balance), transactionRecord.Balance.ToString(CultureInfo.InvariantCulture) },
                    { nameof(transactionRecord.Currency), transactionRecord.Currency?.ToString() },
                    { nameof(transactionRecord.PaymentType), transactionRecord.PaymentType.ToString() }
                }
            };
        }
    }

    private GemCategory CalculateCategory(PaymentType paymentType)
    {
        if (paymentType == PaymentType.CashOutShareIncomeToSeller)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.TransitRebuyDelayedInterest)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.Deposits)
            return GemCategory.Deposit;

        if (paymentType == PaymentType.Interest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.LoanRepurchaseInterest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.PendingPaymentsInterest)
            return GemCategory.Gain;

        if (paymentType == PaymentType.Investment)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.LateFee)
            return GemCategory.Gain;

        if (paymentType == PaymentType.MintosCoreFee)
            return GemCategory.Loss;

        if (paymentType == PaymentType.MintosCustomLoansFee)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.Principal)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.LoanRepurchasePrincipal)
            return GemCategory.Unknown;

        if (paymentType == PaymentType.SecondaryMarketTransaction)
            return GemCategory.Unknown;
        
        if (paymentType == PaymentType.TaxWithholding)
            return GemCategory.Loss;
        
        if (paymentType == PaymentType.Withdrawal)
            return GemCategory.Withdrawal;
        
        return GemCategory.Unknown;
    }
}