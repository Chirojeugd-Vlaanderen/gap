// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model om adres te verwijderen.  Bevat een adres met daaraan
	/// gekoppeld de bewoners van wie het adres mogelijk vervalt
	/// </summary>
	/// <remarks>Je zou je kunnen afvragen waarom dit model opgebowd
	/// wordt op basis van AdresID en GelieerdePersoonID, en niet via
	/// enkel PersoonsAdresID.  Het heeft ermee te maken dat je toch
	/// steeds de oorspronkelijke GelieerdePersoonsID zal moeten onthouden,
	/// omdat je na het verwijderen van het persoonsadres wel terug moet
	/// kunnen redirecten naar de juiste persoonsinfo.
	/// Je moet dus 2 ID's bewaren, en dat kunnen dus net zo goed
	/// GelieerdePersoonID en AdresID zijn.</remarks>
	public class AdresVerwijderenModel : MasterViewModel
	{
		/// <summary>
		/// ID van GelieerdePersoon met het te verwijderen adres.
		/// Wordt bewaard om achteraf terug naar de details van de
		/// aanvrager te kunnen redirecten.
		/// </summary>
		public int AanvragerID { get; set; }

		/// <summary>
		/// AdresMetBewoners bevat te verwijderen adres,
		/// met daaraan gekoppeld alle bewoners die de aangelogde gebruiker
		/// mag zien.
		/// </summary>
		public GezinInfo Adres { get; set; }

		/// <summary>
		/// Het lijstje PersoonIDs bevat de ID's van de personen van wie
		/// het adres uiteindelijk zal vervallen.
		/// </summary>
		public List<int> PersoonIDs { get; set; }

		/// <summary>
		/// Saaie standaardconstructor
		/// </summary>
		public AdresVerwijderenModel()
		{
			PersoonIDs = new List<int>();
		}
	}
}
