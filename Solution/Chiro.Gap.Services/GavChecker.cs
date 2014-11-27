/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
﻿using System.Linq;

using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    public class GavChecker
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public GavChecker(IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
        }

        public void Check(Afdeling g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.ChiroGroep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Uitstap g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Deelnemer g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV is voor de gegeven <paramref name="functie"/>,
        /// en throwt de GeenGav-FaultException indien niet.
        /// </summary>
        /// <param name="functie">te controleren functie</param>
        public void Check(Functie functie)
        {
            if (functie == null ||  !(functie.IsNationaal || _autorisatieMgr.IsGav(functie.Groep)))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Categorie g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(GelieerdePersoon g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Persoon p)
        {
            if (p == null || !_autorisatieMgr.IsGav(p))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Lid g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(AfdelingsJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.GroepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(GroepsWerkJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Groep g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void CheckSuperGav()
        {
            if (!_autorisatieMgr.IsSuperGav())
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }
    }
}