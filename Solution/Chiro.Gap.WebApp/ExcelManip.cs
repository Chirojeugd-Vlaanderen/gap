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
	public class ExcelManip
	{
		public MemoryStream ExcelTabel<T>(IEnumerable<T> rows, params Func<T, object>[] cols)
		{
			var result = new MemoryStream();

			CreateSpreadsheetWorkbook(result);

			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(result, true))
			{
				WriteRows(spreadSheet, rows, cols);
			}

			return result;
		}

		public void WriteRows<T>(string docName, IEnumerable<T> rows, params Func<T, object>[] cols)
		{
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
			{
				WriteRows(spreadSheet, rows, cols);
			}
		}

		public void WriteRows<T>(SpreadsheetDocument spreadSheet, IEnumerable<T> rows, params Func<T, object>[] cols)
		{
			uint rowIndex = 1;

			foreach (var rij in rows)
			{
				int colIndex = 1;

				foreach (var selector in cols)
				{
					//InsertText(spreadSheet, selector(rij).ToString(), colIndex, rowIndex);
					var inhoud = selector(rij);

					if (inhoud != null)
					{
						if (inhoud.GetType() == typeof (int))
						{
							InsertNumber(spreadSheet, (double) (int) inhoud, colIndex, rowIndex);
						}
						else if (inhoud.GetType() == typeof (double))
						{
							InsertNumber(spreadSheet, (double) inhoud, colIndex, rowIndex);
						}
						else if (inhoud.GetType() == typeof (DateTime))
						{
							InsertDate(spreadSheet, (DateTime) inhoud, colIndex, rowIndex);
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


		// Given a document name and text, 
		// finds the first worksheet and writes the text to cell (colNr, rowNr) of the new worksheet.
		// (colNr, rowNr are 1-based)
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

		// Given a document name and int, 
		// finds the first worksheet and writes the text to cell (colNr, rowNr) of the new worksheet.
		// (colNr, rowNr are 1-based)
		public void InsertNumber(SpreadsheetDocument spreadSheet, double getal, int colNr, uint rowNr)
		{
			string column = colNumToName(colNr);
			WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
			Cell cell = InsertCellInWorksheet(column, rowNr, worksheetPart);
			cell.CellValue = new CellValue(getal.ToString(System.Globalization.CultureInfo.InvariantCulture));
			cell.DataType = new EnumValue<CellValues>(CellValues.Number);
			worksheetPart.Worksheet.Save();
		}

		// Given a document name and int, 
		// finds the first worksheet and writes the date to cell (colNr, rowNr) of the new worksheet.
		// (colNr, rowNr are 1-based)
		public void InsertDate(SpreadsheetDocument spreadSheet, DateTime datum, int colNr, uint rowNr)
		{
			string column = colNumToName(colNr);
			WorksheetPart worksheetPart = spreadSheet.WorkbookPart.WorksheetParts.First();
			Cell cell = InsertCellInWorksheet(column, rowNr, worksheetPart);

			//cell.StyleIndex = (UInt32Value) 1U;
			cell.CellValue = new CellValue(datum.ToOADate().ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.Date);

			worksheetPart.Worksheet.Save();
		}


		// Given a document name and text, 
		// inserts a new worksheet and writes the text to cell (colNr, rowNr) of the new worksheet.
		// (colNr, rowNr are 1-based)
		public void InsertNumber(string docName, double getal, int colNr, uint rowNr)
		{
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
			{
				InsertNumber(spreadSheet, getal, colNr, rowNr);
			}
		}

		// Given a document name and text, 
		// inserts a new worksheet and writes the text to cell (colNr, rowNr) of the new worksheet.
		// (colNr, rowNr are 1-based)
		public void InsertText(string docName, string text, int colNr, uint rowNr)
		{
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
			{
				InsertText(spreadSheet, text, colNr, rowNr);
			}
		}


		// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
		// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
		private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
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

		// Given a WorkbookPart, inserts a new worksheet.
		private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
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


		// Given a column name, a rowNr index, and a WorksheetPart, inserts a cell into the worksheet. 
		// If the cell already exists, returns it. 
		private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
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

		public void CreateSpreadsheetWorkbook(string filepath)
		{
			// Create a spreadsheet document by supplying the filepath.
			// By default, AutoSave = true, Editable = true, and Type = xlsx.
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

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

		public void CreateSpreadsheetWorkbook(MemoryStream stream)
		{
			// Create a spreadsheet document by supplying the filepath.
			// By default, AutoSave = true, Editable = true, and Type = xlsx.
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

			//// WoorkBookStylesPart voor stijl datums
			//WorkbookStylesPart workbookStylesPart1 = workbookpart.AddNewPart<WorkbookStylesPart>("rId3");

			//Stylesheet stylesheet1 = new Stylesheet();

			//CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)1U };

			//cellFormats1.Count = (UInt32Value)2U;

			//CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)14U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true };
			//cellFormats1.Append(cellFormat1);

			//stylesheet1.CellFormats = cellFormats1;

			//workbookStylesPart1.Stylesheet = stylesheet1;

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
