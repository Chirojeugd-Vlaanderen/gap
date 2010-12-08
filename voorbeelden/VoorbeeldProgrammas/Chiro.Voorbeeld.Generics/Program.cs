using System;
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
