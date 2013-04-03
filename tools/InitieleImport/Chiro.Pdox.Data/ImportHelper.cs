/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Pdox.Data.Properties;

namespace Chiro.Pdox.Data
{
	public class ImportHelper
	{
		/// <summary>
		/// Probeert een string 'straat nr bus' op te splitsen in zijn componenten.
		/// </summary>
		/// <param name="straatNr">String met straat, nummer en bus</param>
		/// <param name="straat">Enkel de straatnaam</param>
		/// <param name="nr">Enkel het nummer</param>
		/// <param name="bus">Enkel de bus</param>
		/// <remarks>Deze method gaat ervan uit dat er geen cijfers in de straatnaam zitten, en dan
		/// nog is het resultaat onbetrouwbaar :)</remarks>
		public void SpitsStraatNr(string straatNr, out string straat, out int? nr, out string bus)
		{
			var patroon = new Regex("^(?<straat>[^0-9]*)(?<nr>[0-9]*)?(?<bus>[^0-9].*)?");

			var match = patroon.Match(straatNr);
			if (!match.Success)
			{
				throw new ArgumentException(Resources.OngeldigeAdresLijn, "straatNr");
			}
			else
			{
				straat = match.Groups["straat"].ToString().Trim();

				bool geenNr = (match.Groups["nr"].ToString() == String.Empty);
				bool geenBus = (match.Groups["bus"].ToString() == String.Empty);

				nr =  geenNr ? null : (int?)(int.Parse(match.Groups["nr"].ToString()));
				bus = geenBus ? null : match.Groups["bus"].ToString().Trim();
			}
		}

		/// <summary>
		/// Probeert een telefoonnr te formatteren volgens Barts huisstijlregels.
		/// </summary>
		/// <param name="zootje">Telefoonnr in eender welke vorm</param>
		/// <returns>Geformatteerd telefoonnummer, of null als het niet lukte</returns>
		public string FormatteerTelefoonNr(string zootje)
		{
			string cijfers = new string((from c in zootje where Char.IsDigit(c) select c).ToArray());

			if (cijfers == String.Empty)
			{
				return null;
			}

			if (cijfers.First() != '0')
			{
				cijfers = '0' + cijfers;
			}

			Regex regex;
			const string VASTELIJNEXPRESSIE = "^(0[2349]|0[15678].)([0-9]{2,3})([0-9]{2})([0-9]{2})$";
			const string GSMEXPRESSIE = "^(0[1-9]{3})([0-9]{2})([0-9]{2})([0-9]{2})$";

			regex = cijfers.Length == 9 ? new Regex(VASTELIJNEXPRESSIE) : new Regex(GSMEXPRESSIE);

			var match = regex.Match(cijfers);

			if (!match.Success)
			{
				return null;
			}

			string zone = match.Groups[1].ToString();
			string net = match.Groups[2].ToString();
			string ab1 = match.Groups[3].ToString();
			string ab2 = match.Groups[4].ToString();

			return String.Format("{0}-{1} {2} {3}", zone, net, ab1, ab2);
		}
		
		/// <summary>
		/// Creeert een adres van de typische adresgegevens uit het oude Chirogroepprogramma.  Als het adres
		/// geen Belgisch adres lijkt, of het postnummer is geen int, dan is het resultaat null.
		/// </summary>
		/// <param name="straatNr">Combinatie straat, nummer, bus</param>
		/// <param name="postNr">Postnummer</param>
		/// <param name="gemeente">Woonplaats</param>
		/// <param name="land">Land</param>
		/// <param name="type">Adrestype dat het adres moet krijgen</param>
		/// <returns>PersoonsAdresInfo met de adresgegevens</returns>
		public PersoonsAdresInfo MaakAdresInfo(
			string straatNr, 
			string postNr, 
			string gemeente, 
			string land, 
			AdresTypeEnum type)
		{
			if (!Regex.IsMatch(postNr, "[0-9]+"))
			{
				return null;
			}
			
			if (land.Trim() != String.Empty && !land.Trim().StartsWith("BE", true, null))
			{
				return null;
			}

			string straat;
			int? nr;
			string bus;

			SpitsStraatNr(straatNr, out straat, out nr, out bus);

			PersoonsAdresInfo resultaat = null;

			try
			{
				resultaat = new PersoonsAdresInfo
			          	{
		                		StraatNaamNaam = straat,
		                		HuisNr = nr,
		                		Bus = bus,
			                	PostNr = int.Parse(postNr),
			                	WoonPlaatsNaam = gemeente,
						AdresType = type
			        	};
			}
			catch (Exception)
			{
				Console.WriteLine(Resources.FormatException);
			}
			return resultaat;

		}
	}
}
