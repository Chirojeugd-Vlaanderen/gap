// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Domain
{
	public enum FoutNummer
	{
        AlgemeneFout,
        GeenGav,
        BestaatAl,
        ValidatieFout,

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
        #endregion


        #region niet beschikbaar in werkjaar
        GroepsWerkJaarNietBeschikbaar,
        FunctieNietBeschikbaar,
        AfdelingNietBeschikbaar,
        #endregion


        #region nog te verdelen
        CategorieNietLeeg,
        FunctieNietLeeg,
        #endregion
	}
}
