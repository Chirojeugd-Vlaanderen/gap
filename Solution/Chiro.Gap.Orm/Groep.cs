// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een Groep-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	[AssociationEndBehavior("Afdeling", Owned = true)]
	[AssociationEndBehavior("Categorie", Owned = true)]
	public partial class Groep : IEfBasisEntiteit
	{
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get
			{
				return _teVerwijderen;
			}
			set
			{
				_teVerwijderen = value;
			}
		}

		public string VersieString
		{
			get
			{
				return this.VersieStringGet();
			}
			set
			{
				this.VersieStringSet(value);
			}
		}

		#region Identity en equality

		public override int GetHashCode()
		{
			return 7;
		}

		public override bool Equals(object obj)
		{
			var andere = obj as Groep;
			// Als obj geen Groep is, wordt andere null.

			if (andere == null)
			{
				return false;
			}
			else if (ID == 0 || andere.ID == 0)
			{
				return base.Equals(andere);
			}
			else
			{
				return ID.Equals(andere.ID);
			}
		}

		#endregion
	}
}
