// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Algemene exception voor GAP
	/// </summary>
	/// <remarks>Gebaseerd op http://blog.gurock.com/articles/creating-custom-exceptions-in-dotnet/</remarks>
	[Serializable]
	public class GapException : Exception
	{
		private int _foutNummer;
		private IEnumerable<string> _items;

		#region property's

		/// <summary>
		/// Foutnummer voor de exception.
		/// </summary>
		public int FoutNummer { get { return _foutNummer; } set { _foutNummer = value; } }

		/// <summary>
		/// Worden gebruikt voor substituties in foutberichten
		/// </summary>
		public IEnumerable<string> Items { get { return _items; } set { _items = value; } }

		#endregion

		#region standaardconstructors

		/// <summary>
		/// De standaardconstructor
		/// </summary>
		public GapException() { }

		/// <summary>
		/// Construeer GapException met bericht <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public GapException(string message) : base(message) { }

		/// <summary>
		/// Construeer GapException met bericht <paramref name="message"/> en een inner exception
		/// <paramref name="innerException"/>
		/// </summary>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		/// <param name="innerException">Andere exception die de deze veroorzaakt</param>
		public GapException(string message, Exception innerException) : base(message, innerException) { }

		#endregion

		#region serializatie

		/// <summary>
		/// Constructor voor deserializatie.
		/// </summary>
		/// <param name="info">De serializatie-info</param>
		/// <param name="context">De streamingcontext</param>
		protected GapException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			if (info != null)
			{
				_foutNummer = info.GetInt32("_foutNummer");
				_items = (IEnumerable<string>)info.GetValue("_items", typeof(IEnumerable<string>));
			}
		}

		/// <summary>
		/// Serializatie van de exception
		/// </summary>
		/// <param name="info">Serializatie-info waarin eigenschappen van exception bewaard moeten worden</param>
		/// <param name="context">De streamingcontext</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("_foutNummer", _foutNummer);
				info.AddValue("_items", _items);
			}
		}

		#endregion

		#region custom constructors

		/// <summary>
		/// Construeer GapException met bericht <paramref name="message"/> en foutnummer
		/// <paramref name="foutNummer"/>
		/// </summary>
		/// <param name="foutNummer">Foutnummer van de fout die de exception veroorzaakte</param>
		/// <param name="message">Technische info over de exception; nuttig voor developer</param>
		public GapException(int foutNummer, string message) : base(message) 
		{
			_foutNummer = foutNummer;
		}
		
		#endregion
	}
}
