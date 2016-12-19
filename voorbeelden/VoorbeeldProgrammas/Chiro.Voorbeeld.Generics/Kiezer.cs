/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
	// Klasse die een willekeurige keuze maakt uit objecten
	// van het generieke type T
	public class Kiezer<T>
	{
		private static Random _rnd;

		// De statische constructor wordt aangeroepen op het moment
		// dat de klasse gemaakt wordt.  (Niet bij iedere aanmaak van
		// een object.)  Dat is het uitgelezen moment om het statische
		// member te initialiseren.
		static Kiezer()
		{
			_rnd = new Random(DateTime.Now.GetHashCode());
		}

		// Deze functie verwacht 2 objecten van het type T,
		// kiest er daar willekeurig 1 uit, en geeft die terug.
		public T Kies(T optie1, T optie2)
		{
			// genereer een willekeurige int kleiner dan 2.  Is het
			// 0, dan kiezen we optie 1.  Anders optie 2.

			return _rnd.Next(2) == 0 ? optie1 : optie2;
		}
	}
}
