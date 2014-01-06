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
	// Dit voorbeeld laat zien hoe je m.b.v. lambda-expressies op 
	// eenvoudige manier een delegate kan maken.
	public class Voorbeeld1
	{
		// Een eenvoudige functie die een gewicht in pounds omzet
		// naar een gewicht in kilogram.
		private static double PoundsNaarKg(double gewichtInPounds)
		{
			return gewichtInPounds*0.45359237;
		}

		// Het eigenlijke voorbeeldprogramma.
		public static void Uitvoeren()
		{
			// Definieer de 'delegate' conversie.
			// Een 'delegate' is een variabele waar je een functie
			// of een method aan kan toekennen.  In dit geval is
			// conversie een functie, die als argument een double
			// heeft, en als resultaat een double aflevert.

			Func<double, double> conversie;

			// De 'signature' van PoundsNaarKg (een double als argument,
			// een double als resultaat) is van die aard dat PoundsNaarKg
			// toegekend kan worden aan de delegate conversie.

			conversie = Voorbeeld1.PoundsNaarKg;

                        // Een delegate kan op dezelfde manier aangeroepen worden
			// als een gewone functie of method.

			double resultaat = conversie(100);

			Console.WriteLine(
				"100 pounds komt overeen met {0} kg.",
				resultaat);

			// In plaats van ergens in je code een hele functie
			// te definieren, die je misschien maar 1 keer nodig hebt
			// om aan een delegate toe te kennen, kan je deze functie
			// ook verkort definieren m.b.v. een 'lambda-expressie'.

			Func<double, double> conversie2 = (fahr => (fahr - 32) * 5 / 9);

			// Bovenstaande lambda-expressie is een functie met als
			// argument de double fahr.  Als resultaat levert ze
			// de double (fahr - 32) * 5 / 9 op.

			// Oproepen gebeurt op dezelfde manier:

			Console.WriteLine(
				"0 Fahrenheit komt overeen met {0} graden Celcius.\n\n",
				conversie2(0));
		}
	}
}
