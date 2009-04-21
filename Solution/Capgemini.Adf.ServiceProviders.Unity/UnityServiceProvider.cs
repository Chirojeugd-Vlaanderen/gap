using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Capgemini.Adf.ServiceProviders.Unity
{
	public class UnityServiceProvider : IServiceProvider
	{
		// TODO: get this from config or .ctor
		private readonly IUnityContainer container;

		// TODO: get config path or something
		public UnityServiceProvider()
		{
			container = new UnityContainer();
		}

		/// <summary>
		/// Initializes a new instance of the UnityServiceProvider class based on an existing container.
		/// </summary>
		/// <param name="container"></param>
		public UnityServiceProvider(IUnityContainer container)
		{
			this.container = container;
		}

		/// <summary>
		/// Gets the configured service implementation for the specified interface.
		/// </summary>
		/// <typeparam name="I">The interafce, type to get the service for</typeparam>
		/// <returns></returns>
		public I GetService<I>() where I : class
		{
			return CatchResolutionException(c => c.Resolve<I>());
		}

		public I GetService<I>(object arguments) where I : class { throw new System.NotImplementedException(); }

		/// <summary>
		/// Gets the configured service implementatio instance with the specified name.
		/// </summary>
		/// <typeparam name="I">The interface or type to get the service for.</typeparam>
		/// <param name="instanceName">The name of the service instance.</param>
		/// <returns></returns>
		public I GetService<I>(string instanceName) where I : class
		{
			return CatchResolutionException(c => c.Resolve<I>(instanceName));
		}

		public I GetService<I>(string instanceName, object arguments) where I : class { throw new System.NotImplementedException(); }

		public IEnumerable<I> GetServices<I>() where I : class
		{
			return CatchResolutionException(c => c.ResolveAll<I>());
		}

		public object GetService(Type serviceType)
		{
			return CatchResolutionException(c => c.Resolve(serviceType));
		}

		public object GetService(Type type, string instanceName)
		{
			return CatchResolutionException(c => c.Resolve(type, instanceName));
		}

		public IEnumerable GetServices(Type serviceType)
		{
			return CatchResolutionException(c => c.ResolveAll(serviceType));
		}

		/// <summary>
		/// Returns a list of instances of all registered types of type I.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		public IEnumerable<I> GetAllServices<I>()
		{
			return CatchResolutionException(c => c.ResolveAll<I>());
		}

		private TResult CatchResolutionException<TResult>(Func<IUnityContainer,TResult> method) where TResult : class 
		{
			try
			{
				return method.Invoke(container);
			}
			catch (ResolutionFailedException)
			{
				return null;
			}
		}

		public bool TryGetService<I>(out I service) where I : class
		{
			service = GetService<I>();
			return service != null;
		}

		public bool TryGetService(Type type, out object service)
		{
			service = GetService(type);
			return service != null;
		}
	}
}
