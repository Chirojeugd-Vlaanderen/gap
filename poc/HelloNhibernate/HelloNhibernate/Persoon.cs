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
using Iesi.Collections.Generic;

namespace HelloNhibernate
{
    public enum GeslachtsType 
    {
        Man = 1, 
        Vrouw = 2, 
        Onbekend = 0
    };

    /// <summary>
    /// Klasse voor 'persoon'.  De bedoeling is dat elke individuele persoon
    /// slechts 1 keer bestaat.  Als een groep echter een nieuwe persoon
    /// maakt, en die persoon bestaat al, dan weet die groep dat soms nog
    /// niet.  De bedoeling is dat het systeem dat meldt aan het secretariaat,
    /// zodat ze daar de dubbele personen eventueel kunnen mergen.
    /// </summary>
    /// 
    public class Persoon
    {
        #region Private members
        private int _id;
        private byte[] _versie;
        private int? _adNummer;
        private string _naam;
        private string _voorNaam;
        private int _geslacht;
        private DateTime? _geboorteDatum;
        private DateTime? _sterfDatum;
        private ISet<CommunicatieVorm> _communicatie = new HashedSet<CommunicatieVorm>();
        #endregion

        #region Properties

        public int ID
        {
            get { return _id; }
        }

        public byte[] Versie
        {
            get { return _versie; }
            set { _versie = value; }
        }

        public int? AdNummer
        {
            get { return _adNummer; }
            set { _adNummer = value; }
        }

        public string Naam
        {
            get { return _naam; }
            set { _naam = value; }
        }

        public string VoorNaam
        {
            get { return _voorNaam; }
            set { _voorNaam = value; }
        }

        public int GeslachtsInt
        {
            get { return _geslacht; }
            set { _geslacht = value; }
        }

        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        public DateTime? GeboorteDatum
        {
            get { return _geboorteDatum; }
            set { _geboorteDatum = value; }
        }

        public DateTime? SterfDatum
        {
            get { return _sterfDatum; }
            set { _sterfDatum = value; }
        }

        public ISet<CommunicatieVorm> Communicatie
        {
            get { return _communicatie; }
        }
        
        #endregion

        /// <summary>
        /// Nieuwe communicatievorm toevoegen voor een persoon
        /// </summary>
        /// <param name="type">communicatietype</param>
        /// <param name="nr">nr/url/...</param>
        /// <param name="voorkeur">true indien dit het voorkeursnummer voor
        /// het gegeven type is.</param>
        public void CommunicatieToevoegen(CommunicatieType type, string nr
            , bool voorkeur)
        {
            CommunicatieVorm cv = new CommunicatieVorm();
            cv.Nummer = nr;
            cv.Type = type;
            cv.Persoon = this;
            cv.Voorkeur = voorkeur;

            _communicatie.Add(cv);
        }

        public string Hallo()
        {
            return string.Format("Hallo, {0} {1}!", VoorNaam, Naam);
        }
    }
}
