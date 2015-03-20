using System;
using System.Linq;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

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
        public async void LidVerwijderen(int adNummer, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            int? civiGroepId = _contactHelper.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet verwijderd.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            var contact = _contactHelper.PersoonMetRecentsteLid(adNummer, civiGroepId);

            if (contact == null || contact.ExternalIdentifier == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te verwijderen lid - als dusdanig terug naar GAP.", adNummer),
                    stamNummer, adNummer, null);
                await _gapUpdateHelper.OngeldigAdNaarGap(adNummer);
                return;
            }

            if (contact.RelationshipResult.Count == 1 &&
                _relationshipHelper.WerkjaarGet(contact.RelationshipResult.Values.First()) == werkJaar)
            {
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "{0} {1} (AD {2}) uitscrhijven voor groep {3} in werkjaar {4}.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer, werkJaar),
                    stamNummer, adNummer, contact.GapId);
                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.RelationshipDelete(_apiKey, _siteKey,
                            new IdRequest(contact.RelationshipResult.Values.First().Id)));
                AssertValid(result);
            }
            else
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "{0} {1} (AD {2}) niet uitgeschreven voor groep {3} in werkjaar {4} - lidrelatie niet gevonden.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer, werkJaar),
                    stamNummer, adNummer, contact.GapId);
            }
        }

        /// <summary>
        /// Verwijdert een lid als het ad-nummer om een of andere reden niet bekend is.
        /// </summary>
        /// <param name="details">
        /// Gegevens die hopelijk toelaten het lid te identificeren
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van het lid
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar van het lid
        /// </param>
        /// <param name="uitschrijfDatum">uitschrijfdatum zoals geregistreerd in GAP</param>
        /// <remarks>
        /// Lid wordt hoe dan ook verwijderd.  De check op probeerperiode gebeurt
        /// in GAP.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void NieuwLidVerwijderen(PersoonDetails details, string stamNummer, int werkJaar, DateTime uitschrijfDatum)
        {
            if (details.Persoon.AdNummer != null)
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "NieuwLidVerwijderen aangeroepen voor persoon {0} {1} (gid {3}) met bestaand AD-Nummer {2}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.AdNummer, details.Persoon.ID),
                    stamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }

            int? adNummer = AdNummerZoeken(details);

            if (adNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format(
                        "NieuwLidVerwijderen aangeroepen voor niet-gematchte persoon {0} {1} (gid {2}) in groep {3}."
                        , details.Persoon.VoorNaam, details.Persoon.Naam, details.Persoon.ID, stamNummer),
                    stamNummer, details.Persoon.AdNummer, details.Persoon.ID);
            }
            else
            {
                LidVerwijderen(adNummer.Value, stamNummer, werkJaar, uitschrijfDatum);
            }
        }
    }
}