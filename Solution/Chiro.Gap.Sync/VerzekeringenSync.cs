// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Deze klasse staat in voor het overzetten van verzekeringsgegevens naar Kipadmin.
    /// </summary>
    public class VerzekeringenSync : IVerzekeringenSync
    {
        /// <summary>
        /// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
        /// </summary>
        /// <param name="persoonsVerzekering">Over te zetten persoonsverzekering</param>
        /// <param name="gwj">Bepaalt werkJaar en groep die factuur zal krijgen</param>
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
                throw new NotImplementedException();
            }
        }
    }
}
