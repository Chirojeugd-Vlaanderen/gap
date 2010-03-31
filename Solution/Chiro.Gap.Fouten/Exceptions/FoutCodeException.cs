using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exception die een foutcode mee oplevert
	/// </summary>
	/// <typeparam name="T">Type van de foutcode</typeparam>
	[Serializable]
	public class FoutCodeException<T> : System.Exception, ISerializable
	{
		private T _foutCode = default(T);

		/// <summary>
		/// Default constructor
		/// </summary>
		public FoutCodeException() : this(default(T), null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		public FoutCodeException(T foutCode) : this(foutCode, null, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		public FoutCodeException(T foutCode, string message) : this(foutCode, message, null) { }

		/// <summary>
		/// Creeert een exception met gegeven foutcode
		/// </summary>
		/// <param name="foutCode">Foutcode voor de exception</param>
		/// <param name="message">String die meer informatie over de exception geeft</param>
		/// <param name="inner">Inner exception</param>
		public FoutCodeException(T foutCode, string message, Exception inner)
			: base(message, inner)
		{
			_foutCode = foutCode;
		}

		/// <summary>
		/// Property die toegang geeft tot de foutcode
		/// </summary>
		public T FoutCode
		{
			get
			{
				return _foutCode;
			}
			set
			{
				_foutCode = value;
			}
		}
	}
}
