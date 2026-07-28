namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public interface IDatabaseConfiguration
{
	string ConnectionString { get; }
}