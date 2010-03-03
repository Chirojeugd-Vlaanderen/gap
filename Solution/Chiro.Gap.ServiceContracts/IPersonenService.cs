// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    [ServiceContract]
    interface IPersonenService
    {
        /// <summary>
        /// Eens ingelogd, kun je hiermee je huidige GAV-entity opvragen
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Gav OphalenGAV();

        /// <summary>
        /// Vanuit je huidige GAV-entity alle groepen opvragen waarvan je admin bent
        /// </summary>
		/// <param name="g">De GAV over wie het gaat</param>
        /// <returns>De GAV met de groepen waar hij/zij admin van is</returns>
        [OperationContract]
        Gav OphalenGavMetGroepen(Gav g);

        /// <summary>
        /// Een groep toevoegen aan de rechten van de gegeven GAV
        /// </summary>
		/// <param name="g">De GAV over wie het gaat</param>
        [OperationContract]
        void GroepAanGavToevoegen(Gav g);

        /// <summary>
        /// Zorgen dat een GAV geen rechten meer heeft voor een gegeven groep
        /// </summary>
        /// <param name="g">De GAV over wie het gaat</param>
        /// <param name="groepID">ID van de groep waar hij/zij van losgekoppeld wordt</param>
        [OperationContract]
        void GroepVanGavVerwijderen(Gav g, int groepID);
    }
}
