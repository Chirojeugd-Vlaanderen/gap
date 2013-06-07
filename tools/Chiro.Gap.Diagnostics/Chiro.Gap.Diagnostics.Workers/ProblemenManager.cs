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

using Chiro.Cdf.Data;
using Chiro.Gap.Diagnostics.Data;
using Chiro.Gap.Diagnostics.Orm;
using Chiro.Gap.Diagnostics.Orm.DataInterfaces;

namespace Chiro.Gap.Diagnostics.Workers
{
    /// <summary>
    /// Backend-functionaliteit voor diagnostische doeleinden
    /// </summary>
    public class ProblemenManager
    {
        private readonly IVerlorenAdressenDao _verlorenAdressenDao;
        private readonly IFunctieProblemenDao _functieProblemenDao;
        private readonly IBivakProblemenDao _bivakProblemenDao;

        /// <summary>
        /// Standaardconstructor.  Data access wordt geinjecteerd.
        /// </summary>
        /// <param name="verlorenAdressenDao">Data access voor verloren adressen</param>
        /// <param name="functieProblemenDao">Data access voor functieproblemen</param>
        /// <param name="bivakProblemenDao">Data access voor bivakproblemen</param>
        public ProblemenManager(IVerlorenAdressenDao verlorenAdressenDao, IFunctieProblemenDao functieProblemenDao,
                                IBivakProblemenDao bivakProblemenDao)
        {
            _verlorenAdressenDao = verlorenAdressenDao;
            _functieProblemenDao = functieProblemenDao;
            _bivakProblemenDao = bivakProblemenDao;
        }

        /// <summary>
        /// Haalt de personen op die wel een adres hebben in Kipadmin, maar geen adres
        /// in GAP.
        /// </summary>
        /// <returns>Info over alle personen waarvan het adres verloren ging</returns>
        public IEnumerable<VerlorenAdres> VerdwenenAdressenOphalen()
        {
            return _verlorenAdressenDao.AllesOphalen();
        }

        /// <summary>
        /// Haalt alle functie-inconsistenties op voor het huidige werkjaar
        /// </summary>
        /// <returns>Rijen informatie voor inconsistenties tussen functies in het GAP
        /// en functies in Kipadmin.</returns>
        public IEnumerable<FunctieProbleem> FunctieProblemenOphalen()
        {
            return _functieProblemenDao.AllesOphalen();
        }

        /// <summary>
        /// Haalt de groepen op die wel een bivak hebben in GAP, maar waarvoor de delphi-client
        /// geen bivakaangifte toont in Kipadmin.
        /// </summary>
        /// <returns>Informatie over de problematische bivakaangiften.</returns>
        public IEnumerable<VerlorenBivak> BivakProblemenOphalen()
        {
            return _bivakProblemenDao.AllesOphalen();
        }
    }
}
