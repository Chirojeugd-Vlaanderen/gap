// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Orm
{
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
	[DataContract]
	[Flags]
	public enum LidExtras
	{
		[EnumMember]
		Geen = 0x00,
		/// <summary>
		/// Haalt groepswerkjaar en groep mee op
		/// </summary>
		[EnumMember]
		Groep = 0x01,
		/// <summary>
		/// Haalt afdelingsjaren en afdelingen mee op
		/// </summary>
		[EnumMember]
		Afdelingen = 0x02,
		/// <summary>
		/// Haalt alle afdelingen van het groepswerkjaar van het lid mee op
		/// </summary>
		[EnumMember]
		AlleAfdelingen = 0x04,
		/// <summary>
		/// Haalt functies mee op
		/// </summary>
		[EnumMember]
		Functies = 0x08,
		[EnumMember]
		Alles = Groep | Afdelingen | Functies
	}

	/// <summary>
	/// Enum om aan te geven welke extra informatie er met een (gelieerde) persoon mee opgehaald moet worden.
	/// </summary>
	[DataContract]
	[Flags]
	public enum PersoonsExtras
	{
		[EnumMember]
		Geen = 0x00,
		[EnumMember]
		Adressen = 0x01
	}

	/// <summary>
	/// Extra info die opgehaald kan worden met een groepswerkjaar
	/// </summary>
	[DataContract]
	[Flags]
	public enum GroepsWerkJaarExtras
	{
		[EnumMember]
		Geen = 0x00,
		/// <summary>
		/// Haalt alle afdelingsjaren van het groepswerkjaar mee op, inclusief de gekoppelde afdelingen.
		/// </summary>
		[EnumMember]
		Afdelingen = 0x01,
		/// <summary>
		/// Haalt alle leden van het groepswerkjaar mee op
		/// </summary>
		[EnumMember]
		Leden = 0x02,
		/// <summary>
		/// Haalt de functies van de leden mee op
		/// </summary>
		[EnumMember]
		LidFuncties = 0x04,
		/// <summary>
		/// Haalt de groep mee op, samen met al zijn functies (ook diegene die niet relevant zijn in het
		/// gegeven groepswerkjaar.)
		/// </summary>
		[EnumMember]
		GroepsFuncties = 0x08
	}
		
}
