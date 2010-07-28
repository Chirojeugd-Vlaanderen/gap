using System;

using Chiro.Cdf.ServiceHelper;

namespace Chiro.Adf.ServiceModel
{
	/// <summary>
	/// ServiceHelper is nu een niet-statische wrapper naar de oorspronkelijke ServiceHelper, die nu
	/// StaticServiceHelper heet.
	/// </summary>
	public class ServiceHelper: IServiceHelper
	{
		#region IServiceHelper Members

		/// <summary>
		/// Gebruik deze method om een functie aan te roepen van een service.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <typeparam name="T">Type van het resultaat van <paramref name="operation"/></typeparam>
		/// <param name="operation">Lambda-expressie die de <typeparamref name="I"/> afbeeldt op het gewenste
		/// resultaat.</param>
		/// <returns>Resultaat van de functie-aanroep</returns>
		/// <example>
		/// <c>string = CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		public T CallService<I, T>(Func<I, T> operation) where I : class
		{
			return StaticServiceHelper.CallService<I, T>(operation);
		}

		/// <summary>
		/// Gebruik deze method om een method van een service aan te roepen.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <param name="operation">Lambda-expressie die de vertrekkende van <typeparamref name="I"/>
		/// de gewenste method oproept.</param>
		/// <example>
		/// <c>CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		public void CallService<I>(Action<I> operation) where I : class
		{
			StaticServiceHelper.CallService<I>(operation);
		}

		#endregion
	}
}
