// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Autorisatiemanager die slechts 2 dingen doet:
    /// 1. Zeggen 'Ik ben super-GAV'.
    /// 2. De gebruikersnaam opleveren
    /// Alle andere zaken zijn niet-geïmplementeerd, en voor de veiligheid
    /// blijven ze dat best ook.
    /// </summary>
    /// <remarks>
    /// Deze klasse is een kopie van die in Chiro.Gap.UpdataSvc.Service.  Het lijkt
    /// me zo dom om hiervoor een apart project te maken.  Maar ik weet ook niet
    /// goed waar ik dit anders kwijt moet.  Ik wil dit uit Chiro.Gap.Workers houden,
    /// om te vermijden dat je super-gav kunt worden door gewoon de unity-configuration
    /// aan te passen.  Nu kun je dat enkel als Chiro.Gap.Diagnostics.Service.dll
    /// beschikbaar is.
    /// </remarks>
    public class SuperGavAutorisatieManager : IAutorisatieManager
    {
        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="gelieerdePersonenIDs">
        /// </param>
        /// <returns>
        /// </returns>
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="personenIDs">
        /// </param>
        /// <returns>
        /// </returns>
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="lidIDs">
        /// </param>
        /// <returns>
        /// </returns>
        public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="groepID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavGroep(int groepID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="groepIDs">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavGroepen(IEnumerable<int> groepIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="persoonID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="afdelingsID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavAfdeling(int afdelingsID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="afdelingsJaarID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavAfdelingsJaar(int afdelingsJaarID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="lidID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavLid(int lidID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="categorieID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavCategorie(int categorieID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="commvormID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavCommVorm(int commvormID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="functieID">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool IsGavFunctie(int functieID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="persoonsAdresID">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool IsGavPersoonsAdres(int persoonsAdresID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="persoonsAdresIDs">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="uitstapID">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool IsGavUitstap(int uitstapID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <returns>
        /// </returns>
        public bool IsSuperGav()
        {
            return true;
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <returns>
        /// </returns>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? String.Empty
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="plaatsID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavPlaats(int plaatsID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="deelnemerID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavDeelnemer(int deelnemerID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="gebruikersRechtID">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// </param>
        /// <returns>
        /// </returns>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO (#190): documenteren
        /// </summary>
        /// <param name="gebruikersrecht">
        /// </param>
        /// <returns>
        /// </returns>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            throw new NotImplementedException();
        }
    }
}
