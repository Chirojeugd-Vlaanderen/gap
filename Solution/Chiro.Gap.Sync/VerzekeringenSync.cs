// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.Linq;

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Deze klasse staat in voor het overzetten van verzekeringsgegevens naar Kipadmin.
    /// </summary>
    public class VerzekeringenSync : IVerzekeringenSync
    {
        private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
        private readonly IPersonenDao _personenDao;

        /// <summary>
        /// Standaardconstructor.  De parameters worden gebruikt voor dependency injection.
        /// </summary>
        /// <param name="gelieerdePersonenDao">Data access object voor gelieerde personen</param>
        /// <param name="personenDao">Data access object voor personen</param>
        public VerzekeringenSync(
            IGelieerdePersonenDao gelieerdePersonenDao,
            IPersonenDao personenDao)
        {
            _gelieerdePersonenDao = gelieerdePersonenDao;
            _personenDao = personenDao;
        }

        /// <summary>
        /// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
        /// </summary>
        /// <param name="persoonsVerzekering">Over te zetten persoonsverzekering</param>
        /// <param name="gwj">Bepaalt werkjaar en groep die factuur zal krijgen</param>
        public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
        {
            if (persoonsVerzekering.Persoon.AdNummer != null)
            {
                // Verzekeren op basis van AD-nummer
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LoonVerliesVerzekeren(
                    persoonsVerzekering.Persoon.AdNummer ?? 0,
                    gwj.Groep.Code,
                    gwj.WerkJaar));
            }
            else
            {
                // Verzekeren op basis van details.  
                // Hoewel een verzekering loonverlies
                // enkel mogelijk is voor leden, mogen we er niet vanuit gaan dat het AD-nummer al
                // in aanvraag is.  Het kan namelijk zijn dat het lid nog in zijn probeerperiode is.
                // Vraag dus sowieso een AD-nummer aan.

                persoonsVerzekering.Persoon.AdInAanvraag = true;
                _personenDao.Bewaren(persoonsVerzekering.Persoon);

                // Haal even de gelieerde persoon op, om gemakkelijk de details te kunnen mappen.
                // TODO (#754): Op die manier verliezen we mogelijk communicatiemiddelen

                var gp = _gelieerdePersonenDao.Ophalen(
                    persoonsVerzekering.Persoon.ID,
                    gwj.Groep.ID,
                    true,
                    gelp => gelp.Persoon,
                    gelp => gelp.Communicatie.First().CommunicatieType);

                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LoonVerliesVerzekerenAdOnbekend(
                    Mapper.Map<GelieerdePersoon, PersoonDetails>(gp),
                    gwj.Groep.Code,
                    gwj.WerkJaar));
            }
        }
    }
}
