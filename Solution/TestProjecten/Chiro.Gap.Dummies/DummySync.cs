// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Gauw een klasse die gebruikt kan worden om eender welke Sync te mocken.
    /// </summary>
    public class DummySync : IAdressenSync, ICommunicatieSync, IPersonenSync, ILedenSync, IDubbelpuntSync, IVerzekeringenSync, IBivakSync, IGroepenSync
    {
        public void StandaardAdressenBewaren(IEnumerable<PersoonsAdres> persoonsAdressen)
        {
        }

        public void Verwijderen(CommunicatieVorm communicatieVorm)
        {
        }

        public void Toevoegen(CommunicatieVorm commvorm)
        {
        }

        public void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
        {
            if (gp == null)
            {
                throw new ArgumentNullException("gp");
            }
        }

        public void CommunicatieUpdaten(GelieerdePersoon gp)
        {
        }

        public void Bewaren(Lid l)
        {
        }

        public void FunctiesUpdaten(Lid lid)
        {
        }

        public void AfdelingenUpdaten(Lid lid)
        {
        }

        public void TypeUpdaten(Lid lid)
        {
        }

        public void Verwijderen(Lid lid)
        {
        }

        public void Abonneren(GelieerdePersoon gp)
        {
        }

        public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
        {
        }

    	public void Bewaren(Uitstap uitstap)
    	{
    	}

    	public void Verwijderen(int uitstapID)
    	{
    	}

        public void Abonneren(Abonnement abonnement)
        {
        }

        public void Bewaren(Groep g)
        {
        }
    }
}
