// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Klasse met extension methods voor basisentiteiten.
	/// </summary>
	public static class BasisEntiteitMethods
	{
		/// <summary>
		/// Sql rowversion als string, om gemakkelijk te kunnen gebruiken
		/// met MVC model binding in forms
		/// </summary>
		/// <param name="be">Deze entiteit</param>
		/// <returns>Stringrepresentatie van de rowversion</returns>
		public static string VersieStringGet(this IBasisEntiteit be)
		{
			return be.Versie == null ? String.Empty : Convert.ToBase64String(be.Versie);
		}

		/// <summary>
		/// Omgekeerde bewerking van VersieStringGet
		/// </summary>
		/// <param name="be">Deze entiteit</param>
		/// <param name="ver">Stringrepresentatie van rowversion</param>
		public static void VersieStringSet(this IBasisEntiteit be, String ver)
		{
			if (ver == null)
			{
				// strings die null zijn in webforms, dat komt nooit goed.
				// (dan krijg je problemen zoals ticket #198)
				// Vandaar dat een nulle version string wordt behandeld
				// als de lege string.

				ver = String.Empty;
			}

			be.Versie = Convert.FromBase64String(ver);
		}

		/// <summary>
		/// Experimentele equals die objecten als gelijk beschouwt als hun ID's niet nul en gelijk zijn.
		/// </summary>
		/// <typeparam name="T">Type van <paramref name="be"/>, moet IBasisentiteit implementeren</typeparam>
		/// <param name="be">te vergelijken entiteit</param>
		/// <param name="obj">te vergelijken entiteit</param>
		/// <returns>true indien <paramref name="be"/> en <paramref name="obj"/> hetzelfde niet-nulle ID 
		/// hebben</returns>
		/// <remarks>als zowel <paramref name="be"/> als <paramref name="obj"/> ID 0 hebben, wordt
		/// Object.Equas aangeroepen</remarks>
		public static bool ChiroEquals<T>(this T be, object obj) where T : class, IBasisEntiteit
		{
			if (obj is T)
			{
				var f = obj as T;

				if (f.ID != 0 && f.ID == be.ID)
				{
					return true;
				}
				else if (f.ID == 0 && be.ID == 0)
				{
					return ReferenceEquals(be, obj);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		#region Entity path marker methods

		/// <summary>
		/// Marker method to indicate this section of the path expression
		/// should not be loaded but only referenced.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static object ReferenceOnly(this IBasisEntiteit entity)
		{
			throw new InvalidOperationException("The ReferenceOnly() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		/// <summary>
		/// Marker method to indicate the instances the method is called on
		/// within path expressions should not be updated.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static object WithoutUpdate(this IBasisEntiteit entity)
		{
			throw new InvalidOperationException("The WithoutUpdate() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		#endregion
	}
}
