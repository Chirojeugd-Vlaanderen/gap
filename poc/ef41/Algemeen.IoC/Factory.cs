using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Algemeen.IoC
{
	public static class Factory
	{
		private static IUnityContainer _container;

        public static void ContainerInit()
        {
			var section = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
			_container = new UnityContainer();
			Debug.Assert(_container != null);	// Als _container wel null is, dan ontbreekt de unityconfiguratie in Web.config
			section.Configure(_container);            
        }

		public static T Maak<T>()
		{
			return _container.Resolve<T>();
		}

        public static object Maak(Type t)
        {
            return _container.Resolve(t);
        }
	}
}
