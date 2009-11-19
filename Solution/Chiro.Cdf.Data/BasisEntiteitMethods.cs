using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Klasse met extension methods voor basisentiteiten.
	/// </summary>
	public static class BasisEntiteitMethods
	{
		const int DefaultID = 0;

		/// <summary>
		/// Sql rowversion als string, om gemakkelijk te kunnen gebruiken
		/// met MVC model binding in forms
		/// </summary>
		/// <param name="be">deze entiteit</param>
		/// <returns>stringrepresentatie van de rowversion</returns>
		public static string VersieStringGet(this IBasisEntiteit be)
		{
			return be.Versie == null ? "" : Convert.ToBase64String(be.Versie);
		}

		/// <summary>
		/// Omgekeerde bewerking van VersieStringGet
		/// </summary>
		/// <param name="be">deze entiteit</param>
		/// <param name="ver">stringrepresentatie van rowversion</param>
		public static void VersieStringSet(this IBasisEntiteit be, String ver)
		{
			be.Versie = Convert.FromBase64String(ver);
		}


		#region Entity path marker methods

		/// <summary>
		/// Marker method to indicate this section of the path expression
		/// should not be loaded but only referenced.
		/// </summary>
		public static object ReferenceOnly(this IBasisEntiteit entity)
		{
			throw new InvalidOperationException("The ReferenceOnly() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		/// <summary>
		/// Marker method to indicate the instances the method is called on
		/// within path expressions should not be updated.
		/// </summary>
		public static object WithoutUpdate(this IBasisEntiteit entity)
		{
			throw new InvalidOperationException("The WithoutUpdate() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		#endregion
	}
}
