// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Web.Mvc;

using Chiro.Cdf.Ioc;

namespace Chiro.Gap.WebApp
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
