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
using System.Data;
using System.Runtime.Serialization;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Dit is de basisklasse voor alle entiteiten in het model.
    /// Het is een abstracte klasse; je kan enkel subklasses instantieren.
    /// 
    /// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
    /// </summary>
    ///
    [DataContract]
    public abstract class BasisEntiteit
    {
        public const int DefaultID = 0;

        private int _id = DefaultID;
        private byte[] _versie;
        private bool _teVerwijderen = false;
        

        [DataMember]
        public virtual int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public virtual byte[] Versie
        {
            get { return _versie; }
            set { _versie = value;  }
        }

        [DataMember]
        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public BasisEntiteit()
        {
        }

        /// <summary>
        /// Vergelijkt deze entiteit met een andere.  Twee entiteiten zijn
        /// gelijk als geen van beide null is, ze beiden hetzelfde ID hebben
        /// of beiden 'transient' zijn met dezelfde 'business signature'.
        /// </summary>
        /// <param name="obj">Entiteit om mee te vergelijken</param>
        /// <returns>True indien deze entiteit dezlefde is als 'obj'.</returns>
        public sealed override bool Equals(object obj)
        {
            BasisEntiteit teVergelijken = obj as BasisEntiteit;

            return (teVergelijken != null) 
                && (HeeftZelfdeNietStandaardId(teVergelijken)
                || ((IsTransient() || teVergelijken.IsTransient())
                    && HeeftZelfdeBusinessSignature(teVergelijken)));
        }

        /// <summary>
        /// Controleert of de entiteit 'transient' is.  Een 'transiente'
        /// entiteit is niet gekoppeld met de database, en heeft geen ID.
        /// </summary>
        /// <returns>True voor transiente entiteiten</returns>
        public bool IsTransient()
        {
            return ID == DefaultID;
        }

        /// <summary>
        /// Dit is overgenomen uit het boek.  Maar aangezien we iets verderop
        /// 'ToString' overriden, denk ik dat alle entity's van hetzelfde type
        /// zo dezelfde hashcode zullen krijgen.  Zou dat de bedoeling zijn?
        /// 
        /// Misschien moeten de entiteiten zelf ToString nog eens overriden...
        /// </summary>
        /// <returns>een hashcode voor deze entiteit.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Vergelijkt de hashcode van deze entiteit met die van een andere.
        /// </summary>
        /// <param name="teVergelijken">entiteit waarmee vergeleken moet 
        /// worden</param>
        /// <returns>true indien beide entiteiten dezelfde hashcode 
        /// hebben.</returns>
        private bool HeeftZelfdeBusinessSignature(BasisEntiteit teVergelijken)
        {
            return GetHashCode().Equals(teVergelijken.GetHashCode());
        }

        /// <summary>
        /// Controleert of deze entiteit hetzelfde 'echte' (niet 0 of null) ID
        /// heeft als een andere entiteit.
        /// </summary>
        /// <param name="teVergelijken">entiteit waarvan ID vergeleken moet
        /// worden</param>
        /// <returns>true indien ID's gelijk en niet 0/null zijn</returns>
        private bool HeeftZelfdeNietStandaardId(BasisEntiteit teVergelijken)
        {
            return (ID != DefaultID)
                && (teVergelijken.ID != DefaultID)
                && ID == teVergelijken.ID;
        }

        /// <summary>
        /// Bepaal de stringrepresentatie van entiteit.
        /// </summary>
        /// <returns>de klassenaam van het object</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            // FIXME: Onderstaande lijn is nog altijd niet goed genoeg,
            // want deze ToString wordt bepaald aan de hand van de
            // hashcode, en die wordt dan weer gebruikt om te kijken of
            // objecten met ID 0 verschillend zijn.  Met onderstaande
            // 'ToString' krijgen objecten met ID 0 steeds dezelfde
            // hashcode.

            str.Append(" Klasse: ").Append(GetType().FullName).Append(ID.ToString());
            return str.ToString();
        }
    }
}
