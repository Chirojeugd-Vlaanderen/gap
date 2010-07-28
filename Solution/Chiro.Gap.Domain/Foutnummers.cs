// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Identificatie van de fout
	/// </summary>
	public enum FoutNummer
	{
		AlgemeneFout,
		GeenGav,
		BestaatAl,
		ValidatieFout,
		FouteGeboortejarenVoorAfdeling,
		ChronologieFout,		// fout ivm volgorde datums

		#region adressen
		WoonPlaatsNietGevonden,
		StraatNietGevonden,
		StraatOntbreekt,
		OngeldigPostNummer,
		WoonPlaatsOntbreekt,
		WonenDaarAl,
		#endregion

		#region verkeerde groep
		CategorieNietVanGroep,
		FunctieNietVanGroep,
		AfdelingNietVanGroep,
		GroepsWerkJaarNietVanGroep,
		#endregion

		#region niet beschikbaar in werkjaar
		GroepsWerkJaarNietBeschikbaar,
		FunctieNietBeschikbaar,
		AfdelingNietBeschikbaar,
		#endregion

		#region nog te verdelen
		CategorieNietLeeg,
		FunctieNietLeeg,
		AfdelingNietLeeg,
		#endregion
	}
}
