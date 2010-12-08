using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Voorbeeld.LambdaExpressies
{
	class Program
	{
		static void Main(string[] args)
		{
			Voorbeeld1.Uitvoeren();

			// Wacht op een ENTER van de gebruiker, om
			// te vermijden dat Windows de terminal 
			// dadelijk sluit.

			Console.WriteLine("Druk <ENTER>");
			Console.ReadLine();
		}
	}
}
