/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
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
