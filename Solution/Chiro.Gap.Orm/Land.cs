﻿using System;

using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
    /// <summary>
    /// Een land
    /// </summary>
    /// <remarks>
    /// Een land heeft geen versie (timestamp) in de database.
    /// Dat lijkt me ook niet direct nodig voor een klasse die
    /// bijna nooit wijzigt.
    /// <para/>
    /// Het feit dat er geen timestamp is, wil wel zeggen dat
    /// 'concurrencygewijze' de laatste altijd zal winnen.    
    /// </remarks>
    public partial class Land : IEfBasisEntiteit
    {
        private bool _teVerwijderen;

        /// <summary>
        /// Wordt gebruikt om te verwijderen entiteiten mee te markeren
        /// </summary>
        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        /// <summary>
        /// Geeft stringrepresentatie van Versie weer (hex).
        /// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
        /// </summary>
        /// <remarks>Een land wordt nooit geüpdatet, dus zijn er geen problemen met
        /// concurrency.  VersieString is dus niet nodig.</remarks>
        public string VersieString
        {
            get { return null; }
            set { /*Doe niets*/ }
        }

        /// <summary>
        /// De byte-representatie van Versie
        /// </summary>
        /// <remarks>Een land wordt nooit geüpdatet, dus zijn er geen problemen met
        /// concurrency.  Versie is dus niet nodig.</remarks>
        public byte[] Versie
        {
            get { return null; }
            set { /*Doe niets*/ }
        }

        /// <summary>
        /// Een waarde waarmee we het object kunnen identificeren,
        /// overgenomen van de ID
        /// </summary>
        /// <returns>Een int waarmee we het object kunnen identificeren</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Vergelijkt het huidige object met een ander om te zien of het over
        /// twee instanties van hetzelfde object gaat
        /// </summary>
        /// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
        /// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
        public override bool Equals(object obj)
        {
            IEfBasisEntiteit andere = obj as VerzekeringsType;
            // Als obj geen VerzekeringsType is, wordt andere null.

            return andere != null && ((ID != 0) && (ID == andere.ID)
                || (ID == 0 || andere.ID == 0) && base.Equals(andere));

            // Is obj geen Verzekeringstype, dan is de vergelijking altijd vals.
            // Hebben beide objecten een ID verschillend van 0, en zijn deze
            // ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
            // base.Equals, wat eigenlijk niet helemaal correct is.
        }
    }
}