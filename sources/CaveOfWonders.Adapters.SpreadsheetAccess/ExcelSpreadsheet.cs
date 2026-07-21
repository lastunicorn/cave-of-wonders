using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;
using ExcelDataReader;
using System.Data;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.SpreadsheetAccess;

public sealed class ExcelSpreadsheet : IExcelSpreadsheet
{
	private readonly IExcelDataReader reader;

	public ExcelSpreadsheet(string filePath)
	{
		FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
		reader = ExcelReaderFactory.CreateReader(stream);
	}

	public IEnumerable<SheetValue> Read(IEnumerable<SheetMapping> sheetDescriptors)
	{
		return sheetDescriptors
			.SelectMany(x =>
			{
				DataTable dataTable = GetDataTable(x.Name);
				return ReadSheet(dataTable, x);
			});
	}

	private DataTable GetDataTable(string sheetName)
	{
		ExcelDataSetConfiguration configuration = new()
		{
			ConfigureDataTable = (_) => new ExcelDataTableConfiguration
			{
				UseHeaderRow = true
			}
		};

		DataSet dataSet = reader.AsDataSet(configuration);
		DataTable dataTable = dataSet.Tables[sheetName];

		if (dataTable == null)
			throw new SheetsException($"Sheet '{sheetName}' not found.");

		return dataTable;
	}

	private static IEnumerable<SheetValue> ReadSheet(DataTable dataTable, SheetMapping sheetDescriptor)
	{
		foreach (DataRow row in dataTable.Rows)
		{
			foreach (ColumnMappings columnDescriptor in sheetDescriptor.ColumnDescriptors)
			{
				object valueRaw = row[columnDescriptor.Index];

				if (valueRaw == DBNull.Value)
					continue;

				decimal value = Convert.ToDecimal(valueRaw, CultureInfo.InvariantCulture);

				object dateRaw = row[columnDescriptor.DateIndex];

				if (dateRaw == DBNull.Value)
					throw new SheetsException($"Date value missing in column {columnDescriptor.DateIndex} for value in column {columnDescriptor.Index} ({value}) in sheet '{sheetDescriptor.Name}'.");

				DateOnly date = DateOnly.FromDateTime(Convert.ToDateTime(dateRaw, CultureInfo.InvariantCulture));

				yield return new SheetValue
				{
					Key = columnDescriptor.Key,
					Date = date,
					Value = value
				};
			}
		}
	}

	public void Dispose()
	{
		reader?.Dispose();
	}
}