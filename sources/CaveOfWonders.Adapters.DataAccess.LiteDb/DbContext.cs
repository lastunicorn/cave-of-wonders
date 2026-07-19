using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using LiteDB;
using System.Data.Common;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

public sealed class DbContext : IDisposable
{
	private readonly LiteDatabase db;
	private ILiteCollection<ExchangeRateDbEntity> exchangeRates;
	private ILiteCollection<PotDbEntity> pots;
	private ILiteCollection<GemDbEntity> gems;

	internal ExchangeRateTracker ExchangeRateTracker { get; } = new();

	internal ILiteCollection<ExchangeRateDbEntity> ExchangeRates
	{
		get
		{
			if (exchangeRates == null)
			{
				exchangeRates = db.GetCollection<ExchangeRateDbEntity>("exchange_rates");
				exchangeRates.EnsureIndex(static x => x.Date);
			}

			return exchangeRates;
		}
	}

	internal ILiteCollection<PotDbEntity> Pots
	{
		get
		{
			if (pots == null)
			{
				pots = db.GetCollection<PotDbEntity>("pots");
				pots.EnsureIndex(static x => x.Name);
			}

			return pots;
		}
	}

	internal ILiteCollection<GemDbEntity> Gems
	{
		get
		{
			gems ??= db.GetCollection<GemDbEntity>("gems");
			return gems;
		}
	}

	public DbContext(string connectionString)
	{
		DbConnectionStringBuilder connectionStringBuilder = new()
		{
			ConnectionString = connectionString
		};

		string filePath = connectionStringBuilder.TryGetValue("Data Source", out object value)
			? value as string
			: null;

		string databaseDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(filePath, AppContext.BaseDirectory));

		if (!Directory.Exists(databaseDirectoryPath))
			Directory.CreateDirectory(databaseDirectoryPath);

		// Each DbContext gets its own BsonMapper instead of relying on the default
		// LiteDatabase behavior of falling back to the shared, static BsonMapper.Global.
		// That shared mapper's type-metadata cache isn't safe under concurrent first-time
		// access from multiple LiteDatabase instances (e.g. parallel test runs), which can
		// corrupt reads for types like DateOnly and throw ArgumentOutOfRangeException.
		db = new LiteDatabase(filePath, new BsonMapper());
	}

	public void SaveChanges()
	{
		ExchangeRateTracker.SaveChanges(ExchangeRates);
	}

	public void Dispose()
	{
		db?.Dispose();
	}
}