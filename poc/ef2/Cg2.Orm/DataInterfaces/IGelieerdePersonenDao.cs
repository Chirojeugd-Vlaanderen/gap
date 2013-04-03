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
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Met een GelieerdePersoon moet altijd het geassocieerde
    /// persoonsobject meekomen, anders heeft het weinig zin.
    /// </summary>
    public interface IGelieerdePersonenDao: IDao<GelieerdePersoon>
    {
        /// <summary>
        /// Haalt de persoonsgegevens van alle gelieerde personen van een groep op.
        /// </summary>
        /// <param name="GroepID">ID van de groep</param>
        /// <returns>Lijst van gelieerde personen</returns>
        IList<GelieerdePersoon> AllenOphalen(int GroepID);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het gegeven werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="werkJaar">werkjaar waarvoor lidinfo op te halen</param>
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, int werkJaar, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
        /// eventueel lidobject in het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="pagina">paginanummer (1 of groter)</param>
        /// <param name="paginaGrootte">aantal records op een pagina</param>
        /// <param name="aantalOpgehaald">outputparameter die aangeeft hoeveel personen meegegeven zijn</param>
        /// <returns>Lijst met gelieerde personen</returns>
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt persoonsgegevens van een gelieerd persoon op, incl. adressen en communicatievormen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van op te halen gelieerde persoon</param>
        /// <returns>Gelieerde persoon met persoonsgegevens, adressen en communicatievormen</returns>
        GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Laadt groepsgegevens in GelieerdePersoonsobject
        /// </summary>
        /// <param name="p">gelieerde persoon</param>
        /// <returns>referentie naar p, nadat groepsgegevens
        /// geladen zijn</returns>
        GelieerdePersoon GroepLaden(GelieerdePersoon p);
    }
}
