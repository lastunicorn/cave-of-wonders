using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;

internal static class ColumnDescriptorExtensions
{
	public static ColumnMappings ToColumnDescriptor(this JColumnMapping jColumnDescriptor)
	{
		return new ColumnMappings
		{
			Index = jColumnDescriptor.Index,
			DateIndex = jColumnDescriptor.DateIndex,
			Key = jColumnDescriptor.PotId
		};
	}

	public static SheetMapping ToSheetDescriptor(this JSheetMapping jSheetDescriptor)
	{
		return new SheetMapping
		{
			Name = jSheetDescriptor.Name,
			ColumnDescriptors = jSheetDescriptor.Columns?
				.Select(x => x.ToColumnDescriptor())
				.ToList()
		};
	}
}