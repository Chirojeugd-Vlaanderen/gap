// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een GroepsWerkJaar-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	public partial class GroepsWerkJaar : IEfBasisEntiteit
	{
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		// Ge kunt er over zeggen wat ge wilt, maar het gebeurt al heel gauw dat groepswerkjaren
		// met elkaar vergeleken worden ipv de groepswerkjaarID's.  Dus als het geen problemen geeft
		// om Equals/GetHashCode te overloaden, wil ik het toch graag doen.

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.ChiroEquals(obj);
		}
	}
}