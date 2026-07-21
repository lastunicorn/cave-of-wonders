namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public class CpiImportExportPool
{
	private readonly ICpiImportExportFactory factory;
	private readonly IDictionary<Guid, CpiImportExportDescriptor> descriptors = new Dictionary<Guid, CpiImportExportDescriptor>();

	public CpiImportExportPool(ICpiImportExportFactory factory)
	{
		this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	public void Add(CpiImportExportDescriptor descriptor)
	{
		if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
		descriptors.Add(descriptor.Id, descriptor);
	}

	public ICpiImportExport Get(Guid id)
	{
		if (!descriptors.TryGetValue(id, out CpiImportExportDescriptor descriptor))
			throw new ArgumentException($"No CPI import/export found for the given ID: {id}", nameof(id));

		return factory.Create(descriptor.Type);
	}
}