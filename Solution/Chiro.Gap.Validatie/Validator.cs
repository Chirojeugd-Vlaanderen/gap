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

using Chiro.Gap.Domain;

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Klasse die gebruikt wordt om validatieregels te controleren
	/// </summary>
	/// <typeparam name="T">Object waarop de validator van toepassing is</typeparam>
	public abstract class Validator<T> : IValidator<T>
	{
		/// <summary>
		/// Valideert een object.  De bedoeling is dat er hier een aantal
		/// generieke zaken getest kunnen worden.  (Bijv. 'maxlengths'
		/// die gegeven zijn via attributen.)
		/// <para />
		/// Voorlopig zijn er zo nog geen attributen, dus retourneert
		/// deze functie gewoonweg <c>true</c>.
		/// </summary>
		/// <param name="teValideren">Object dat gevalideerd moet worden</param>
		/// <returns><c>True</c> indien validatie oké</returns>
		public virtual bool Valideer(T teValideren)
		{
            //return FoutNummer(teValideren) != null;
            //Arno: De regel hierboven heb ik vervangen door de code die er nu staat omdat de andere steeds een fout terug gaf, het kan zijn dat dit terug
            //      veranderd moet worden.
		    return FoutNummer(teValideren) == null;
		}

        /// <summary>
        /// Valideert een object.  De bedoeling is dat er hier een aantal
        /// generieke zaken getest kunnen worden.  (Bijv. 'maxlengths'
        /// die gegeven zijn via attributen.)
        /// </summary>
        /// <param name="teValideren">Object dat gevalideerd moet worden</param>
        /// <returns><c>null</c> indien validatie oké, anders een foutnummer met meer
        /// informatie over de opgetreden fout.</returns>
	    public abstract FoutNummer? FoutNummer(T teValideren);
	}
}
