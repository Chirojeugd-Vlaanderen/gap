using System;
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
