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
using System.Globalization;
using Chiro.Kip.Log;
using Chiro.Kip.ServiceContracts;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;

namespace Chiro.Kip.Services
{
	/// <summary>
	/// Klasse die persoons- en lidgegevens overzet van GAP naar Kipadmin.
	/// 
	/// BELANGRIJK: Oorspronkelijk werden voor de meeste methods geen personen over de lijn gestuurd, maar enkel
	/// AD-nummers.  Het idee daarachter was dat toch enkel gegevens van personen met AD-nummer naar kipadmin
	/// gesynct moeten worden.
	/// 
	/// Maar met het AD-nummer alleen kom je er niet.  Het kan namelijk goed zijn dat een persoon gewijzigd wordt
	/// tussen het moment dat hij voor het eerst lid wordt, en het moment dat hij zijn AD-nummer krijgt.  Deze
	/// wijzigingen willen we niet verliezen.
	/// 
	/// Het PersoonID van GAP meesturen helpt in de meeste gevallen.  Maar dat kan mis gaan op het moment dat een persoon
	/// uit kipadmin nog dubbel in GAP zit.  Vooraleer deze persoon zijn AD-nummer krijgt, weten we dat immers niet.
	/// 
	/// Vandaar dat nu alle methods volledige persoonsobjecten gebruiken, zodat het opzoeken van een persoon zo optimaal
	/// mogelijk kan gebeuren.  Het persoonsobject een AD-nummer heeft, wordt er niet naar de rest gekeken.
	/// 
	/// TODO: De mapping van Persoon naar PersoonZoekInfo zou beter ergens op 1 plaats gedefinieerd worden, ipv in 
	/// elke method apart.
	/// </summary>
	public partial class SyncPersoonService : ISyncPersoonService
	{
		private readonly IMiniLog _log;

		/// <summary>
		/// Kipadmin kent het onderscheid postnummer/postcode niet.  Deze
		/// domme functie plakt de twee aan elkaar.
		/// </summary>
		/// <param name="adres">Adres</param>
		/// <returns>combinatie postnummer/postcode van adres</returns>
		private static string KipPostNr(Adres adres)
        {
            // Sommige groepen die het verschil postcode/postnr niet snappen, typen in beide vakjes hetzelfde.
            // Dat filteren we er hier ook maar uit.

            // TODO: test of het resultaat van KipPostNr niet langer wordt dan 10 tekens.

		    string resultaat = (String.IsNullOrEmpty(adres.PostCode) ||
		                        String.CompareOrdinal(adres.PostNr.ToString(CultureInfo.InvariantCulture), adres.PostCode) == 0)
		                           ? adres.PostNr.ToString(CultureInfo.InvariantCulture)
		                           : String.Format(
		                               "{0} {1}",
		                               adres.PostNr,
		                               adres.PostCode);
            
            // Voor de zekerheid beperken tot 10 karakters.

            if (resultaat.Length > 10)
            {
                resultaat = resultaat.Substring(0, 10);
            }

		    return resultaat;
        }
	}
}
