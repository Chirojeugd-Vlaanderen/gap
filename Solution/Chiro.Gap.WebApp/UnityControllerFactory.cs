// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// 
	/// </summary>
	public class UnityControllerFactory : DefaultControllerFactory
	{
		IUnityContainer container;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		public UnityControllerFactory(IUnityContainer container)
		{
			this.container = container;
		}

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

			return container.Resolve(controllerType) as IController;
		}
	}
}
