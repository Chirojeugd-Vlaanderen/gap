using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class CallContextInitializer : ICallContextInitializer
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Type _contextType;


        /// <summary>
        /// Initializes a new instance of the <see cref="CallContextInitializer"/> class.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <remarks></remarks>
        public CallContextInitializer(Type contextType)
        {
            _contextType = contextType;
        }

        /// <summary>
        /// Implement to participate in the initialization of the operation thread.
        /// </summary>
        /// <param name="instanceContext">The service instance for the operation.</param>
        /// <param name="channel">The client channel.</param>
        /// <param name="message">The incoming message.</param>
        /// <returns>A correlation object passed back as the parameter of the <see cref="M:System.ServiceModel.Dispatcher.ICallContextInitializer.AfterInvoke(System.Object)"/> method.</returns>
        /// <remarks></remarks>
        object ICallContextInitializer.BeforeInvoke(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.IClientChannel channel, Message message)
        {
            CallContext.Current = Activator.CreateInstance(_contextType) as CallContext;

            if (CallContext.Current == null)
                throw new ConfigurationErrorsException(string.Format("The type '{0}' could not be initialized.", _contextType));

            CallContext.Current.Initialize();

            return null; // we don't need no correlation
        }

        /// <summary>
        /// Afters the invoke.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <remarks></remarks>
        void ICallContextInitializer.AfterInvoke(object state)
        {
            // Dispose the context if it as created
            if (CallContext.Current != null)
                CallContext.Current.Dispose();
        }
    }
}