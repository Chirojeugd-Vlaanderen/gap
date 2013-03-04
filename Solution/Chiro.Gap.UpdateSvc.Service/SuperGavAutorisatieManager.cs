// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Autorisatiemanager die slechts 2 dingen doet:
    /// 1. Zeggen 'Ik ben super-GAV'.
    /// 2. De gebruikersnaam opleveren
    /// Alle andere zaken zijn niet-geïmplementeerd, en voor de veiligheid
    /// blijven ze dat best ook.
    /// </summary>
    /// <remarks>Deze klasse is een kopie van die in Chiro.Gap.UpdataSvc.Service.  Het lijkt
    /// me zo dom om hiervoor een apart project te maken.  Maar ik weet ook niet
    /// goed waar ik dit anders kwijt moet.  Ik wil dit uit Chiro.Gap.Workers houden,
    /// om te vermijden dat je super-gav kunt worden door gewoon de unity-configuration
    /// aan te passen.  Nu kun je dat enkel als Chiro.Gap.Diagnostics.Service.dll
    /// beschikbaar is.</remarks>
    public class SuperGavAutorisatieManager: IAutorisatieManager
    {

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">Lijst met ID's van gelieerde personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van personen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="personenIDs">Lijst met ID's van personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkJaar, het
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns><c>True</c> (enkel) als user supergav is</returns>
        /// <remarks></remarks>
        public bool IsSuperGav()
        {
            return true;
        }


        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        /// <remarks></remarks>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? String.Empty
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }
        
        public bool IsGav(Groep groep)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GroepsWerkJaar groepsWerkJaar)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GelieerdePersoon gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Deelnemer gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Plaats gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Uitstap gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GebruikersRecht gelieerdePersoon)
        {
            throw new NotImplementedException();
        }
        
        public bool IsGav(Lid gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
        {
            throw new NotImplementedException();
        }

        public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gebruikerses the recht gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoonID">The gelieerde persoon ID.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Determines whether the specified gebruikersrecht is verlengbaar.
        /// </summary>
        /// <param name="gebruikersrecht">The gebruikersrecht.</param>
        /// <returns><c>true</c> if the specified gebruikersrecht is verlengbaar; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            throw new NotImplementedException();
        }
    }
}
