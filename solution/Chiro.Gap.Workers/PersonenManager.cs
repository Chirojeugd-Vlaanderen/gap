// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. personen bevat
    /// </summary>
    public class PersonenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public PersonenManager(IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizer">
        /// Te verhuizen GelieerdePersoon
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            Verhuizen(new[] { verhuizer }, oudAdres, nieuwAdres, adresType);
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizers">
        /// Te verhuizen personen
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        public void Verhuizen(IEnumerable<Persoon> verhuizers, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            var persIDs = (from p in verhuizers
                           select p.ID).ToArray();
            var mijnPersIDs = _autorisatieMgr.EnkelMijnPersonen(persIDs);

            if (persIDs.Count() == mijnPersIDs.Count())
            {
                // Vind personen waarvan het adres al gekoppeld is.
                var bestaand = verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == nieuwAdres.ID));

                if (bestaand.FirstOrDefault() != null)
                {
                    // Geef een exception met daarin de persoonsadresobjecten die al bestaan
                    throw new BlokkerendeObjectenException<PersoonsAdres>(
                        bestaand,
                        bestaand.Count(),
                        Resources.WonenDaarAl);
                }

                var oudePersoonsAdressen =
                    verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == oudAdres.ID));

                foreach (var pa in oudePersoonsAdressen)
                {
                    // verwijder koppeling oud adres->persoonsadres
                    pa.Adres.PersoonsAdres.Remove(pa);

                    // adrestype
                    pa.AdresType = adresType;

                    // koppel persoonsadres aan nieuw adres
                    pa.Adres = nieuwAdres;

                    nieuwAdres.PersoonsAdres.Add(pa);
                }
            }
            else
            {
                // Minstens een persoon waarvoor de user geen GAV is.  Zo'n gepruts verdient
                // een onverbiddellijke geen-gav-exception.
                throw new GeenGavException(Resources.GeenGav);
            }
        }


        /// <summary>
        /// Verlegt alle referenties van de persoon met ID <paramref name="dubbelID"/> naar de persoon met ID
        /// <paramref name="origineelID"/>, en verwijdert vervolgens de dubbele persoon.
        /// </summary>
        /// <param name="origineelID">
        /// ID van de te behouden persoon
        /// </param>
        /// <param name="dubbelID">
        /// ID van de te verwijderen persoon, die eigenlijk gewoon dezelfde is de te
        /// behouden.
        /// </param>
        /// <remarks>
        /// Het is niet proper dit soort van logica in de data access te doen.  Anderzijds zou het een 
        /// heel gedoe zijn om dit in de businesslaag te implementeren, omdat er heel wat relaties verlegd moeten worden.
        /// Dat wil zeggen: relaties verwijderen en vervolgens nieuwe maken.  Dit zou een heel aantal 'TeVerwijderens' met zich
        /// meebrengen, wat het allemaal zeer complex zou maken.  Vandaar dat we gewoon via een stored procedure werken.
        /// <para>
        /// </para>
        /// </remarks>
        public void DubbelVerwijderen(int origineelID, int dubbelID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Zoekt in de database personen met een gedeeld AD-nummer, en merget deze.
        /// </summary>
        public void FixGedeeldeAds()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Kent een AD-nummer toe aan een persoon, en persisteert.  Als er al een persoon bestond met
        /// het gegeven AD-nummer, worden de personen gemerged.
        /// </summary>
        /// <param name="persoon">
        /// Persoon met toe te kennen AD-nummer
        /// </param>
        /// <param name="adNummer">
        /// Toe te kennen AD-nummer
        /// </param>
        public void AdNummerToekennen(Persoon persoon, int adNummer)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //if (_autorisatieMgr.IsSuperGav())
            //{
            //    // Wie heeft het gegeven AD-nummer al?
            //    var gevonden = _dao.ZoekenOpAd(adNummer);

            //    foreach (var p in gevonden.Where(prs => prs.ID != persoon.ID))
            //    {
            //        // Als er andere personen zijn met hetzelfde AD-nummer, 
            //        // merge dan met deze persoon.

            //        // Door 'persoon.ID' als origineel te kiezen, vermijden
            //        // we dat persoon van ID verandert.
            //        _dao.DubbelVerwijderen(persoon.ID, p.ID);
            //    }

            //    // Tenslotte het echte werk.
            //    persoon.AdNummer = adNummer;
            //    _dao.Bewaren(persoon);
            //}
            //else
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}
        }
    }
}