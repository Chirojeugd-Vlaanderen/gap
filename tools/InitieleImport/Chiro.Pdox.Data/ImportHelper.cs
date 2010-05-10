using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
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

			return new PersoonsAdresInfo
			          	{
		                		StraatNaamNaam = straat,
		                		HuisNr = nr,
		                		Bus = bus,
			                	PostNr = int.Parse(postNr),
			                	WoonPlaatsNaam = gemeente,
						AdresType = type
			        	};

		}
	}
}
