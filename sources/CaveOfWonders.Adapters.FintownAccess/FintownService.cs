using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.FintownAccess;
using DustInTheWind.Fintown.Toolkit;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.FintownAccess;

public class FintownService : IFintownService
{
    public async IAsyncEnumerable<Gem> GetGemsAsync(string filePath, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        TransactionsDocument transactionsDocument = await TransactionsDocument.LoadFromFileAsync(filePath);

        IEnumerable<Gem> gems = transactionsDocument.Transactions
            .Select(CreateGem);
        
        foreach (Gem gem in gems)
            yield return gem;
    }

    private static Gem CreateGem(TransactionRecord transactionRecord)
    {
        Gem gem = new()
        {
            Id = Guid.NewGuid(),
            ExternalId = transactionRecord.Date.Ticks.ToString(),
            Date = transactionRecord.Date,
            Amount = transactionRecord.Amount.Value
        };

        if (transactionRecord.Description == "Investing funds")
            gem.Category = GemCategory.Internal;
        else if (transactionRecord.Description == "Deposit funds")
            gem.Category = GemCategory.Deposit;
        else if (transactionRecord.Description == "Interest Pay-Out")
            gem.Category = GemCategory.Gain;
        else
            gem.Category = GemCategory.Unknown;

        return gem;
    }
}