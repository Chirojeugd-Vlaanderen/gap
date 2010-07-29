﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using tar_cs;

namespace Chiro.Pdox.Data
{
	/// <summary>
	/// Klasse voor de tricky dngen van Kipadmin (plaats van e-mailbestanden op het netwerk e.d.)
	/// </summary>
	public class KipdorpHelper
	{
		/// <summary>
		/// Converteert een stamnummer naar de bestandsnaam van het overeenkomstig aansluitingsbestand
		/// (zonder extentie, bijv: mg /0113 -> mg_0113, leg/0100 -> leg0100)
		/// </summary>
		/// <param name="stamnr">Stamnummer</param>
		/// <returns>Bestandsnaam (zonder extensie) van het aansluitingsbestand</returns>
		public string StamNrNaarBestand(string stamnr)
		{
			return stamnr.Substring(0, 3).Replace(' ', '_') + stamnr.Substring(4, 4);
		}

		/// <summary>
		/// Bepaalt het recentste aansluitingsbestand van de groep met stamnummer <paramref name="stamnr"/> uit de
		/// 'aansluitingenmap'
		/// </summary>
		/// <param name="stamnr">Stamnummer van de groep</param>
		/// <returns>Recentste aansluitingsbestand van groep met stamnummer <paramref name="stamnr"/>.
		/// Lege string als geen bestand gevonden.</returns>
		public string RecentsteAansluitingsBestand(string stamnr)
		{
			string directory = String.Format(@"{0}/{1}", Properties.Settings.Default.AansluitingsMap, StamNrNaarBestand(stamnr));

			var di = new DirectoryInfo(directory);
			FileInfo[] files;

			try
			{
				files = di.GetFiles("*.a*");
			}
			catch (DirectoryNotFoundException)
			{
				// groep heeft geen aansluitingsbestand
				return String.Empty;
			}

			var fileQuery = from file in files
			                where !file.Name.EndsWith("adr", true, null)
			                orderby file.LastWriteTime descending
			                select (file.DirectoryName ?? String.Empty).Replace('\\','/') + @"/" + file.Name;

			return fileQuery.FirstOrDefault();
		}

		/// <summary>
		/// Pakt aansluitingsbestand met naam <paramref name="bestandsNaam"/> uit, ergens ver weg verborgen onder
		/// <paramref name="uitPakPath"/>
		/// </summary>
		/// <param name="bestandsNaam">Naam uit te pakken bestand</param>
		/// <param name="uitPakPath">Naam van path waarin uitgepakt mag worden</param>
		/// <returns>Het effectieve bad van waar de uitgepakte bestanden gevonden kunnen worden.  Dit is een file
		/// ergens diep onder <paramref name="uitPakPath"/>, en dat heeft te maken met een brolimplementatie in
		/// kipadmin (mijn fout).</returns>
		public string Uitpakken(string bestandsNaam, string uitPakPath)
		{
			using (var stream = File.OpenRead(bestandsNaam))
			{
				using (var decompressed = new GZipStream(stream, CompressionMode.Decompress))
				{
					var reader = new TarReader(decompressed);


					try
					{
						reader.ReadToEnd(uitPakPath);
					}
					catch (TarException ex)
					{
						if (String.Compare(ex.Message, "Broken Archive", true) != 0)
						{
							// We verwachten een 'broken archive'.  Iets anders wordt
							// opnieuw gethrowd.

							throw;
						}
					}

					return String.Format("{0}/{1}", uitPakPath, Path.GetDirectoryName(reader.FileInfo.FileName));

				}
			}
		}
	}
}
