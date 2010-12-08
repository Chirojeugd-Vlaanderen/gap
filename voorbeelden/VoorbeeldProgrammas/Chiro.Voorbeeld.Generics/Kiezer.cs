using System;
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
