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

using System;
using System.Web.Mvc;

using Chiro.Cdf.Ioc;

namespace Chiro.Gap.WebApi
{
    /// <summary>
    /// Onze eigen 'controller factory' die ervoor zorgt dat er IOC gebruikt kan worden bij het aanmaken
    /// van controllers voor MVC-toepassingen
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
	{
		protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				// throw new ArgumentNullException("controllerType");

				// Misschien werkt het wel als ik dan null teruggeef.
				return null;
			}

			if (!typeof(IController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException(string.Format("Type requested {0} is not a controller", controllerType));
			}

			return Factory.Maak(controllerType) as IController;
		}
	}
}
