// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Klasse die momenteel gewoon gebruikt wordt zodat de IOC-container iets heeft
	/// om <code>IDao`1</code> op te mappen.  Ik weet nameijk niet hoe je een generic
	/// met 1 type `injecteert' naar een generic met 2 types.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AbstractChiroGroepDao<T>: Dao<T, ChiroGroepEntities>, IDao<T> where T:EntityObject, IEfBasisEntiteit
	{
	}
}
