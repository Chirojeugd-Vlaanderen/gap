using System;
using System.Diagnostics;
using System.ServiceModel;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.TestService
{
    /// <summary>
    /// Deze service heeft in eerste instantie gewoon tot doel om de combinatie
    /// WCF, Unity en IDisposable te testen.
    /// </summary>
    public class Service1 : IService1, IDisposable
    {
        private IMyDisposable _mdp;

        public Service1()
        {
            Debug.WriteLine(string.Format("Constructing service. {0}", base.GetHashCode()));
            _mdp = Factory.Maak<IMyDisposable>();
        }

        public Service1(IMyDisposable mdp)
        {
            Debug.WriteLine(string.Format("Constructing service. {0}", base.GetHashCode()));
            _mdp = mdp;
        }

        public string Hello()
        {
            return _mdp.Hello();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Debug.WriteLine(string.Format("Disposing service. {0}", base.GetHashCode()));
            _mdp.Dispose();
        }
    }
}
