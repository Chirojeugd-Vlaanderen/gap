using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exception die opgeworpen kan worden als er ergens een groepswerkjaar niet klopt.
	/// </summary>
	public class GroepsWerkJaarException: System.Exception, ISerializable
	{
		public GroepsWerkJaarException(): base() {}
		public GroepsWerkJaarException(string message): base(message) {}
		public GroepsWerkJaarException(string message, Exception inner): base(message, inner) {}
		public GroepsWerkJaarException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

}
