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
            .SelectMany(x => ReadSheet(reader, x));
    }

    private IEnumerable<SheetValue> ReadSheet(IExcelDataReader reader, SheetMapping sheetDescriptor)
    {
        DataSet dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true // Set to false if no headers
            }
        });

        DataTable sheet = dataSet.Tables[sheetDescriptor.Name];

        if (sheet == null)
            throw new Exception($"Sheet '{sheetDescriptor.Name}' not found.");


        foreach (DataRow row in sheet.Rows)
        {
            foreach (ColumnMappings columnDescriptor in sheetDescriptor.ColumnDescriptors)
            {
                yield return new SheetValue
                {
                    Key = columnDescriptor.Key,
                    Date = row.GetDate(columnDescriptor.DateIndex),
                    Value = row.GetDecimal(columnDescriptor.Index)
                };
            }
        }
    }

    public void Dispose()
    {
        reader?.Dispose();
    }
}
