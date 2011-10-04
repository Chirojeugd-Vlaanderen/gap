// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
        LandNietGevonden,
		WoonPlaatsNietGevonden,
		StraatNietGevonden,
		StraatOntbreekt,
		OngeldigPostNummer,
		WoonPlaatsOntbreekt,
		WonenDaarAl,
		AdresOntbreekt,
		#endregion

		#region leden
		AlgemeneLidFout,
		AlgemeneKindFout,
		AlgemeneLeidingFout,
		OnbekendGeslachtFout,
		#endregion

		#region verkeerde groep
		CategorieNietVanGroep,
		FunctieNietVanGroep,
		AfdelingNietVanGroep,
		GroepsWerkJaarNietVanGroep,
		UitstapNietVanGroep,
		#endregion

		#region niet beschikbaar in werkjaar
		GroepsWerkJaarNietBeschikbaar,
		FunctieNietBeschikbaar,
		AfdelingNietBeschikbaar,
		#endregion

		#region container niet leeg
		CategorieNietLeeg,
		FunctieNietLeeg,
		AfdelingNietLeeg,
		#endregion

        #region verplichte gegevens
        AdNummerVerplicht,
        EMailVerplicht,
        #endregion

        #region nog te groeperen
        GeenDatabaseVerbinding,
		Concurrency,
		DeelnemerNietVanUitstap,
        PublicatieInactief,
        BestelPeriodeDubbelpuntVoorbij,
        GebruikersRechtNietVerlengbaar,
        GebruikersRechtWasAlVervallen
        #endregion
	}
}
