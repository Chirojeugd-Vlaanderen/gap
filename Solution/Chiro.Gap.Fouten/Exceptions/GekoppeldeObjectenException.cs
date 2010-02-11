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
	public class GekoppeldeObjectenException<T>: System.Exception, ISerializable where T:IBasisEntiteit
	{
		/// <summary>
		/// Lijst met gekoppelde objecten die de problemen veroorzaken
		/// </summary>
		private IEnumerable<T> _gekoppeldeObjecten;

		public GekoppeldeObjectenException() : base() { }
		public GekoppeldeObjectenException(string message) : base(message) { }
		public GekoppeldeObjectenException(string message, Exception inner) : base(message, inner) { }
		public GekoppeldeObjectenException(
			SerializationInfo info, 
			StreamingContext context) : base(info, context) { }

		public GekoppeldeObjectenException(string message, IEnumerable<T> objecten): base(message)
		{
			_gekoppeldeObjecten = objecten;
		}

		/// <summary>
		/// Lijst met gekoppelde objecten die de problemen veroorzaken
		/// </summary>
		public IEnumerable<T> Objecten { get { return _gekoppeldeObjecten; } }
	}
}
