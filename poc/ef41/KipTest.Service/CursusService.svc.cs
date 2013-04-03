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
using System.Linq;
using KipTest.ServiceContracts;
using KipTest.Workers;

namespace KipTest.Service
{
    public class CursusService : ICursusService
    {
        private readonly CursusManager _cursusManager;

        public CursusService(CursusManager cmgr)
        {
            // dependency injection

            _cursusManager = cmgr;
        }

        public int CursusMaken(string naam, DateTime startDatum, DateTime stopDatum)
        {
            var c = _cursusManager.Maken(naam, startDatum, stopDatum);
            _cursusManager.WijzigingenBewaren();
            return c.ID;
        }

        public int DeelnemerInschrijven(int cursusID, string deelnemerNaam)
        {
            var cursus = _cursusManager.Ophalen(cursusID);
            var deelnemer = _cursusManager.Inschrijven(cursus, deelnemerNaam);
            _cursusManager.WijzigingenBewaren();
            return deelnemer.ID;
        }

        public string[] DeelnemersOphalen(int cursusID)
        {
            return _cursusManager.DeelnemersOphalen(cursusID);
        }

        // Verwijdert alle deelnemers met de gegeven naam.
        // domme functie, maar POC
        public void DeelnemerVerwijderen(int cursusID, string naam)
        {
            // In praktijk zul je dit niet op deze manier implementeren.  Maar dit is
            // om aan te tonen dat je bij het bewaren van je aangepaste situatie geen
            // 'TeVerwijderen' en geen lambda-expressies meer nodig hebt.

            // Haal cursus op met deelnemers
            var cursus = _cursusManager.Ophalen(cursusID, true);

            var pineuten = (from d in cursus.Deelnemers
                            where String.Compare(d.Naam, naam, true) == 0
                            select d).ToArray();

            _cursusManager.DeelnemersVerwijderen(pineuten);
            _cursusManager.WijzigingenBewaren();
        }

        // Verhuist deelnemers met gegeven naam naar een andere cursus
        public void DeelnemerVerhuizen(int cursusVanID, int cursusTotID, string naam)
        {
            // In praktijk doe je dit natuurlijk efficienter.  Maar het is een voorbeeld.

            // Haal cursus op met deelnemers
            var cursus1 = _cursusManager.Ophalen(cursusVanID, true);
            var cursus2 = _cursusManager.Ophalen(cursusTotID, true);

            var pineuten = (from d in cursus1.Deelnemers
                            where String.Compare(d.Naam, naam, true) == 0
                            select d).ToArray();

            _cursusManager.DeelnemersVerhuizen(pineuten, cursus2);
            _cursusManager.WijzigingenBewaren();
        }

        public string Hallo()
        {
            return "Hello world!";
        }
    }
}
