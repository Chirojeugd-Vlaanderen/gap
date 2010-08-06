// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Data.Objects.DataClasses;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// IEfBasisEntiteit: Basisentiteit voor gebruik met Chiro.Cdf.Data.Entity
	/// (entity framework).
	/// </summary>
	public interface IEfBasisEntiteit : IBasisEntiteit, IEntityWithKey
	{
	}
}
