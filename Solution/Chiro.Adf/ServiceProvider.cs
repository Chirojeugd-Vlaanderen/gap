using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

using Chiro.Adf.Configuration;

namespace Chiro.Adf
{
	/// <summary>
	/// Resolves dependencies through configured or registered service providers.
	/// </summary>
	public class ServiceProvider : IServiceProvider
	{
		#region static members

		private static readonly ServiceProvider DefaultInstance = new ServiceProvider();

		static ServiceProvider()
		{
			var configSection = ConfigurationManager.GetSection(AdfConfigurationSection.DefaultAdfConfigurationSectionName) as AdfConfigurationSection;

			if (configSection != null)
				DefaultInstance.ApplyConfiguration(configSection);
		}


		/// <summary>
		/// Gets the default ServiceProvider instance.
		/// </summary>
		public static ServiceProvider Default
		{
			get { return DefaultInstance; }
		}

		#endregion

		private readonly IList<IServiceProvider> _serviceProviders = new List<IServiceProvider>();
        
		/// <summary>
		/// Initializes a new instance of the ServiceProvider class.
		/// </summary>
		public ServiceProvider() { }

		/// <summary>
		/// Initializes a new instance of the ServiceProvider class 
		/// </summary>
		/// <param name="configurationSection"></param>
		public ServiceProvider(AdfConfigurationSection configurationSection)
		{
			if(configurationSection != null)
				ApplyConfiguration(configurationSection);
		}

		/// <summary>
		/// Gets the implementation for the specified service interface I.
		/// </summary>
		/// <typeparam name="I">The interface type to resolve.</typeparam>
		/// <returns>The configured implementation of interface I or null.</returns>
		public I GetService<I>() where I : class
		{
			return CallProviders(p => p.GetService<I>(), string.Format("Service type '{0}' could not be resolved.", typeof(I)));
		}

		/// <summary>
		/// Gets the implementation for the specified service interface I.
		/// </summary>
		/// <typeparam name="I">The interface type to resolve.</typeparam>
		/// <param name="instanceName">The name of the configured implementation.</param>
		/// <returns>The configured implementation of interface I or null.</returns>
		public I GetService<I>(string instanceName) where I : class
		{
			return CallProviders(p => p.GetService<I>(instanceName), string.Format("Service type '{0}' could not be resolved.", typeof(I)));
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="arguments"></param>
        /// <returns></returns>
		public I GetService<I>(object arguments) where I : class
		{
			return CallProviders(p => p.GetService<I>(arguments), string.Format("Service type '{0}' could not be resolved.", typeof(I)));
		}

		/// <summary>
		/// Gets the service implementation for the specified interface and .ctor arguments with the specified name.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <param name="instanceName"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public I GetService<I>(string instanceName, object arguments) where I : class
		{
			return CallProviders(p => p.GetService<I>(instanceName, arguments), string.Format("Service type '{0}' with name '{1}' could not be resolved.", typeof(I), instanceName));
		}

		/// <summary>
		/// Gets the implementation for the specified service interface type.
		/// </summary>        
		/// <returns>The configured implementation of interface I or null.</returns>
		public object GetService(Type serviceType)
		{
            return CallProviders(p => p.GetService(serviceType), string.Format("Service type '{0}' could not be resolved.", serviceType));
		}

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
		public object GetService(Type type, string instanceName)
		{
			return CallProviders(p => p.GetService(type, instanceName), string.Format("Service type '{0}' with name '{1}' could not be resolved.", type, instanceName));
		}

		/// <summary>
		/// Gets all implementation for the specified service interface type.
		/// </summary>        
		public IEnumerable<I> GetServices<I>() where I : class
		{
			return CallProviders(p => p.GetServices<I>(), string.Format("Service type '{0}' could not be resolved.", typeof(I)));
		}

		/// <summary>
		/// Gets all implementation for the specified service interface type.
		/// </summary>        
		public IEnumerable GetServices(Type type)
		{
			return CallProviders(p => p.GetServices(type), string.Format("Service type '{0}' could not be resolved.", type));
		}

		/// <summary>
        /// Tries to get the implementation for the specified service interface I.
        /// </summary>
        /// <typeparam name="I">The interface type to resolve.</typeparam>
        /// <returns>True if the service was found, otherwise false</returns>
        public bool TryGetService<I>(out I service) where I : class
        {
			service = CallProviders(p => p.GetService<I>(), null);
			return service != null;
        }

        /// <summary>
        /// Tries to get the implementation for the specified service interface type.
        /// </summary>        
        /// <returns>Returns true if the service was found, otherwise false.</returns>
        public bool TryGetService(Type type, out object service)
		{
			service = CallProviders(p => p.GetService(type), null);
			return service != null;
        }

        /// <summary>
        /// Tries to get the implementation for the specified service interface I.
        /// </summary>
        /// <typeparam name="I">The interface type to resolve.</typeparam>
        /// <param name="instanceName">The name of the configured implementation.</param>
        /// <param name="service">The configured implementation of interface I or null.</param>
        /// <returns>True if the service was found, otherwise false</returns>
        public bool TryGetService<I>(string instanceName, out I service) where I : class
        {
        	service = CallProviders(p => p.GetService<I>(instanceName), null);
        	return service != null;
        }

		/// <summary>
		/// Adds an IServiceProvider to the list of service providers to probe for services.
		/// </summary>
		/// <param name="provider">The IServiceProvider implementation tto register.</param>
		public void RegisterProvider(IServiceProvider provider)
		{
			if (provider == null) throw new ArgumentNullException("provider");
            if(_serviceProviders.Contains(provider)) throw new ArgumentException("The specified provider is already registered.", "provider");

			_serviceProviders.Add(provider);
		}

        /// <summary>
        /// Removes an IServiceProvider from the list of service providers.
        /// </summary>
        public void UnregisterProvider(IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (!_serviceProviders.Contains(provider)) throw new ArgumentException("The specified provider is not registered.", "provider");

            _serviceProviders.Remove(provider);
        }

		/// <summary>
		/// Configures the ServiceProviders with the specified configuration section
		/// </summary>
		/// <param name="configurationSection"></param>
		public void ApplyConfiguration(AdfConfigurationSection configurationSection)
		{
			if (configurationSection == null) throw new ArgumentNullException("configurationSection");

			if(_serviceProviders.Count > 0) throw new InvalidOperationException("Configuration cannot be applied twice.");

			foreach (ServiceProviderConfigurationElement providerSettings in configurationSection.ServiceProviders)
				_serviceProviders.Add(CreateProvider(providerSettings));
		}

        /// <summary>
        /// Clear all service providers
        /// </summary>
        public void ClearProviders()
        {
            _serviceProviders.Clear();
        }
        
		private I CallProviders<I>(Func<IServiceProvider, I> func, string exceptionMessage) where I : class
		{
			try
			{
				foreach (var provider in _serviceProviders)
				{
					try
					{
						var service = func.Invoke(provider);
						if (service != null) return service;
					}
					catch (NotSupportedException) { }
				}
			}
			catch (Exception ex) // this should never happen
			{
				throw new ServiceActivationException(exceptionMessage ?? ex.Message, ex);
			}

			if(string.IsNullOrEmpty(exceptionMessage)) return null;

			throw new ServiceActivationException(exceptionMessage);
		}
        
		private static IServiceProvider CreateProvider(ServiceProviderConfigurationElement providerSettings)
		{
			var type = Type.GetType(providerSettings.Type);

			if (providerSettings.ConstructorParameters.Count > 0)
				return Activator.CreateInstance(type, GetConstructorParameters(type, providerSettings.ConstructorParameters)) as IServiceProvider;

			return Activator.CreateInstance(type) as IServiceProvider;
		}

		private static object[] GetConstructorParameters(Type type, IDictionary<string, string> parameters)
		{
			if (parameters.Count == 0) return null;

			foreach (var constructor in type.GetConstructors())
			{
				var parameterInfos = constructor.GetParameters();
                if (parameterInfos.Length != parameters.Count) continue; // if parameter count doesn't match, go straight to the next .ctor

				// array to return and flag to indicate we got a matching .ctor
				var rets = new object[parameters.Count];
                var match = true;

				for (var i = 0; i < parameters.Count; i++)
				{
					if (!parameters.ContainsKey(parameterInfos[i].Name))
					{
						match = false;
						break;
					}

					rets[i] = Convert.ChangeType(parameters[parameterInfos[i].Name], parameterInfos[i].ParameterType, CultureInfo.InvariantCulture);
				}

				if (match) return rets;
			}

			return null;
		}
	}
}
