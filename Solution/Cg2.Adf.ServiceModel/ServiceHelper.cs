using System;
using System.ServiceModel;

namespace Cg2.Adf.ServiceModel
{
	/// <summary>
	/// Provides helper/extension methods to call operations on service proxies and making sure the proxy is 
	/// disposed or aborted in case of timeout or communication exceptions.
	/// </summary>
	public static class ServiceHelper
	{
		/// <summary>
		/// Calls the specified operation on a service instance and safely disposes or aborts the service 
		/// (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The service type</typeparam>
		/// <param name="service">The service instance to perform the opreation on.</param>
		/// <param name="operation">The operation to invoke.</param>
		public static void Call<I>(this I service, Action<I> operation) where I : class
		{
			if (service == null) throw new ArgumentNullException("service");
			if (operation == null) throw new ArgumentNullException("operation");
			if (!(service is IClientChannel)) throw new ArgumentException("The Call extension method can only be called in conjuction with IClientChannel proxies.");

			try
			{
				operation.Invoke(service);
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
		/// Calls the specified operation on a service instance and safely disposes or aborts the service (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The type of service.</typeparam>
		/// <typeparam name="T">The return value type</typeparam>
		/// <param name="service">The serivice instance to call the operation on.</param>
		/// <param name="operation">The service operation to invoke.</param>
		/// <returns>the return value of the service operation.</returns>
		public static T Call<I, T>(this I service, Func<I, T> operation) where I : class
		{
			if (service == null) throw new ArgumentNullException("service");
			if (operation == null) throw new ArgumentNullException("operation");
			if (!(service is IClientChannel)) throw new ArgumentException("The Call extension method can only be called in conjuction with IClientChannel proxies.");

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
        /// TODO: Documenteren
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="operation"></param>
		public static void CallService<I>(Action<I> operation) where I : class
		{
			Call(ServiceProvider.Default.GetService<I>(), operation);
		}

        /// <summary>
        /// TODO: Documenteren
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
		public static T CallService<I, T>(Func<I, T> operation) where I : class
		{
			return Call(ServiceProvider.Default.GetService<I>(), operation);
		}

		private static void DisposeServiceInstance(object instance)
		{
			var client = instance as IClientChannel;
			if (client == null || client.State == CommunicationState.Closed) return;
			
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