using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// IEfBasisEntiteit: Basisentiteit voor gebruik met Chiro.Cdf.Data.Entity
	/// (entity framework).
	/// </summary>
	public interface IEfBasisEntiteit: IBasisEntiteit, IEntityWithKey
	{
	}
}
