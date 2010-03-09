// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Deze exception zal opgeworpen worden als een operatie niet uitgevoerd kan worden
	/// omdat er objecten aan elkaar gekoppeld zijn, waar dat niet verwacht is.
	/// (Bijv. het verwijderen van een niet-lege categorie.)
	/// </summary>
	/// <typeparam name="T">Klasse van de gekoppelde objecten</typeparam>
	public class GekoppeldeObjectenException<T> : System.Exception, ISerializable where T : IBasisEntiteit
	{
		/// <summary>
		/// Lijst met gekoppelde objecten die de problemen veroorzaken
		/// </summary>
		private IEnumerable<T> _gekoppeldeObjecten;

		/// <summary>
		/// Instantieert een lege GekoppeldeObjectenException
		/// </summary>
		public GekoppeldeObjectenException()
			: base()
		{
		}

		/// <summary>
		/// Instantieert een GekoppeldeObjectenException met een opgegeven foutboodschap
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		public GekoppeldeObjectenException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Instantieert een GekoppeldeObjectenException met een opgegeven foutboodschap en 'inner exception'
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		/// <param name="inner"></param>
		public GekoppeldeObjectenException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Instantieert een GekoppeldeObjectenException met een opgegeven SerializationInfo en StreamingContext
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public GekoppeldeObjectenException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Instantieert een GekoppeldeObjectenException met een opgegeven foutboodschap en gekoppelde objecten
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		/// <param name="objecten">De gekoppelde objecten die de problemen veroorzaken</param>
		public GekoppeldeObjectenException(string message, IEnumerable<T> objecten)
			: base(message)
		{
			_gekoppeldeObjecten = objecten;
		}

		/// <summary>
		/// Lijst met gekoppelde objecten die de problemen veroorzaken
		/// </summary>
		public IEnumerable<T> Objecten
		{
			get
			{
				return _gekoppeldeObjecten;
			}
		}
	}
}
