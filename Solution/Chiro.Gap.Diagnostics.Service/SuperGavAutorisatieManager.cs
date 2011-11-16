// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Autorisatiemanager voor de synchronisatie kipadmin->gap, die
    /// eigenlijk alleen maar zegt: 'Ik ben supergav!'.  De rest is niet
    /// geimplementeerd, en dat houden we voorlopig zo.
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
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            throw new NotImplementedException();
        }

        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroep(int groepID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroepen(IEnumerable<int> groepIDs)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdeling(int afdelingsID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdelingsJaar(int afdelingsJaarID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavLid(int lidID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCategorie(int categorieID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCommVorm(int commvormID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavFunctie(int functieID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoonsAdres(int persoonsAdresID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
        {
            throw new NotImplementedException();
        }

        public bool IsGavUitstap(int uitstapID)
        {
            throw new NotImplementedException();
        }

        public bool IsSuperGav()
        {
            return true;
        }

        public string GebruikersNaamGet()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            throw new NotImplementedException();
        }

        public bool IsGavPlaats(int plaatsID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavDeelnemer(int deelnemerID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            throw new NotImplementedException();
        }

        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            throw new NotImplementedException();
        }
    }
}
