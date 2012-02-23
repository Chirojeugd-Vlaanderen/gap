// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Reflection;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Extension methods using reflection.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Invokes a public method by reflection.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="methodName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static object PublicInvokeMethod(this object instance, string methodName, params object[] args)
		{
			return instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance).Invoke(instance, args);
		}

		/// <summary>
		/// Gets a public property by reflection.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static object PublicGetProperty(this object instance, string propertyName)
		{
			return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
		}

		/// <summary>
		/// Sets a public property by reflection.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public static void PublicSetProperty(this object instance, string propertyName, object value)
		{
			instance.GetType().GetProperty(propertyName).SetValue(instance, value, null);
		}
	}
}
