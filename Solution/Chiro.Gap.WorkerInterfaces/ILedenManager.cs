/*
 * Copyright 2008-2013, 2015, 2016, 2017 the GAP developers.
 * See the NOTICE file at the top-level directory of this distribution, 
 * and at https://gapwiki.chiro.be/copyright
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

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface ILedenManager
    {
        /// <summary>
        /// Doet een voorstel voor de inschrijving van de gegeven gelieerdepersoon <paramref name="gp"/> in groepswerkjaar 
        /// <paramref name="gwj"/>
        /// <para />
        /// Als de persoon in een afdeling past, krijgt hij die afdeling. Als er meerdere passen, wordt er een gekozen.
        /// Als de persoon niet in een afdeling past, en <paramref name="leidingIndienMogelijk"/> <c>true</c> is, 
        /// wordt hij leiding als hij oud genoeg is.
        /// Anders wordt een afdeling gekozen die het dichtst aanleunt bij de leeftijd van de persoon.
        /// Zijn er geen afdelingen, dan wordt een exception opgeworpen.
        /// </summary>
        /// <param name="gp">
        /// De persoon om in te schrijven, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="leidingIndienMogelijk">Als deze <c>true</c> is, dan stelt de method voor om een persoon
        /// leiding te maken als er geen geschikte afdeling is, en hij/zij oud genoeg is.</param>
        /// <returns>
        /// Voorstel tot inschrijving
        /// </returns>
        LidVoorstel InschrijvingVoorstellen(GelieerdePersoon gp, GroepsWerkJaar gwj, bool leidingIndienMogelijk);

        /// <summary>
        /// Schrijft een gelieerde persoon in, persisteert niet.  Er mag nog geen lidobject (ook geen inactief) voor de
        /// gelieerde persoon bestaan.
        /// </summary>
        /// <param name="voorstelLid">
        ///     Voorstel voor de eigenschappen van het in te schrijven lid.
        /// </param>
        /// <param name="isJaarOvergang">
        ///     Geeft aan of het over de automatische jaarovergang gaat; relevant voor de
        ///     probeerperiode
        /// </param>
        /// <returns>
        /// Het aangemaakte lid object
        /// </returns>
        Lid NieuwInschrijven(LidVoorstel voorstelLid, bool isJaarOvergang);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// (kind)lid kan worden, d.w.z. dat hij qua (Chiro)leeftijd in een afdeling past.
        /// </summary>
        /// <param name="gelieerdePersoon">een gelieerde persoon</param>
        /// <returns><c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// lid kan worden, d.w.z. dat hij qua (Chiro)leeftijd in een afdeling past.</returns>
        bool KanInschrijvenAlsKind(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Geeft <c>true</c> als er in de Civi een betalende aansluiting bestaat voor het gegeven lid
        /// <paramref name="l"/> (eventueel via dezelfde persoon in een andere groep).
        /// </summary>
        /// <param name="l">Een lid</param>
        /// <returns><c>true</c> als er in de Civi een betalende aansluiting bestaat voor het gegeven lid
        /// <paramref name="l"/> (eventueel via dezelfde persoon in een andere groep).</returns>
        /// <remarks>Dit wordt bepaald o.m. op basis van het IsAangesloten-veld van de leden.</remarks>
        bool IsBetalendAangesloten(Lid l);

        /// <summary>
        /// Geeft <c>true</c> als er in de Civi al een aansluiting bestaat voor het gegeven <paramref name="lid"/>
        /// (typisch via dezelfde persoon in een andere groep).
        /// </summary>
        /// <param name="lid">Een lid</param>
        /// <returns>
        /// <c>true</c> als er in de Civi al een aansluiting bestaat voor het gegeven <paramref name="lid"/>.
        /// </returns>
        /// <remarks>We gaan hiervoor niet in Civi kijken, maar wel naar het IsAangesloten-veld van de leden.</remarks>
        bool IsAangesloten(Lid lid);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// leiding kan worden. Dit hangt eigenlijk enkel van de leeftijd af.
        /// </summary>
        /// <param name="gelieerdePersoon">een gelieerde persoon</param>
        /// <returns><c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// leiding kan worden.</returns>
        bool KanInschrijvenAlsLeiding(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// (kind)lid in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// (kind)lid in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        bool IsActiefKind(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        bool IsActieveLeiding(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn
        /// groep, wordt het lidID opgeleverd, zo niet <c>null</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Het lidID als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar
        /// van zijn groep, anders <c>null</c>.
        /// </returns>
        int? LidIDGet(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn groep, dan
        /// levert deze method het overeenkomstige lidobject op. In het andere geval <c>null</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn groep, dan
        /// levert deze method het overeenkomstige lidobject op. In het andere geval <c>null</c>.
        /// </returns>
        Lid HuidigLidGet(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Zoekt een afdelingsjaar van het recentste groepswerkjaar, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> (kind)lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.
        /// </summary>
        /// <param name="gelieerdePersoon">gelieerde persoon waarvoor we een afdeling zoeken</param>
        /// <returns>een afdelingsjaar van het recentste groepswerkjaar, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.</returns>
        AfdelingsJaar AfdelingsJaarVoorstellen(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// kind of leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als kind
        /// of leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        bool IsActiefLid(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Verandert het lidtype van <paramref name="origineelLid"/> van
        /// <c>Kind</c> naar <c>Leiding</c> of omgekeerd
        /// </summary>
        /// <param name="origineelLid">Lid waarvan type veranderd moet worden</param>
        /// <returns>Nieuw lid, met ander type</returns>
        /// <remarks>Het origineel lid moet door de caller zelf uit de repository verwijderd worden.</remarks>
        Lid TypeToggle(Lid origineelLid);

        /// <summary>
        /// Vervang de afdelingsjaren van gegeven <paramref name="lid"/> door de 
        /// gegeven <paramref name="afdelingsJaren"/>/
        /// </summary>
        /// <param name="lid">lid waarvan afdelingsjaren te vervangen</param>
        /// <param name="afdelingsJaren">nieuwe afdelingsjaren voor <paramref name="lid"/></param>
        /// <remarks>als <paramref name="lid"/> een kindlid is, dan moet <paramref name="afdelingsJaren"/>
        /// precies 1 afdelingsjaar bevatten.</remarks>
        void AfdelingsJarenVervangen(Lid lid, IList<AfdelingsJaar> afdelingsJaren);

        /// <summary>
        /// Geeft <c>true</c> als de probeerperiode van het gegeven <paramref name="lid"/> voorbij is,
        /// anders <c>false</c>.
        /// </summary>
        /// <param name="lid">Een lid waarvan de probeerperiode getest moet worden.</param>
        /// <returns><c>true</c> als de probeerperiode van het gegeven <paramref name="lid"/> voorbij is,
        /// anders <c>false</c>.</returns>
        bool ProbeerPeriodeVoorbij(Lid lid);

        /// <summary>
        /// Bepaalt of een lid gratis kan aansluiten.
        /// </summary>
        /// <param name="lid">Een lid.</param>
        /// <returns><c>true</c> als dat lid gratis aangesloten kan worden.</returns>
        bool GratisAansluiting(Lid lid);

        /// <summary>
        /// Levert het stamnummer van een lid op.
        /// </summary>
        /// <param name="lid">Lid waarvan we het stamnummer willen weten.</param>
        /// <returns>Het stamnummer van dat lid.</returns>
        string StamNummer(Lid lid);

        /// <summary>
        /// Haal alle leden op die nog aangesloten moeten worden voor werkjaar <paramref name="werkjaar"/>
        /// op de dag gegeven in <paramref name="vandaag"/>.
        /// </summary>
        /// <param name="lidQueryable">Queryable om leden in te zoeken.</param>
        /// <param name="werkjaar">Werkjaar waarvoor leden te zoeken.</param>
        /// <param name="vandaag">De datum van vandaag.</param>
        /// <param name="limit"></param>
        /// <returns>Een array met leden.</returns>
        Lid[] AanTeSluitenLedenOphalen(IQueryable<Lid> lidQueryable, int werkjaar, DateTime vandaag, int? limit);

        /// <summary>
        /// Levert <c>true</c> als het gegeven <paramref name="lid"/> een lid is van het huidige werkjaar
        /// van zijn groep.
        /// </summary>
        /// <param name="lid"></param>
        /// <returns>
        /// Levert <c>true</c> als het gegeven <paramref name="lid"/> een lid is van het huidige werkjaar
        /// van zijn groep.
        /// </returns>
        bool IsVanHuidigWerkjaar(Lid lid);
    }
}