using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Messaging;

using Microsoft.Practices.Unity;

namespace Chiro.Cdf.ServiceModel
{
    /// <summary>
    /// Generic ServiceHost with extra features 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceHost<T> : ServiceHost
    {
        public ServiceHost()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Constructor voor dependency injection.  Hier wordt DI gebruikt om de instantie
        /// aan te maken.  (Hoop ik.)
        /// </summary>
        /// <param name="instantie"></param>
        [InjectionConstructor]
        public ServiceHost(T instantie)
            : base(instantie)
        {
        }

        public ServiceHost(params string[] baseAdresses)
            : base(typeof(T), Convert(baseAdresses))
        {
        }

        public ServiceHost(params Uri[] baseAdresses)
            : base(typeof(T), baseAdresses)
        {
        }

        static Uri[] Convert(string[] baseAddresses)
        {
            Converter<string, Uri> convert = (address) => new Uri(address);
            return baseAddresses.ConvertAll(convert);
        }

        protected override void OnOpening()
        {
            // Onderstaande truuk, die de queues aanmaakt als ze nog niet bestaan, werkt enkel
            // als de queue in hetzelfde domein zit als de user die de queue leest.  Voor ons is
            // dit typisch niet het geval.  (het al dan niet bestaan van de queues in chiro.wereld
            // wordt niet opgemerkt door de user in chiro.lokaal.)

            // foreach (var endpoint in Description.Endpoints)
            // {
            //         endpoint.VerifyQueue();
            // }
            base.OnOpening();
        }
    }

    public static class QueuedServiceHelper
    {
        public static void VerifyQueue(this ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queue = GetQueueFromUri(endpoint.Address.Uri);
                if (MessageQueue.Exists(queue) == false)
                {
                    MessageQueue.Create(queue, true);
                }
            }
        }

        private static string GetQueueFromUri(Uri uri)
        {
            string path = uri.PathAndQuery;
            string queue = "." + path.Replace(@"/", @"\").Replace("private", "private$");
            return queue;
        }
    }
}
