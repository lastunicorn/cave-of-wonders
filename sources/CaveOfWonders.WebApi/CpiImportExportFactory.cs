using DustInTheWind.CaveOfWonders.Ports.InsAccess;

namespace DustInTheWind.CaveOfWonders.WebApi;

internal class CpiImportExportFactory : ICpiImportExportFactory
{
	private readonly IServiceProvider serviceProvider;

	public CpiImportExportFactory(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	}

	public ICpiImportExport Create(Type type)
	{
		if (!type.IsAssignableTo(typeof(ICpiImportExport)))
			throw new ArgumentException($"The type '{type.FullName}' is not assignable to ICpiImportExport.", nameof(type));

		return (ICpiImportExport)serviceProvider.GetRequiredService(type);
	}
}