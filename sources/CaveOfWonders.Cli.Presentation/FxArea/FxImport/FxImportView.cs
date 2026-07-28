using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

internal class FxImportView : IView<FxImportViewModel>
{
	public void Display(FxImportViewModel result)
	{
		foreach (UpdateValueViewModel updateValueViewModel in result.Updates)
		{
			Console.WriteLine();
			Console.WriteLine($"Existing : {updateValueViewModel.Date} {updateValueViewModel.CurrencyPair} {updateValueViewModel.OldValue}");
			Console.WriteLine($"New      : {updateValueViewModel.Date} {updateValueViewModel.CurrencyPair} {updateValueViewModel.NewValue}");
		}

		foreach (DuplicateValueViewModel duplicateValueViewModel in result.Duplicates)
		{
			Console.WriteLine();
			Console.WriteLine($"Duplicate 1 : {duplicateValueViewModel.Date} {duplicateValueViewModel.CurrencyPair} {duplicateValueViewModel.Value1}");
			Console.WriteLine($"Duplicate 2 : {duplicateValueViewModel.Date} {duplicateValueViewModel.CurrencyPair} {duplicateValueViewModel.Value2}");
		}

		Console.WriteLine();

		DataGrid dataGrid = DataGridTemplate.CreateNew();
		dataGrid.Title = "Report";
		dataGrid.DisplayBorderBetweenRows = true;

		dataGrid.Columns.Add(new Column("Name")
		{
			ForegroundColor = ConsoleColor.White
		});
		dataGrid.Columns.Add(new Column("Value")
		{
			CellHorizontalAlignment = HorizontalAlignment.Right
		});
		dataGrid.Columns.Add(new Column("Comments")
		{
			ForegroundColor = ConsoleColor.DarkGray
		});

		dataGrid.HeaderRow.IsVisible = false;

		ContentRow contentRowAdded = dataGrid.Rows.Add("Added", result.AddedCount.ToStringOrEmpty("-"), "New items - added into the storage.");
		if (result.AddedCount > 0)
			contentRowAdded[1].ForegroundColor = ConsoleColor.Green;

		ContentRow contentRowUpdated = dataGrid.Rows.Add("Updated", result.ExistingUpdatedCount.ToStringOrEmpty("-"), "Existing items - values needed to be updated.");
		if (result.ExistingUpdatedCount > 0)
			contentRowUpdated[1].ForegroundColor = ConsoleColor.Green;

		dataGrid.Rows.Add("Existing", result.ExistingIdenticalCount.ToStringOrEmpty("-"), "Existing items - nothing to do.");

		if (result.NewDuplicateIdenticalCount > 0)
		{
			ContentRow contentRowSourceDuplicatesIdentical = dataGrid.Rows.Add("Source Duplicates\n(identical)", result.NewDuplicateIdenticalCount.ToStringOrEmpty("-"), "The import source provided same exchange rate multiple times.\nDuplicates were ignored.");
			contentRowSourceDuplicatesIdentical[1].ForegroundColor = ConsoleColor.DarkRed;
		}

		if (result.NewDuplicateIdenticalCount > 0)
		{
			ContentRow contentRowSourceDuplicatesDifferent = dataGrid.Rows.Add("Source Duplicates\n(different value)", result.NewDuplicateDifferentCount.ToStringOrEmpty("-"), "The import source provided same currency multiple times, but with different values.\nDuplicates were ignored.");
			contentRowSourceDuplicatesDifferent[1].ForegroundColor = ConsoleColor.DarkRed;
		}

		dataGrid.Display();
	}
}