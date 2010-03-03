// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using Chiro.Cdf.Data;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Dit FaultContract zal over de lijn gaan als een operatie niet uitgevoerd kan worden
	/// omdat er objecten aan elkaar gekoppeld zijn, waar dat niet verwacht is.
	/// (Bijv. het verwijderen van een niet-lege categorie.)
	/// </summary>
	/// <typeparam name="T">Klasse van de gekoppelde objecten</typeparam>
	[DataContract]
	public class GekoppeldeObjectenFault<T>
	{
		[DataMember]
		public IEnumerable<T> Objecten { get; set; }
	}
}
