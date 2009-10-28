using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Chiro.Gap.WebApp
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        IUnityContainer container;

        public UnityControllerFactory(IUnityContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(Type controllerType)
        {
            if (controllerType == null)
                throw new ArgumentNullException("controllerType");

            if (!typeof(IController).IsAssignableFrom(controllerType))
                throw new ArgumentException(string.Format("Type requested {0} is not a controller", controllerType));

            return container.Resolve(controllerType) as IController;
        }

    }
}
