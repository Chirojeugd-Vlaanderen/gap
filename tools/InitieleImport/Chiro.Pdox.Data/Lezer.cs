using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace Chiro.Pdox.Data
{
	/// <summary>
	/// Klasse om data te lezen uit een e-mailbestand van het oude chirogroepprogramma
	/// </summary>
	public class Lezer
	{
		private string _connectionString;

		/// <summary>
		/// Creeert een nieuwe lezer
		/// </summary>
		/// <param name="path">pad van de directory met daarin de paradoxtabellen</param>
		public Lezer(string path)
		{
			var builder = new StringBuilder("");

			builder.Append(@"Provider=Microsoft.Jet.OLEDB.4.0;");
			builder.Append(@"Extended Properties=Paradox 5.x;");
			builder.Append(String.Format("Data Source={0};", path));

			_connectionString = builder.ToString();
		}
	}
}
