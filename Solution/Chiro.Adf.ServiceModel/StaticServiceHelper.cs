using System;
using System.ServiceModel;

namespace Chiro.Adf.ServiceModel
{
	/// <summary>
	/// Provides helper/extension methods to call operations on service proxies and making sure the proxy is 
	/// disposed or aborted in case of timeout or communication exceptions.
	/// </summary>
	public static class StaticServiceHelper
	{
		/// <summary>
		/// Calls the specified operation on a service instance and safely disposes or aborts the service 
		/// (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The service type</typeparam>
		/// <param name="service">The service instance to perform the opreation on.</param>
		/// <param name="operation">The operation to invoke.</param>
		// ReSharper disable InconsistentNaming
		public static void Call<I>(this I service, Action<I> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (operation == null)
				throw new ArgumentNullException("operation");
			if (!(service is IClientChannel))
				throw new ArgumentException("The Call extension method can only be called in conjuction with IClientChannel proxies.");

			try
			{
				operation.Invoke(service);
			}
			catch (TimeoutException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			catch (FaultException)
			{
				((IClientChannel) service).Close();
				throw;
			}
			catch (CommunicationException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			finally
			{
				DisposeServiceInstance(service);
			}
		}

		/// <summary>
		/// Calls the specified operation on a service instance and safely disposes or aborts the service (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The type of service.</typeparam>
		/// <typeparam name="T">The return value type</typeparam>
		/// <param name="service">The serivice instance to call the operation on.</param>
		/// <param name="operation">The service operation to invoke.</param>
		/// <returns>the return value of the service operation.</returns>
		// ReSharper disable InconsistentNaming
		public static T Call<I, T>(this I service, Func<I, T> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (operation == null)
				throw new ArgumentNullException("operation");
			if (!(service is IClientChannel))
				throw new ArgumentException("The Call extension method can only be called in conjuction with IClientChannel proxies.");

			try
			{
				return operation.Invoke(service);
			}
			catch (TimeoutException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			catch (CommunicationException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			finally
			{
				DisposeServiceInstance(service);
			}
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
		// ReSharper disable InconsistentNaming
		public static void CallService<I>(Action<I> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			Call(ServiceProvider.Default.GetService<I>(), operation);
		}

		/// <summary>
		/// Gebruik deze method om een functie aan te roepen van een service.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <typeparam name="T">Type van het resultaat van <paramref name="operation"/></typeparam>
		/// <param name="operation">Lambda-expressie die de <typeparamref name="I"/> afbeeldt op het gewenste
		/// resultaat.</param>
		/// <returns>Resultaat van de functie-aanroep</returns>
		/// <example>
		/// <c>string = 
		///	StaticAdfServiceHelper.CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		// ReSharper disable InconsistentNaming
		public static T CallService<I, T>(Func<I, T> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			return Call(ServiceProvider.Default.GetService<I>(), operation);
		}

		private static void DisposeServiceInstance(object instance)
		{
			var client = instance as IClientChannel;
			if (client == null || client.State == CommunicationState.Closed)
				return;

			try
			{
				client.Close();
			}
			catch
			{
				client.Abort();
			}
		}
	}
}