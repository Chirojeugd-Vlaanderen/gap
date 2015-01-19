using Chiro.Cdf.ServiceHelper;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Base synchronization class.
    /// </summary>
    public class BaseSync
    {
        private readonly ServiceHelper _serviceHelper;
        protected ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public BaseSync(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }
    }
}
