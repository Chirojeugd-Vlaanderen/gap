// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Gap.Orm
{
	// Deze enums mogen geen datacontracts zijn; de '-Extra's' mogen niet over de lijn.
	// Zie ticket #439

	/// <summary>
	/// Enum die aangeeft welke extra's er meegeleverd kunnen worden met een groep.
	/// </summary>
	/// <remarks>Dit is bewust geen datacontract meer.  Deze enums mogen maar zo weinig mogelijk via de
	/// service geexposet worden.</remarks>
	[Flags]
	public enum GroepsExtras
	{
		Geen = 0x00,
		AlleAfdelingen = 0x01,
	}

	/// <summary>
	/// Enum die aangeeft welke gerelateerde entity's er meegeleverd kunnen worden met een afdelingsjaar.
	/// </summary>
	/// <remarks>Dit is bewust geen datacontract meer.  Deze enums mogen maar zo weinig mogelijk via de
	/// service geexposet worden.</remarks>
	[Flags]
	public enum AfdelingsJaarExtras
	{
		Geen = 0x00,
		Afdeling = 0x01,
		OfficieleAfdeling = 0x02,
		GroepsWerkJaar = 0x04,
		Leden = 0x08,
		Alles = Afdeling | OfficieleAfdeling | GroepsWerkJaar | Leden
	}

	/// <summary>
	/// Enum om aan te geven welke extra informatie er met een lid mee opgehaald moet worden.
	/// </summary>
	/// <remarks>GroepsWerkJaar wordt blijkbaar steeds standaard mee opgenomen.</remarks>
	[Flags]
	public enum LidExtras
	{
		Geen = 0x00,

		/// <summary>
		/// Haalt groepswerkjaar en groep mee op
		/// </summary>
		Groep = 0x01,
		
		/// <summary>
		/// Haalt afdelingsjaren en afdelingen mee op
		/// </summary>
		Afdelingen = 0x02,
		
		/// <summary>
		/// Haalt alle afdelingen van het groepswerkjaar van het lid mee op
		/// </summary>
		AlleAfdelingen = 0x04,
		
		/// <summary>
		/// Haalt functies mee op
		/// </summary>
		Functies = 0x08,
		
		/// <summary>
		/// Haalt gelieerde persoon en persoon mee op.
		/// </summary>
		Persoon = 0x10,

		/// <summary>
		/// Haalt personen en verzekeringen mee op
		/// </summary>
		Verzekeringen = 0x20,

		/// <summary>
		/// Haalt adressen van de personen mee op
		/// </summary>
		Adressen = 0x40,

		/// <summary>
		/// Communicatiemiddelen mee ophalen
		/// </summary>
		Communicatie = 0x80,
		
		Alles = Groep | Afdelingen | Functies | Persoon | Verzekeringen | Adressen | Communicatie
	}

	/// <summary>
	/// Enum om aan te geven welke extra informatie er met een (gelieerde) persoon mee opgehaald moet worden.
	/// </summary>
	[Flags]
	public enum PersoonsExtras
	{
		/// <summary>
		/// Haal niks extra mee op
		/// </summary>
		Geen = 0x00,
		/// <summary>
		/// Haal de adressen van de (gelieerde) persoon mee op
		/// </summary>
		Adressen = 0x01,
		/// <summary>
		/// Haal de groep(en) op waar de (gelieerde) personen mee geleerd zijn
		/// </summary>
                Groep = 0x02,
		/// <summary>
		/// Haal alle communicatievormen van de gelieerde persoon mee op, met gekoppeld
		/// communicatietype.
		/// </summary>
		Communicatie = 0x04,
		/// <summary>
		/// De categorieen van de gelieerde persoon
		/// </summary>
		Categorieen = 0x08,
		/// <summary>
		/// Enkel van toepassing op persoonsobjecten: neem *alle* gelieerdepersoonsobjecten mee (ook die van andere groepen).
		/// Enkel gebruiken als strict noodzakelijk! (bijv als het voorkeursadres in een andere groep mee gemanipuleerd moet 
		/// worden)
		/// </summary>
		AlleGelieerdePersonen = 0x10
	}

	/// <summary>
	/// Extra info die opgehaald kan worden met een groepswerkjaar
	/// </summary>
	[Flags]
	public enum GroepsWerkJaarExtras
	{
		Geen = 0x00,

		/// <summary>
		/// Haalt alle afdelingsjaren van het groepswerkjaar mee op, inclusief de gekoppelde afdelingen.
		/// </summary>
		Afdelingen = 0x01,
	
		/// <summary>
		/// Haalt alle leden van het groepswerkjaar mee op
		/// </summary>
		Leden = 0x02,
		
		/// <summary>
		/// Haalt de functies van de leden mee op
		/// </summary>
		LidFuncties = 0x04,

		/// <summary>
		/// Haalt de groep mee op.
		/// </summary>
		Groep = 0x08,

		/// <summary>
		/// Haalt de groep mee op, samen met al zijn functies (ook diegene die niet relevant zijn in het
		/// gegeven groepswerkjaar.)
		/// </summary>
		GroepsFuncties = 0x10
	}
}
