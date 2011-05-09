using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UitstappenService : IUitstappenService
    {
        private readonly UitstappenManager _uitstappenMgr;
        private readonly GroepsWerkJaarManager _groepsWerkJaarMgr;
        private readonly PlaatsenManager _plaatsenMgr;
        private readonly AdressenManager _adressenMgr;
		private readonly GelieerdePersonenManager _gelieerdePersonenMgr;
        private readonly DeelnemersManager _deelnemersMgr;

		/// <summary>
		/// Constructor.  De managers moeten m.b.v. dependency injection gecreeerd worden.
		/// </summary>
		/// <param name="uMgr">Uitstappenmanager</param>
		/// <param name="gwjMgr">Groepswerkjaarmanager</param>
		/// <param name="plMgr">Plaatsenmanager</param>
		/// <param name="adMgr">Adressenmanager</param>
		/// <param name="gpMgr">GelieerdePersonenManager</param>
		/// <param name="dMgr">Deelnemersmanager</param>
		public UitstappenService(
            UitstappenManager uMgr, 
            GroepsWerkJaarManager gwjMgr, 
            PlaatsenManager plMgr, 
            AdressenManager adMgr, 
            GelieerdePersonenManager gpMgr,
            DeelnemersManager dMgr)
		{
			_uitstappenMgr = uMgr;
			_groepsWerkJaarMgr = gwjMgr;
			_plaatsenMgr = plMgr;
			_adressenMgr = adMgr;
			_gelieerdePersonenMgr = gpMgr;
		    _deelnemersMgr = dMgr;
		}

        /// <summary>
        /// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep horende bij de uitstap.
        ///   Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
        /// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
        ///   dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
        /// <returns>ID van de uitstap</returns>
        public int Bewaren(int groepID, UitstapInfo info)
        {
            // Als de uitstap een ID heeft, moet een bestaande uitstap worden opgehaald.
            // Anders maken we een nieuwe.

            Uitstap uitstap;

            if (info.ID == 0)
            {
                var gwj = _groepsWerkJaarMgr.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Groep);
                uitstap = Mapper.Map<UitstapInfo, Uitstap>(info);
                _uitstappenMgr.Koppelen(uitstap, gwj);
            }
            else
            {
                // haal origineel op, gekoppeld aan groepswerkjaar
                uitstap = _uitstappenMgr.Ophalen(info.ID, UitstapExtras.GroepsWerkJaar);
                // overschrijf met gegevens uit 'info'
                Mapper.Map(info, uitstap);
            }

            try
            {
                return _uitstappenMgr.Bewaren(uitstap, UitstapExtras.Geen).ID;
            }
            // Afhandelen van verwachte exceptions
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);

                throw;
                // Enkel pro forma, want FoutAfhandelen throwt normaal gesproeken
                // een fault.
            }
            catch (FoutNummerException ex)
            {
                if (ex.FoutNummer == FoutNummer.GroepsWerkJaarNietBeschikbaar)
                {
                    FoutAfhandelaar.FoutAfhandelen(ex);
                }

                // Onverwachte exceptions gewoon terug throwen, zodat we ze tegen komen
                // bij het debuggen.

                throw;
            }
        }

        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="inschrijvenMogelijk">Als deze <c>true</c> is, worden enkel de uitstappen opgehaald
        /// waarvoor je nog kunt inschrijven.  In praktijk zijn dit de uitstappen van het huidige werkjaar.
        /// </param>
        /// <returns>Details van uitstappen</returns>
        /// <remarks>We laten toe om inschrijvingen te doen voor uitstappen uit het verleden, om als dat
        /// nodig is achteraf fouten in de administratie recht te zetten.</remarks>
        public IEnumerable<UitstapInfo> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
        {
            try
            {
                return Mapper.Map<IEnumerable<Uitstap>, IEnumerable<UitstapInfo>>(_uitstappenMgr.OphalenVanGroep(groepID, inschrijvenMogelijk));
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }
        }

        /// <summary>
        /// Haalt details over uitstap met gegeven <paramref name="uitstapID"/> op.
        /// </summary>
        /// <param name="uitstapID">ID van de uitstap</param>
        /// <returns>Details over de uitstap</returns>
        public UitstapDetail DetailsOphalen(int uitstapID)
        {
            try
            {
                var uitstap = _uitstappenMgr.Ophalen(uitstapID, UitstapExtras.GroepsWerkJaar | UitstapExtras.Plaats);
                var uitstapDetail = Mapper.Map<Uitstap, UitstapDetail>(uitstap);
                return uitstapDetail;
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }
        }

        /// <summary>
        /// Updatet de plaats voor de uitstap met gegeven <paramref name="uitstapID"/>
        /// </summary>
        /// <param name="uitstapID">ID van uitstap waarvan plaats geüpdatet moet worden</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adresInfo">Adres van de plaats</param>
        public void PlaatsBewaren(int uitstapID, string plaatsNaam, AdresInfo adresInfo)
        {
            Uitstap uitstap;
            Plaats plaats;

            try
            {
                uitstap = _uitstappenMgr.Ophalen(uitstapID, UitstapExtras.Groep | UitstapExtras.Plaats);
                if (uitstap == null)
                {
                    // Als er geen uitstap is gevonden, dan is de gebruiker waarschijnlijk iets
                    // aan het doen dat niet mag.  Bij deze een algemene exception.
                    throw new GeenGavException();
                }
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            Debug.Assert(uitstap != null);

            Adres adres = _adressenMgr.ZoekenOfMaken(adresInfo);

            try
            {
                plaats = _plaatsenMgr.ZoekenOfMaken(uitstap.GroepsWerkJaar.Groep.ID, plaatsNaam, adres.ID);
            }
            catch (OngeldigObjectException ex)
            {
                // Verkeerde straatnaam of zo
                var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);
                throw new FaultException<OngeldigObjectFault>(fault);
            }

            Debug.Assert(plaats != null);

            try
            {
                uitstap = _uitstappenMgr.Koppelen(uitstap, plaats);
            }
            catch (BlokkerendeObjectenException<Plaats> ex)
            {
                // Was al gekoppeld

                var fault = Mapper.Map<BlokkerendeObjectenException<Plaats>, BlokkerendeObjectenFault<PlaatsInfo>>(ex);
                throw new FaultException<BlokkerendeObjectenFault<PlaatsInfo>>(fault);
            }

			_uitstappenMgr.Bewaren(uitstap, UitstapExtras.Plaats);
		}

		/// <summary>
		/// Schrijft de gelieerde personen met ID's <paramref name="gelieerdePersoonIDs"/> in voor de
		/// uitstap met ID <paramref name="geselecteerdeUitstapID" />.  Als
		/// <paramref name="logistiekDeelnemer" /> <c>true</c> is, wordt er ingeschreven als
		/// logistiek deelnemer.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van in te schrijven gelieerde personen</param>
		/// <param name="geselecteerdeUitstapID">ID van uitstap waarvoor in te schrijven</param>
		/// <param name="logistiekDeelnemer">Bepaalt of al dan niet ingeschreven wordt als 
		/// logistieker</param>
		public UitstapInfo Inschrijven(IList<int> gelieerdePersoonIDs, int geselecteerdeUitstapID, bool logistiekDeelnemer)
		{
			IEnumerable<GelieerdePersoon> gelieerdePersonen;
			Uitstap uitstap;

			// Ik haal de personen op, samen met de uitstappen waarvoor ze ooit waren ingeschreven.
			// Dat is overkill, maar op die manier kunnen de workers wel controleren wie er al wel/nog niet
			// ingeschreven is voor de gevraagde uitstap. (-> proper :))

			try
			{
				gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Uitstappen|PersoonsExtras.Groep);
			}
			catch (GeenGavException ex)
			{
				var fault = Mapper.Map<GeenGavException, GapFault>(ex);
				throw new FaultException<GapFault>(fault);
			}

			try
			{
				// Als de uitstap al gekoppeld was aan een gelieerde persoon, dan hebben we die al opgehaald.  Zo niet
				// halen we de uitstap op via de uitstappenMgr.

				uitstap =
					gelieerdePersonen.SelectMany(gp => gp.Deelnemer).Select(d => d.Uitstap).Where(u => u.ID == geselecteerdeUitstapID).
						FirstOrDefault() ?? _uitstappenMgr.Ophalen(geselecteerdeUitstapID, UitstapExtras.Groep);
			}
			catch (GeenGavException ex)
			{
				var fault = Mapper.Map<GeenGavException, GapFault>(ex);
				throw new FaultException<GapFault>(fault);
			}


			try
			{
				_uitstappenMgr.Inschrijven(uitstap, gelieerdePersonen, logistiekDeelnemer);
			}
			catch (FoutNummerException ex)
			{
				if (ex.FoutNummer == FoutNummer.UitstapNietVanGroep || ex.FoutNummer == FoutNummer.GroepsWerkJaarNietBeschikbaar)
				{
					var fault = Mapper.Map<FoutNummerException, FoutNummerFault>(ex);
					throw new FaultException<FoutNummerFault>(fault);
				}

				// Als het foutnummer iets anders is, dan is er iets
				// onverwachts gebeurd.  Gewoon throwen.
				throw;
			}

			_uitstappenMgr.Bewaren(uitstap, UitstapExtras.Deelnemers);

			return Mapper.Map<Uitstap, UitstapInfo>(uitstap);
		}

        /// <summary>
        /// Haalt informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/> op.
        /// </summary>
        /// <param name="uitstapID">ID van de relevante uitstap</param>
        /// <returns>informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/></returns>
        public IEnumerable<DeelnemerDetail> DeelnemersOphalen(int uitstapID)
        {
            IEnumerable<Deelnemer> deelnemers;
            try
            {
                deelnemers = _uitstappenMgr.DeelnemersOphalen(uitstapID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            var resultaat = Mapper.Map<IEnumerable<Deelnemer>, IEnumerable<DeelnemerDetail>>(deelnemers);

            return resultaat;
        }

        /// <summary>
        /// Stelt de deelnemer met gegeven <paramref name="deelnemerID" /> in als contactpersoon voor de uitstap
        /// waaraan hij deelneemt
        /// </summary>
        /// <param name="deelnemerID">ID van de als contact in te stellen deelnemer</param>
        /// <returns>De ID van de uitstap, ter controle, en misschien handig voor feedback</returns>
        public int ContactInstellen(int deelnemerID)
        {
            Deelnemer deelnemer;
            try
            {
                deelnemer = _deelnemersMgr.Ophalen(deelnemerID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            _deelnemersMgr.InstellenAlsContact(deelnemer);

            _uitstappenMgr.Bewaren(deelnemer.Uitstap, UitstapExtras.Contact);

            return deelnemer.Uitstap.ID;
        }

        /// <summary>
        /// Schrijft de deelnemer met gegeven <paramref name="deelnemerID"/> uit voor zijn uitstap.
        /// </summary>
        /// <param name="deelnemerID">ID uit te schrijven deelnemer</param>
        /// <returns>ID van de uitstap, ter controle, en handig voor feedback</returns>
        public int Uitschrijven(int deelnemerID)
        {
            Deelnemer deelnemer;
            try
            {
                deelnemer = _deelnemersMgr.Ophalen(deelnemerID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

	        _deelnemersMgr.Verwijderen(deelnemer);

            return deelnemer.Uitstap.ID;
        }

        /// <summary>
        /// Haalt informatie over de deelnemer met ID <paramref name="deelnemerID"/> op.
        /// </summary>
        /// <param name="deelnemerID">ID van de relevante deelnemer</param>
        /// <returns>informatie over de deelnemer met ID <paramref name="deelnemerID"/></returns>
        public DeelnemerDetail DeelnemerOphalen(int deelnemerID)
        {
            Deelnemer d;
            try
            {
                d = _deelnemersMgr.Ophalen(deelnemerID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            var resultaat = Mapper.Map<Deelnemer, DeelnemerDetail>(d);

            return resultaat;
        }


        /// <summary>
        /// Updatet een deelnemer op basis van de info in <paramref name="info"/>
        /// </summary>
        /// <param name="info">info nodig voor de update</param>
        public void DeelnemerBewaren(DeelnemerInfo info)
        {
            Deelnemer d;
            try
            {
                d = _deelnemersMgr.Ophalen(info.DeelnemerID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            d.IsLogistieker = info.IsLogistieker;
            d.HeeftBetaald = info.HeeftBetaald;
            d.MedischeFicheOk = info.MedischeFicheOk;
            d.Opmerkingen = info.Opmerkingen;

            _deelnemersMgr.Bewaren(d);
        }
    }
}
