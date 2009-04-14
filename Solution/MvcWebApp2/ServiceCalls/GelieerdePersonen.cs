using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Cg2.ServiceContracts;
using Cg2.Orm;

namespace MvcWebApp2.ServiceCalls
{
    public static class GelieerdePersonen
    {
        private const string _endpointConfig = "WSHttpBinding_IGelieerdePersonenService";

        /// <summary>
        /// Haalt adres met bewonersinfo op van de service
        /// </summary>
        /// <param name="adresID">ID van het op te halen adres</param>
        /// <returns>Adresobject met bewonersinfo</returns>
        public static Adres AdresMetBewonersOphalen(int adresID)
        {
            Adres res;
            using (ChannelFactory<IGelieerdePersonenService> cf = new ChannelFactory<IGelieerdePersonenService>(_endpointConfig))
            {
                IGelieerdePersonenService service = cf.CreateChannel();
                try
                {
                    res = service.AdresMetBewonersOphalen(adresID);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Haalt pagina met persoonsinfo op, inclusief een optionele connectie met een lidobject
        /// </summary>
        /// <param name="aantal">zal het aantal opgehaalde personen bevatten</param>
        /// <param name="groepID">ID van groep waarvoor personen opgehaald moeten worden</param>
        /// <param name="pagina">paginanr (>= 1)</param>
        /// <param name="paginaGrootte">aantal leden per pagina</param>
        /// <returns>lijst met gelieerde personen</returns>
        public static IList<GelieerdePersoon> PaginaOphalenMetLidInfo(out int aantal, int groepID, int pagina, int paginaGrootte)
        {
            IList<GelieerdePersoon> res;
            using (ChannelFactory<IGelieerdePersonenService> cf = new ChannelFactory<IGelieerdePersonenService>(_endpointConfig))
            {
                IGelieerdePersonenService service = cf.CreateChannel();
                try
                {
                    res = service.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantal);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Haalt een persoon met adressen en communicatievormen op
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>het gevraagde persoonsobject</returns>
        public static GelieerdePersoon PersoonOphalenMetDetails(int persoonID)
        {
            GelieerdePersoon res;
            using (ChannelFactory<IGelieerdePersonenService> cf = new ChannelFactory<IGelieerdePersonenService>(_endpointConfig))
            {
                IGelieerdePersonenService service = cf.CreateChannel();
                try
                {
                    res = service.PersoonOphalenMetDetails(persoonID);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Bewaart een persoon in de database (geen contactinfo of adressen)
        /// </summary>
        /// <param name="p">Te bewaren persoonsobject</param>
        public static void PersoonBewaren(GelieerdePersoon p)
        {
            using (ChannelFactory<IGelieerdePersonenService> cf = new ChannelFactory<IGelieerdePersonenService>(_endpointConfig))
            {
                IGelieerdePersonenService service = cf.CreateChannel();
                try
                {
                    service.PersoonBewaren(p);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
        }

        /// <summary>
        /// Verhuist gelieerde personen met gegeven ID's van adres met gegeven
        /// oud ID naar gegeven nieuw adres.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's te verhuizen personen</param>
        /// <param name="nieuwAdres">adresobject waarnaar verhuisd moet worden</param>
        /// <param name="oudAdresID">ID van van-adres</param>
        /// <remarks>Het AdresID van nieuwAdres wordt genegeerd.  Aan de hand van
        /// straat, nr,... wordt gekeken of het adres bestaat.  Zo niet wordt een
        /// nieuw aangemaakt, met nieuw ID.</remarks>
        public static void Verhuizen(IList<int> gelieerdePersoonIDs, Adres nieuwAdres, int oudAdresID)
        {
            using (ChannelFactory<IGelieerdePersonenService> cf = new ChannelFactory<IGelieerdePersonenService>(_endpointConfig))
            {
                IGelieerdePersonenService service = cf.CreateChannel();
                try
                {
                    service.Verhuizen(gelieerdePersoonIDs, nieuwAdres, oudAdresID);
                }
                finally
                {
                    ((IClientChannel)service).Close();
                }
            }
        }

    }
}
