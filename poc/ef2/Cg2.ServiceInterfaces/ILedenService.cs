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
    // NOTE: If you change the interface name "ILedenService" here, you must also update the reference to "ILedenService" in Web.config.
    [ServiceContract]
    public interface ILedenService
    {
        /// <summary>
        /// Een gelieerde persoon ophalen en die lid in het huidige werkjaar
        /// en bewaren in database.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>nieuw lidobject</returns>
        [OperationContract]
        Lid LidMakenEnBewaren(int gelieerdePersoonID);

        /// <summary>
        /// Haalt pagina met leden op uit bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepsWerkJaarID">gevraagde groepswerkjaar</param>
        /// <param name="pagina">paginanr (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal leden per pagina</param>
        /// <param name="aantalOpgehaald">hierin wordt bewaard hoeveel
        /// records er effectief opgehaald zijn</param>
        /// <returns>lijst met leden, inclusief info gelieerde personen
        /// en personen</returns>
        [OperationContract]
        IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Lid> LedenOphalenMetInfo(string name, IList<LidInfo> gevraagd); //andere searcharg


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Lid> LidOphalenMetInfo(int lidID, string name, IList<LidInfo> gevraagd); //andere searcharg


        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        [OperationContract]
        void LidBewaren(Lid lid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidOpNonactiefZetten(Lid lid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidActiveren(Lid lid);
    }
}
