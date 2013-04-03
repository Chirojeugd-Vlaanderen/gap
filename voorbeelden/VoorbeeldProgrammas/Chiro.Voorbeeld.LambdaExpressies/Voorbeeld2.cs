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

namespace Chiro.Voorbeeld.LambdaExpressies
{
	// Dit voorbeeld toont hoe je lambda-expressies kan gebruiken om
	// property's van een object te selecteren.  (Indien niet vertrouwd
	// met generics, best ook daar het voorbeeld eens bekijken)
	public class Voorbeeld2
	{
		private class Persoon
		{
			public string Naam { get; set; }
			public int Leeftijd { get; set; }
		}

		// Deze functie kan gebruikt worden om een member
		// van een klasse af te drukken, via een lambda-expressie
		private static void ToonProperty<T>(
			T bron,
			Func<T,object> selecteer)
		{
			Console.WriteLine(selecteer(bron));
		}

		// Deze test gebruikt lambda-expressies om ToonProperty te
		// laten werken op de properties van een klasse.
		public static void Uitvoeren()
		{
			Persoon p = new Persoon {Naam = "Johan", Leeftijd = 33};

			Console.WriteLine("Naam tonen van persoon:");
			ToonProperty(p, prs => prs.Naam);

			Console.WriteLine("Leeftijd tonen van een persoon");
			ToonProperty(p, prs => prs.Leeftijd);

			// Nadeel van deze manier van werken, is dat je er geen
			// controle over hebt of het tweede argument wel degelijk
			// een property van het eerste bepaalt.  In onderstaand
			// geval levert de lambda-expressie iets op dat helemaal 
			// niets met het eerste argument te maken heeft.

			Console.WriteLine("Louche dings...");
			ToonProperty(p, prs => System.AppDomain.CurrentDomain.BaseDirectory);
		}
	}
}
