// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using Adres = Chiro.Gap.Poco.Model.Adres;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Synchronisatie van bivakaangifte naar Kipadmin
    /// </summary>
    public class BivakSync : IBivakSync
    {
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
                    throw new NotImplementedException();
                }
                contactPersoon = uitstap.ContactDeelnemer.GelieerdePersoon;
            }
            else
            {
                contactPersoon = null;
            }

            if (uitstap.Plaats != null && uitstap.Plaats.Adres != null)
            {
                throw new NotImplementedException();
            }

            if (contactPersoon != null)
            {
                if (contactPersoon.Persoon.AdNummer != null)
                {
                    // AD-nummer gekend: gewoon koppelen via AD-nummer
                    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.BivakContactBewaren(
                        uitstap.ID,
                        (int) contactPersoon.Persoon.AdNummer));
                }
                else
                {
                    throw new NotImplementedException();
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
