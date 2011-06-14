// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Cdf.ServiceHelper
{
	/// <summary>
	/// Een IServiceHelper is een constructie om webservices op te roepen.  De bedoeling van deze
	/// interface is om de ServiceHelper gemakkelijk te kunnen mocken.
	/// </summary>
	public interface IServiceHelper
	{
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
		T CallService<I, T>(Func<I, T> operation) where I : class;

		/// <summary>
		/// Gebruik deze method om een method van een service aan te roepen.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <param name="operation">Lambda-expressie die de vertrekkende van <typeparamref name="I"/>
		/// de gewenste method oproept.</param>
		/// <example>
		/// <c>CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		void CallService<I>(Action<I> operation) where I : class;
	}
}
