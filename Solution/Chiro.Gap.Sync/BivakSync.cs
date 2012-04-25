// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using Adres = Chiro.Gap.Orm.Adres;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Synchronisatie van bivakaangifte naar Kipadmin
    /// </summary>
    public class BivakSync : IBivakSync
    {
        private readonly IAdressenDao _adressenDao;
        private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
        private readonly IDeelnemersDao _deelnemersDao;

        /// <summary>
        /// Standaardconstructor.  De parameters worden gebruikt voor dependency injection.
        /// </summary>
        /// <param name="adressenDao">Data access voor adressen</param>
        /// <param name="gelieerdePersonenDao">Data access voor gelieerde personen</param>
        /// <param name="deelnemersDao">Data access voor deelnemers</param>
        public BivakSync(
            IAdressenDao adressenDao,
            IGelieerdePersonenDao gelieerdePersonenDao,
            IDeelnemersDao deelnemersDao)
        {
            _adressenDao = adressenDao;
            _gelieerdePersonenDao = gelieerdePersonenDao;
            _deelnemersDao = deelnemersDao;
        }

        /// <summary>
        /// Bewaart de uitstap <paramref name="uitstap"/> in Kipadmin als bivak.  Zonder contactpersoon
        /// of plaats.
        /// </summary>
        /// <param name="uitstap">Te bewaren uitstap</param>
        public void Bewaren(Uitstap uitstap)
        {
            // TODO (#1057): Dit zijn waarschijnlijk te veel databasecalls

            var teSyncen = Mapper.Map<Uitstap, Bivak>(uitstap);
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakBewaren(teSyncen));

            GelieerdePersoon contactPersoon;

            if (uitstap.ContactDeelnemer != null)
            {
                // Er is een contactdeelnemer.  Is de persoon nog geladen?

                if (uitstap.ContactDeelnemer.GelieerdePersoon == null || uitstap.ContactDeelnemer.GelieerdePersoon.Persoon == null)
                {
                    // nee, haal deelnemer opnieuw op, met contactpersoon

                    var deelnemer = _deelnemersDao.Ophalen(uitstap.ContactDeelnemer.ID, d => d.GelieerdePersoon.Persoon);
                    contactPersoon = deelnemer.GelieerdePersoon;
                }
                else
                {
                    contactPersoon = uitstap.ContactDeelnemer.GelieerdePersoon;
                }
            }
            else
            {
                contactPersoon = null;
            }

            if (uitstap.Plaats != null && uitstap.Plaats.Adres != null)
            {
                // Haal adres opnieuw op, zodat we zeker gemeente of land mee hebben.

                var adres = _adressenDao.Ophalen(uitstap.Plaats.Adres.ID);
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakPlaatsBewaren(uitstap.ID, uitstap.Plaats.Naam, Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(adres)));
            }

            if (contactPersoon != null)
            {
                if (contactPersoon.Persoon.AdNummer != null)
                {
                    // AD-nummer gekend: gewoon koppelen via AD-nummer
                    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakContactBewaren(
                        uitstap.ID,
                        contactPersoon.Persoon.AdNummer ?? 0));
                }
                else
                {
                    // Als we geen AD-nummer hebben, haal dan ook de communicatie en het
                    // voorkeursadres op, want die zullen we nodig hebben om te identificeren.

                    var gelPersoon = _gelieerdePersonenDao.Ophalen(
                        contactPersoon.ID, PersoonsExtras.Communicatie | PersoonsExtras.VoorkeurAdres);

                    // Geef door met gegevens ipv ad-nummer.  Registreer dat ad-nummer
                    // in aanvraag is.
                    gelPersoon.Persoon.AdInAanvraag = true;
                    _gelieerdePersonenDao.Bewaren(gelPersoon, gp => gp.Persoon);

                    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakContactBewarenAdOnbekend(
                        uitstap.ID,
                        Mapper.Map<GelieerdePersoon, PersoonDetails>(gelPersoon)));
                }
            }
        }

        /// <summary>
        /// Verwijdert uitstap met <paramref name="uitstapID"/> uit kipadmin
        /// </summary>
        /// <param name="uitstapID">ID te verwijderen uitstap</param>
        public void Verwijderen(int uitstapID)
        {
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakVerwijderen(uitstapID));
        }
    }
}
