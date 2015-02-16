using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

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
            throw new NotImplementedException();
        }
    }
}