namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public interface ICpiImportExportFactory
{
	ICpiImportExport Create(Type type);
}