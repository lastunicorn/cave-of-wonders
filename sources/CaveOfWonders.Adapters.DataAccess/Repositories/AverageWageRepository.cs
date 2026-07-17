using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Repositories;

public class AverageWageRepository : IAverageWageRepository
{
    private readonly Database database;

    public AverageWageRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<AverageWage> GetAsync(int year, CancellationToken cancellationToken)
    {
        AverageWage averageWage = database.AverageWages
            .FirstOrDefault(x => x.Year == year);

        return Task.FromResult(averageWage);
    }

    public async IAsyncEnumerable<AverageWage> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IEnumerable<AverageWage> averageWages = database.AverageWages;

        foreach (AverageWage averageWage in averageWages)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return averageWage;
        }
    }

    public void Add(AverageWage averageWage)
    {
        if (averageWage == null)
            throw new ArgumentNullException(nameof(averageWage));

        database.AverageWages.Add(averageWage);
    }

    public void Delete(AverageWage averageWage)
    {
        if (averageWage == null)
            throw new ArgumentNullException(nameof(averageWage));

        database.AverageWages.Remove(averageWage);
    }
}