using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Fouten
{
	/// <summary>
	/// Foutcodes in verband met het aanmaken van reeds bestaande objecten
	/// </summary>
	[DataContract]
	public enum BestaatAlFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,	            // standaardwaarde
		[EnumMember]
		CategorieCodeBestaatAl,	        // er is al een categorie met die code
		[EnumMember]
		CategorieNaamBestaatAl,         // er is al een categorie met die naam
		[EnumMember]
		FunctieCodeBestaatAl,		// er is al een functie met die code
		[EnumMember]
		FunctieNaamBestaatAl,		// er is al een functie met die naam
		[EnumMember]
		LidBestaatAl,			// de persoon is al lid in het gegeven groepswerkjaar
		[EnumMember]
		PersoonBestaatAl		// er is een gelijkaardige persoon gevonden
		// TODO: foutcodes voor andere dingen die al kunnen bestaan.
	}

	/// <summary>
	/// Foutcodes in verband met verkeerde adressen
	/// </summary>
	[DataContract]
	public enum AdresFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,   // standaardwaarde
		[EnumMember]
		OnbekendeStraat,
		[EnumMember]
		OnbekendeGemeente
	}

	[DataContract]
	public enum GeenGavFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,
		[EnumMember]
		Afdeling,
		[EnumMember]
		Categorie,
		[EnumMember]
		CommunicatieVorm,
		[EnumMember]
		Functie,
		[EnumMember]
		Groep,
		[EnumMember]
		GroepsWerkJaar,
		[EnumMember]
		Lid,
		[EnumMember]
		Persoon,
		[EnumMember]
		GeenSuperGav
	}

	/// <summary>
	/// Foutcode bij het gebruiken van entiteiten uit een andere groep.
	/// </summary>
	[DataContract]
	public enum VerkeerdeGroepFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,
		[EnumMember]
		Afdeling,
		[EnumMember]
		Categorie,
		[EnumMember]
		Functie
	};


	/// <summary>
	/// Foutcodes ivm operaties die geblokkeerd worden omdat er nog objecten aan iets gekoppeld zijn
	/// </summary>
	[DataContract]
	public enum GekoppeldeObjectenFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,
		[EnumMember]
		CategorieNietLeeg
	};

	/// <summary>
	/// Foutcode die gebruikt wordt als objecten niet beschikbaar zijn in het gegeven werkjaar.
	/// </summary>
	[DataContract]
	public enum NietBeschikbaarFoutCode
	{
		[EnumMember]
		AlgemeneFout = 0,
		[EnumMember]
		Afdeling,		// Afdeling niet beschikbaar in dit werkjaar
		[EnumMember]
		Functie			// Functie niet beschikbaar in dit werkjaar
	}
}
