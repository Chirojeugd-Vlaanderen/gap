using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AbonnementenService" in code, svc and config file together.
    public class AbonnementenService : IAbonnementenService
    {
        private readonly IGelieerdePersonenManager _gpMgr;
        private readonly IAbonnementenManager _abMgr;


        /// <summary>
        /// Constructor, zorgt voor dependency injection
        /// </summary>
        /// <param name="gelieerdePersonenManager">Businesslogica voor gelieerde personen</param>
        /// <param name="abonnementenManager">Businesslogica voor abonnementen</param>
        public AbonnementenService(IGelieerdePersonenManager gelieerdePersonenManager, IAbonnementenManager abonnementenManager)
        {
            _gpMgr = gelieerdePersonenManager;
            _abMgr = abonnementenManager;
        }

        /// <summary>
        /// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
        public void DubbelPuntBestellen(int gelieerdePersoonID)
        {
            // TODO (#1048): exceptions op databaseniveau catchen

            GelieerdePersoon gp = null;
            Abonnement abonnement = null;

            try
            {
                // Het ophalen van alle groepswerkjaren is overkill.  Maar ik doe het toch.
                gp = _gpMgr.Ophalen(gelieerdePersoonID, PersoonsExtras.Adressen | PersoonsExtras.AbonnementenDitWerkjaar | PersoonsExtras.GroepsWerkJaren);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
            catch (FoutNummerException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            Debug.Assert(gp != null);

            var groepsWerkJaar = gp.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();   // recentste groepswerkjaar
            var dubbelpunt = _abMgr.PublicatieOphalen(PublicatieID.Dubbelpunt);

            try
            {
                abonnement = _abMgr.Abonneren(dubbelpunt, gp, groepsWerkJaar);
            }
            catch (BlokkerendeObjectenException<Abonnement> ex)
            {
                // heeft al een abonnement
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
            catch (FoutNummerException ex)
            {
                if (ex.FoutNummer == FoutNummer.AdresOntbreekt || ex.FoutNummer == FoutNummer.BestelPeriodeDubbelpuntVoorbij)
                {
                    // Verwachte exception afhandelen
                    FoutAfhandelaar.FoutAfhandelen(ex);
                }
                else
                {
                    // Onverwachte exception opnieuw throwen
                    throw;
                }
            }

            _abMgr.Bewaren(abonnement);
        }

    }
}
