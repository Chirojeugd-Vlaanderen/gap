using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel.Extensions
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
    public class CallContextInitializer : ICallContextInitializer
    {
        private readonly Type _contextType;

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="contextType"></param>
        public CallContextInitializer(Type contextType)
        {
            _contextType = contextType;
        }

        object ICallContextInitializer.BeforeInvoke(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.IClientChannel channel, Message message)
        {
            CallContext.Current = Activator.CreateInstance(_contextType) as CallContext;

            if (CallContext.Current == null)
                throw new ConfigurationErrorsException(string.Format("The type '{0}' could not be initialized.", _contextType));

            CallContext.Current.Initialize();

            return null; // we don't need no correlation
        }

        void ICallContextInitializer.AfterInvoke(object state)
        {
            // Dispose the context if it as created
            if (CallContext.Current != null)
                CallContext.Current.Dispose();
        }
    }
}