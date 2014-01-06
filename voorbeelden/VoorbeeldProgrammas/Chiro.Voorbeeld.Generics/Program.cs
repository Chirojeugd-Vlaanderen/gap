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

namespace Chiro.Voorbeeld.Generics
{
	class Bier
	{
		public string Naam { get; set; }
		public string Brouwerij { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			// Maak een Kiezer die werkt op objecten van het type Bier.
			var bierKiezer = new Kiezer<Bier>();

			// Die 'var' staat er enkel om minder te moeten typen :)
			// De compiler leidt zelf af wat er moet staan
			// (in dit geval is dat Kiezer<Bier>).


			// Definieer 2 bieren

			Bier bier1 = new Bier {Naam = "Brugse Bok", Brouwerij = "De halve maan"};
			Bier bier2 = new Bier {Naam = "Manenblusser", Brouwerij = "'t Anker"};

			// Kies er 1 uit m.b.v. de Kiezer<Bier>.

			Bier keuze = bierKiezer.Kies(bier1, bier2);
			
			Console.WriteLine(
				"Vandaag drinken we {0}.\nDruk <ENTER>.",
				keuze.Naam);

			// Wacht op Enter om te vermijden dat Windows de terminal
			// sluit vooraleer je de output hebt gezien.

			Console.ReadLine();

		}
	}
}
