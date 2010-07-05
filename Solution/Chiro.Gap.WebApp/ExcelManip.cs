// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Methods voor manipulatie Excelstreams
	/// </summary>
	public partial class ExcelManip
	{
		/// <summary>
		/// Genereer een Exceldocument op basis van een rij objecten van type <paramref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Type van de objecten</typeparam>
		/// <param name="rows">Objecten die in een rij terecht moeten komen</param>
		/// <param name="cols">(param)array van lambda-expressies, die de kolommen van het document bepaalt</param>
		/// <returns>Een memorystream met daarin het Exceldocument</returns>
		public MemoryStream ExcelTabel<T>(IEnumerable<T> rows, params Func<T, object>[] cols)
		{
			var result = new MemoryStream();

			// Creeer een nieuw document in de stream
			CreateSpreadsheetWorkbook(result);

			// Importeer de tabel in het document 
			using (var spreadSheet = SpreadsheetDocument.Open(result, true))
			{
				WriteRows(spreadSheet, rows, cols);
			}

			return result;
		}

		/// <summary>
		/// Maak een tabel in de eerste worksheet van een bestaand Exceldocument, op basis van een rij objecten
		/// van het type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Type van de objecten in de rij</typeparam>
		/// <param name="spreadSheet">Spreadsheet waarin de tabel moet komen</param>
		/// <param name="rows">rij objecten; elke rij is gebaseerd op een object</param>
		/// <param name="cols">(param)array van lambda-expressies, die de kolommen bepalen</param>
		public void WriteRows<T>(SpreadsheetDocument spreadSheet, IEnumerable<T> rows, params Func<T, object>[] cols)
		{
			uint rowIndex = 1;

			foreach (var rij in rows)
			{
				int colIndex = 1;

				foreach (var selector in cols)
				{
					// InsertText(spreadSheet, selector(rij).ToString(), colIndex, rowIndex);
					var inhoud = selector(rij);

					if (inhoud != null)
					{
						if (inhoud.GetType() == typeof(int))
						{
							InsertNumber(spreadSheet, (double)(int)inhoud, colIndex, rowIndex);
						}
						else if (inhoud.GetType() == typeof(double))
						{
							InsertNumber(spreadSheet, (double)inhoud, colIndex, rowIndex);
						}
						else if (inhoud.GetType() == typeof(DateTime))
						{
							InsertDate(spreadSheet, (DateTime)inhoud, colIndex, rowIndex);
						}
						else
						{
							string waarde = inhoud.ToString();
							InsertText(spreadSheet, waarde, colIndex, rowIndex);
						}
					}

					++colIndex;
				}
				++rowIndex;
			}
		}

		/// <summary>
		/// Plaatst een tekst in een gegeven (lege) cel van een spreadsheetdocument.
		/// </summary>
		/// <param name="spreadSheet">Te bewerken spreadsheet</param>
		/// <param name="text">Tekst die in de lege cel moet komen</param>
		/// <param name="colNr">Kolom van de cel (1, 2, 3,...)</param>
		/// <param name="rowNr">Rij van de cel (1, 2, 3,...)</param>
		public void InsertText(SpreadsheetDocument spreadSheet, string text, int colNr, uint rowNr)
		{
			string column = colNumToName(colNr);

			// Get the SharedStringTablePart. If it does not exist, create a new one.
			SharedStringTablePart shareStringPart;
			if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
			{
				shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
			}
			else
			{
				shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
			}

			// Insert the text into the SharedStringTablePart.
			int index = InsertSharedStringItem(text, shareStringPart);

			WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
			Cell cell = InsertCellInWorksheet(column, rowNr, worksheetPart);
			cell.CellValue = new CellValue(index.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			worksheetPart.Worksheet.Save();
		}

		/// <summary>
		/// Plaatst een getal in een gegeven (lege) cel van een spreadsheetdocument.
		/// </summary>
		/// <param name="spreadSheet">Te bewerken spreadsheet</param>
		/// <param name="getal">Te schrijven getal</param>
		/// <param name="colNr">Kolom van de cel (1, 2, 3,...)</param>
		/// <param name="rowNr">Rij van de cel (1, 2, 3,...)</param>
		public void InsertNumber(SpreadsheetDocument spreadSheet, double getal, int colNr, uint rowNr)
		{
			string column = colNumToName(colNr);
			WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
			Cell cell = InsertCellInWorksheet(column, rowNr, worksheetPart);
			cell.CellValue = new CellValue(getal.ToString(System.Globalization.CultureInfo.InvariantCulture));
			cell.DataType = new EnumValue<CellValues>(CellValues.Number);
			worksheetPart.Worksheet.Save();
		}

		/// <summary>
		/// Plaatst een datum in een gegeven (lege) cel van een spreadsheetdocument.
		/// </summary>
		/// <param name="spreadSheet">Te bewerken spreadsheet</param>
		/// <param name="date">Te schrijven datum</param>
		/// <param name="colNr">Kolom van de cel (1, 2, 3,...)</param>
		/// <param name="rowNr">Rij van de cel (1, 2, 3,...)</param>
		public void InsertDate(SpreadsheetDocument spreadSheet, DateTime date, int colNr, uint rowNr)
		{
			string column = colNumToName(colNr);
			WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
			Cell cell = InsertCellInWorksheet(column, rowNr, worksheetPart);

			cell.StyleIndex = (UInt32Value)1U;
			cell.CellValue = new CellValue(date.ToOADate().ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.Date);

			worksheetPart.Worksheet.Save();
		}

		/// <summary>
		/// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
		//  and inserts it into the SharedStringTablePart. If the item already exists, returns its index
		/// </summary>
		/// <param name="text">Tekst voor de toe te voegen sharedstring</param>
		/// <param name="shareStringPart">SharedStringPart waaraan <paramref name="text"/> toe te voegen is</param>
		/// <returns>Volgnummer van de toegevoegde shared string in het SharedStringTablePart</returns>
		private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
		{
			// If the part does not contain a SharedStringTable, create one.
			if (shareStringPart.SharedStringTable == null)
			{
				shareStringPart.SharedStringTable = new SharedStringTable();
			}

			int i = 0;

			// Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
			foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
			{
				if (item.InnerText == text)
				{
					return i;
				}

				i++;
			}

			// The text does not exist in the part. Create the SharedStringItem and return its index.
			shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
			shareStringPart.SharedStringTable.Save();

			return i;
		}

		/// <summary>
		/// Given a WorkbookPart, inserts a new worksheet.
		/// </summary>
		/// <param name="workbookPart">WorkbookPart waaraan een nieuwe worksheet toegevoegd moet worden</param>
		/// <returns>De nieuwe worksheet</returns>
        private WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
		{
			// Add a new worksheet part to the workbook.
			WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
			newWorksheetPart.Worksheet = new Worksheet(new SheetData());
			newWorksheetPart.Worksheet.Save();

			Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
			string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

			// Get a unique ID for the new sheet.
			uint sheetId = 1;
			if (sheets.Elements<Sheet>().Count() > 0)
			{
				sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
			}

			string sheetName = "Sheet" + sheetId;

			// Append the new worksheet and associate it with the workbook.
			Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
			sheets.Append(sheet);
			workbookPart.Workbook.Save();

			return newWorksheetPart;
		}

		/// <summary>
		/// Converteert een kolomnummer (1, 2, 3, ...) naar de overeenkomstige 'kolomnaam' (A, B, C,...)
		/// </summary>
		/// <param name="colNum">Te converteren kolomnummer</param>
		/// <returns>De overeenkomstige kolomnaam</returns>
		private String colNumToName(int colNum)
		{
			StringBuilder result = new StringBuilder();

			--colNum;

			int cycleNum = colNum / 26;
			int withinNum = colNum - (cycleNum * 26);

			if (cycleNum > 0)
			{
				result.Append((char)((cycleNum - 1) + 'a'));
			}
			result.Append((char)(withinNum + 'a'));
			return (result.ToString());
		}

		/// <summary>
		/// Given a column name, a rowNr index, and a WorksheetPart, inserts a cell into the worksheet. 
		/// If the cell already exists, returns it. 
		/// </summary>
		/// <param name="columnName">Naam van de kolom waarin een cel gemaakt moet worden (A, B, C,...)</param>
		/// <param name="rowIndex">Nummer van de rij waarin een cel gemaakt moet worden (1, 2, 3,...)</param>
		/// <param name="worksheetPart">Worksheetpart in hetwelke de cel gemaakt moet worden</param>
		/// <returns>De gemaakte cel.  (Of de bestaande cel, als er al een cel bestaat)</returns>
		private Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
		{
			Worksheet worksheet = worksheetPart.Worksheet;
			SheetData sheetData = worksheet.GetFirstChild<SheetData>();
			string cellReference = columnName + rowIndex;

			// If the worksheet does not contain a row with the specified row index, insert one.
			Row row;
			if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
			{
				row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
			}
			else
			{
				row = new Row() { RowIndex = rowIndex };
				sheetData.Append(row);
			}

			// If there is not a cell with the specified column name, insert one.  
			if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
			{
				return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
			}
			else
			{
				// Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
				Cell refCell = null;
				foreach (Cell cell in row.Elements<Cell>())
				{
					if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
					{
						refCell = cell;
						break;
					}
				}

				Cell newCell = new Cell() { CellReference = cellReference };
				row.InsertBefore(newCell, refCell);

				worksheet.Save();
				return newCell;
			}
		}

		/// <summary>
		/// Creeert een nieuw Exceldocument in de memorystream <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">memorystream waarin het Exceldocument gemaakt moet worden</param>
		public void CreateSpreadsheetWorkbook(MemoryStream stream)
		{
			// Create a spreadsheet document by supplying the filepath.
			// By default, AutoSave = true, Editable = true, and Type = xlsx.
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

			// WoorkBookStylesPart voor stijl datums
			WorkbookStylesPart workbookStylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
			GenerateWorkbookStylesPartContent(workbookStylesPart);

			// Add a WorksheetPart to the WorkbookPart.
			WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
			worksheetPart.Worksheet = new Worksheet(new SheetData());

			// Add Sheets to the Workbook.
			Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

			// Append a new worksheet and associate it with the workbook.
			Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
			sheets.Append(sheet);

			workbookpart.Workbook.Save();

			// Close the document.
			spreadsheetDocument.Close();
		}
	}
}
