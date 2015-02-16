using System;
using System.ServiceModel;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert een persoon met gekend AD-nummer als lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer te verwijderen lid
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer te verwijderen lid
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar te verwijderen lid
        /// </param>
        /// <param name="uitschrijfDatum"> uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidVerwijderen(int adNummer, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            int? civiGroepId = _contactHelper.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet verwijderd.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            var contact = _contactHelper.PersoonMetRecentsteLid(adNummer, civiGroepId);

            if (contact.ExternalIdentifier == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te verwijderen lid - als dusdanig terug naar GAP.", adNummer),
                    stamNummer, adNummer, null);
            }
            throw new NotImplementedException();
        }
    }
}