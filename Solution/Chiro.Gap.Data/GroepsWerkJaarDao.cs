using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data.Objects;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Data.Ef
{
	public class GroepsWerkJaarDao : Dao<GroepsWerkJaar, ChiroGroepEntities>, IGroepsWerkJaarDao
	{
		// TODO: deze klasse en bijhorende interface mag eigenlijk weg.
		// Je kan net zo goed Dao<GroepsWerkJaar, ChiroGroepEntities> gebruiken.
	}
}
