// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exception die toelaat om meerdere foutboodschappen over de members van een object mee te sturen.
	/// </summary>
	/// <typeparam name="TFoutCode">Type van de foutcodes gebruikt in de foutboodschappen</typeparam>
	/// <remarks>TODO: Dit wordt blijkbaar enkel gebruikt voor adressen.  Is heel die constructie dan wel
	/// nodig? Misschien is een AdresException wel even goed.</remarks>
	[Serializable]
	public class OngeldigObjectException<TFoutCode> : FoutCodeException<TFoutCode>
	{
		private Dictionary<string, FoutBericht<TFoutCode>> _berichten = new Dictionary<string, FoutBericht<TFoutCode>>();

		/// <summary>
		/// Construeert de exception op basis van een dictionary <paramref name="berichten"/>
		/// </summary>
		/// <param name="berichten">Dictionary met entries (component, foutbericht)</param>
		public OngeldigObjectException(Dictionary<string, FoutBericht<TFoutCode>> berichten)
			: base()
		{
			_berichten = berichten;
		}

		/// <summary>
		/// Berichten bij de exception.  De key is de component van het adres waar de fout betrekking
		/// op heeft, de value is het foutbericht zelf.
		/// </summary>
		public Dictionary<string, FoutBericht<TFoutCode>> Berichten
		{
			get { return _berichten; }
			set { _berichten = value; }
		}		
	}
}
