// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Service die updates doorgeeft van GAP naar KipAdmin of omgekeerd
    /// </summary>
    public class UpdateService : IUpdateService
	{
        /// <summary>
        /// Stelt het AD-nummer van de persoon met ID <paramref name="persoonID"/> in.  
        /// </summary>
        /// <param name="persoonID">
        /// ID van de persoon
        /// </param>
        /// <param name="adNummer">
        /// Nieuw AD-nummer
        /// </param>
        public void AdNummerToekennen(int persoonID, int adNummer)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
        /// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
        /// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
        /// </summary>
        /// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
        /// <param name="nieuwAd">Nieuw AD-nummer</param>
        public void AdNummerVervangen(int oudAd, int nieuwAd)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Synct alle leden van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen leden</param>
        public void OpnieuwSyncen(string stamNummer)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Synct alle abonnementen van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen abonnementen</param>
        /// <remarks>Dit is eigenlijk geen sync van Kipadmin naar GAP, maar een vraag van Kipadmin
        /// aan GAP om bepaalde zaken opnieuw te syncen.  Eigenlijk staat dit dus niet op zijn
        /// plaats in deze service.  Maar voorlopig staat het hier, omdat UpdateService de
        /// enige manier is om communicatie van KIP naar GAP te arrangeren.</remarks>
        public void AbonnementenOpnieuwSyncen(string stamNummer)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }
	}
}