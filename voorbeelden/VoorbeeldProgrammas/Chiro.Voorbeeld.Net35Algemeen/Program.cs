﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Voorbeeld.Net35Algemeen
{
	class Program
	{
		internal class Bier
		{
			public string Naam { get; set; }
			public double Alcohol { get; set; }
		}

		static void Main(string[] args)
		{
			// Initialisatie: Onderstaande lijn
			Bier b1 = new Bier {Naam = "Hopus", Alcohol = 8.3};
			// is equivalent met

			//   Bier b1 = new Bier();
			//   b1.Naam = "Hopus";
			//   b1.Alcohol = 8.3;


			// Je kan ook in de declaratie 'Bier' vervangen door 'var'
			var b2 = new Bier {Naam = "Barbãr", Alcohol = 8};
			// is equivalent met

			//   Bier b2 = new Bier {Naam = "Barbãr", Alcohol = 8};
			// De compiler leidt zelf af dat het type van b2 'Bier' moet zijn.

			Bier b3 = b2; // de compiler weet dat het dezelfde types zijn, en doet dus niet moeilijk.

			// Je kan 'var' ook gebruiken voor 'anonieme types'.  Deze kan je wel enkel
			// binnen hun scope blijven gebruiken.

			var bierinfo = new {Naam = b2.Naam, Omschrijving = String.Format("{0} ({1}%)", b2.Naam, b2.Alcohol)};

			// 'bierinfo' heeft nu property's Naam en Omschrijving.  Het type van bierinfo is
			// automatisch gegenereerd.  Je kan de velden accessen op de verwachte manier:

			Console.WriteLine(String.Format("Naam: {0}", bierinfo.Naam));
			Console.WriteLine(String.Format("Omschrijving: {0}", bierinfo.Omschrijving));

			// Eens kijken wat het type van bierinfo is: (niet dat het ertoe doet)

			Console.WriteLine(String.Format("Naam van het 'anonieme type': {0}", bierinfo.GetType()));

			Console.ReadLine();
		}
	}
}