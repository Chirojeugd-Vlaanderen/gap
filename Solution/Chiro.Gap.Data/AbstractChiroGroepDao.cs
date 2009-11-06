using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using System.Data.Objects.DataClasses;

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
