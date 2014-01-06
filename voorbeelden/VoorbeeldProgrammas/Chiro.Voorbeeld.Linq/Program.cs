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

namespace Chiro.Voorbeeld.Linq
{
	class Program
	{
		internal class Bier
		{
			public string Naam { get; set; }
			public double Alcohol { get; set; }
		}

		internal class Brouwerij
		{
			public string Naam { get; set; }
			public string Plaats { get; set; }
			public Bier[] Bieren { get; set; }
			public bool Trappist { get; set; }
		}

		static void Main(string[] args)
		{
			#region voorzie wat testdata

			Brouwerij b1 = new Brouwerij
			               	{
			               		Naam = "Zennebrouwerij",
			               		Plaats = "Brussel",  // niet zeker of ze daar nu al wel zitten :)
			               		Bieren = new Bier[]
			               		         	{
			               		         		new Bier {Naam = "Zinne Bir", Alcohol = 6},
			               		         		new Bier {Naam = "Taras Boulba", Alcohol = 4.5},
			               		         		new Bier {Naam = "Equinox", Alcohol = 8}
			               		         	},
						Trappist = false
			               	};

			Brouwerij b2 = new Brouwerij
			               	{
			               		Naam = "Proefbrouwerij",
			               		Plaats = "Lochristi",
			               		Bieren = new Bier[]
			               		         	{
			               		         		new Bier {Naam = "Vicaris Kerst", Alcohol = 10},
			               		         		new Bier {Naam = "Kempisch Vuur 3-dubbel", Alcohol = 7.5},
			               		         		new Bier {Naam = "Polderken", Alcohol = 6.5}
									// en vele andere :)
			               		         	},
						Trappist = false
			               	};

			Brouwerij b3 = new Brouwerij
			               	{
			               		Naam = "De Ranke",
			               		Plaats = "Dottignies",
			               		Bieren = new Bier[]
			               		         	{
			               		         		new Bier {Naam = "Père Noel", Alcohol = 7},
			               		         		new Bier {Naam = "XX Bitter", Alcohol = 6.2}
			               		         	},
						Trappist = false
			               	};

			Brouwerij b4 = new Brouwerij
			               	{
			               		Naam = "Saint-Rémy",
			               		Plaats = "Rochefort",
			               		Bieren = new Bier[]
			               		         	{
			               		         		new Bier {Naam = "Rochefort 6", Alcohol = 7.5},
			               		         		new Bier {Naam = "Rochefort 8", Alcohol = 9.2},
			               		         		new Bier {Naam = "Rochefort 10", Alcohol = 11.3}
			               		         	},
						Trappist = true
			               	};

			Brouwerij b5 = new Brouwerij
			               	{
			               		Naam = "St.-Sixtus",
			               		Plaats = "Westvleteren",
			               		Bieren = new Bier[]
			               		         	{
			               		         		new Bier {Naam = "Westvleteren Blond", Alcohol = 5.8},
			               		         		new Bier {Naam = "Westvleteren Acht", Alcohol = 8},
			               		         		new Bier {Naam = "Westvletetren Twaalf", Alcohol = 10.2}
			               		         	},
			               		Trappist = true
			               	};

			var brouwerijen = new List<Brouwerij> {b1, b2, b3, b4, b5};
			#endregion

			// Zoek de namen van alle trappistenbrouwerijen op

			var query = from brouw in brouwerijen
			            where brouw.Trappist == true
			            select brouw.Naam;

			// Equivalente syntax:
			//  query = brouwerijen.Where(br => br.Trappist == true).Select(br => br.Naam)

			// Op dit moment is query nog niet geevalueerd; dit is enkel een query-object.

			List<string> res1 = query.ToList();		// evalueer query, en stop resultaat (strings) in res1

			// Voeg nu een trappistenbrouwerij toe

			brouwerijen.Add(new Brouwerij
			                	{
			                		Naam = "Westmalle",
			                		Plaats = "Westmalle",
			                		Bieren = new Bier[]
			                		         	{
			                		         		new Bier {Naam = "Westmalle Dubbel", Alcohol = 6},
			                		         		new Bier {Naam = "Westmalle Tripel", Alcohol = 9},
			                		         		new Bier {Naam = "Westmalle Extra", Alcohol = 4.5}
			                		         	},
			                		Trappist = true
			                	});

			// We evalueren diezelfde query op de uitgebreide data:

			List<string> res2 = query.ToList();

			// Toon resultaten:

			Console.WriteLine("Trappistenabdijen, query 1:");
			foreach (string br in res1)
			{
				Console.Write(String.Format("{0} ", br));
			}

			Console.WriteLine("\nTrappistenabdijen, query 2:");
			foreach (string br in res2)
			{
				Console.Write(String.Format("{0} ", br));
			}

			Console.WriteLine("\n");

			// Selecteer alle *brouwerijen* met een bier met minder dan 6% alcohol

			var q2 = from brouw in brouwerijen
			         where brouw.Bieren.Any(bier => bier.Alcohol < 6)
			         select brouw;

			// Alternatieve syntax:
			//	var q2 = brouwerijen.Where(brouw => brouw.Bieren.Any(bier => bier.Alcohol < 6)).Select(brouw => brouw);

			

			Console.WriteLine("Brouwerijen met licht bier:");
			foreach(Brouwerij b in q2.ToList())
			{
				Console.WriteLine(String.Format("  {0} ({1})", b.Naam, b.Plaats));
			}

			// Selecteer alle bieren, en sorteer op alcoholpercentage

			var q3 = brouwerijen.SelectMany(brouw => brouw.Bieren).OrderBy(bier => bier.Alcohol);
			// De 'SelectMany' bestaat niet in de 'eenvoudige' syntax

			Console.WriteLine("\nAlle bieren:");

			foreach (Bier b in q3.ToList())
			{
				Console.WriteLine("  {0} ({1}%)", b.Naam, b.Alcohol);
			}

			Console.ReadLine();


		}
	}
}
