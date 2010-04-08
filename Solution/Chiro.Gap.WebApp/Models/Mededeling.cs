using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Deze enum kan gebruikt worden om de 'ernst' van een mededeling mee aan te duiden.
	/// </summary>
	public enum MededelingsType
	{
		Onbekend = 0,
		Informatie = 1,
		Waarschuwing = 2,
		Probleem = 3
	};

	/// <summary>
	/// Een mededeling voor in de MasterView.
	/// </summary>
	public struct Mededeling
	{
		public MededelingsType Type { get; set; }
		public string Info { get; set; }
	}
}
