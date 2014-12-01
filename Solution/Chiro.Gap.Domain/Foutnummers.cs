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
        ChronologieFout,		// fout ivm volgorde datums

        #region personen
        OngeldigeGeboorteJarenVoorAfdeling,
        GeboorteDatumOntbreekt,
        PersoonOverleden,
        #endregion

        #region adressen
        LandNietGevonden,
        WoonPlaatsNietGevonden,
        StraatNietGevonden,
        StraatOntbreekt,
        OngeldigPostNummer,
        WoonPlaatsOntbreekt,
        WonenDaarAl,
        AdresOntbreekt,
        AdresNietGekoppeld,
        #endregion

        #region leden
        AlgemeneLidFout,
        AlgemeneKindFout,
        AlgemeneLeidingFout,
        OnbekendGeslacht,
        LidTeJong,
        LeidingTeJong,
        LidTypeVerkeerd,
        AfdelingKindVerplicht,
        LidUitgeschreven,
        LidWasAlIngeschreven,
        #endregion

        #region verkeerde groep
        CategorieNietVanGroep,
        FunctieNietVanGroep,
        AfdelingNietVanGroep,
        GroepsWerkJaarNietVanGroep,
        UitstapNietVanGroep,
        #endregion

        #region niet beschikbaar in werkJaar
        GroepsWerkJaarNietBeschikbaar,
        FunctieNietBeschikbaar,
        AfdelingNietBeschikbaar,
        GroepInactief,
        #endregion

        #region container niet leeg
        CategorieNietLeeg,
        FunctieNietLeeg,
        AfdelingNietLeeg,
        #endregion

        #region verplichte gegevens
        AdNummerVerplicht,
        EMailVerplicht,
        ContactMoetNieuwsBriefKrijgen,
        TelefoonNummerOntbreekt,
        #endregion

        #region nog te groeperen
        GeenDatabaseVerbinding,
        Concurrency,
        DeelnemerNietVanUitstap,
        PublicatieInactief,
        GebruikersRechtNietVerlengbaar,
        GebruikersRechtWasAlVervallen,
        CategorieNietGekoppeld,
        OvergangTeVroeg,
        KoppelingLoginPersoonOntbreekt,
        #endregion
    }
}
