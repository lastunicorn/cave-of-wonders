// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;
using ExcelDataReader;
using System.Data;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;

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

                DateTime date = Convert.ToDateTime(dateRaw, CultureInfo.InvariantCulture);

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