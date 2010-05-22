using System.Collections.Generic;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System;

namespace Chiro.Gap.WebApp.ExcelExporter
{
	public class ExcelExporter<T>
	{

		/// <summary>
		/// Exports the data in a ListData object to a spreadsheet in Excel 2007 (.xlsx) format and returns it in a MemoryStream
		/// </summary>
		/// <param name="data">A ListData object containing the data to write to a spreadsheet in Excel 2007 format</param>
		/// <param name="fields">Lambda-expressies die bepalen welke kolommen er in welke volgorde getoond moeten worden</param>
		/// <returns>A MemoryStream containing the spreadsheet in Excel 2007 (.xlsx) format</returns>
		public static MemoryStream Export(IEnumerable<T> data, params Func<T, object>[] fields)
		{

			MemoryStream ms = new MemoryStream();

			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
			{
				WorkbookPart workbookPart = spreadSheet.AddWorkbookPart();
				workbookPart.Workbook = new Workbook();

				WorksheetPart workSheetPart = InsertWorksheet(spreadSheet.WorkbookPart, "Zoekresultaten");
				SheetData sheet = workSheetPart.Worksheet.GetFirstChild<SheetData>();

				//// Create the header rows
				//Row header = new Row();
				//header.RowIndex = Convert.ToUInt32(1);
				//for (int fieldIndex = 0;
				//     (fieldIndex
				//      <= (data.Fields.Count - 1));
				//     fieldIndex++)
				//{
				//        SPFieldInfo fieldInfo = data.Fields(fieldIndex);
				//        Cell headerCell = CreateCell((fieldIndex + 1), 1, fieldInfo.FieldDisplayName, numberFormatId, dateTimeFormatId);
				//        header.AppendChild(headerCell);
				//}
				//sheet.AppendChild(header);


				// Create the value rows
				int rowIndex = 0;

				foreach (T dataRow in data)
				{
					Row valueRow = new Row();
					for (int colIndex = 0;
					     (colIndex
					      < fields.Length);
					     colIndex++)
					{
						Cell cell = CreateCell((colIndex + 1), (rowIndex + 2), fields[colIndex](dataRow));
						valueRow.AppendChild(cell);
					}
					sheet.AppendChild(valueRow);

					++rowIndex;
				}
				spreadSheet.WorkbookPart.Workbook.Save();
				spreadSheet.Close();
			}
			return ms;
		}

		private static Cell CreateCell(int columnIndex, int rowIndex, object value)
		{
			Cell cell;

			cell = CreateTextCell(value);
			cell.CellReference = (getColumnName(columnIndex) + rowIndex);
			return cell;
		}

		private static Cell CreateTextCell(object value)
		{
			Cell c = new Cell();
			c.DataType = CellValues.InlineString;
			InlineString iString = new InlineString();
			Text t = new Text();
			t.Text = value.ToString();
			iString.Append(t);
			c.Append(iString);
			return c;
		}

		private static Cell CreateNumberCell(object value)
		{
			Cell c = new Cell();
			CellValue v = new CellValue();
			v.Text = value.ToString();
			c.Append(v);
			// c.StyleIndex = 1
			return c;
		}

		private static Cell CreateDateTimeCell(DateTime value)
		{
			Cell c = new Cell();
			CellValue v = new CellValue();
			v.Text = value.ToShortDateString();
			// v.Text = value.ToOADate().ToString
			c.Append(v);
			// c.StyleIndex = 2
			return c;
		}

		private static Stylesheet GenerateStyleSheet()
		{
			Stylesheet stylesheet = new Stylesheet();
			CellFormats cfs = new CellFormats();
			CellFormat datetimeFormat = new CellFormat();
			datetimeFormat.NumberFormatId = 14;
			// short date
			datetimeFormat.ApplyNumberFormat = true;
			datetimeFormat.FontId = 0;
			datetimeFormat.FillId = 0;
			datetimeFormat.BorderId = 0;
			datetimeFormat.FormatId = 0;
			cfs.Append(datetimeFormat);
			cfs.Count = 1;
			stylesheet.Append(cfs);

			return stylesheet;
		}

		private static CellValues getDataTypeForFieldType(string fieldType)
		{
			switch (fieldType)
			{
				case "Number":
					return CellValues.Number;
					break;
				case "DateTime":
					return CellValues.Date;
					break;
				default:
					return CellValues.InlineString;
					break;
			}
		}

		private static string getColumnName(int columnIndex)
		{
			int dividend = columnIndex;
			string columnName = String.Empty;
			int modifier;
			while ((dividend > 0))
			{
				modifier = ((dividend - 1)
					    % 26);
				columnName = (Convert.ToChar((65 + modifier)).ToString() + columnName);
				dividend = Convert.ToInt32(((dividend - modifier)
						/ 26));
			}
			return columnName;
		}

		private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
		{
			//  Add a new worksheet part to the workbook.
			WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
			newWorksheetPart.Worksheet = new Worksheet(new SheetData());
			newWorksheetPart.Worksheet.Save();
			Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
			string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);
			//  Add the new worksheet and associate it with the workbook.
			Sheet sheet = new Sheet();
			sheet.Id = relationshipId;
			sheet.SheetId = 1;
			sheet.Name = sheetName;
			sheets.Append(sheet);
			workbookPart.Workbook.Save();
			return newWorksheetPart;
		}
	}
}