/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
    [ServiceContract]
    public interface IGelieerdePersonenService
    {
        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Zoekt alle personen die aan de criteria voldoen en geeft daarvan een bepaalde pagina weer
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte);
        //... andere zoekmogelijkheden

        /// <summary>
        /// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
        [OperationContract]
        GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID);
        
        /// <summary>
        /// Haalt gelieerd persoon op met extra gevraagde info.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <param name="gevraagd">Stelt voor welke informatie opgehaald moet worden</param>
        /// <returns>GelieerdePersoon uitbreiden met meer info mbt het gevraagde onderwerp </returns>
        [OperationContract(Name = "PersoonOphalenMetCustomDetails")]
        GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd);

        /// <summary>
        /// Bewaart nieuwe/gewijzigde gelieerde persoon
        /// </summary>
        /// <param name="persoon">Te bewaren persoon</param>
        [OperationContract]
        void PersoonBewaren(GelieerdePersoon persoon);
    }
}
