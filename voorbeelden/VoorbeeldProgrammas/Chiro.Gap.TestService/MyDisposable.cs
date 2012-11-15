using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Chiro.Gap.TestService
{
    public interface IMyDisposable : IDisposable
    {
        string Hello();
    }

    public class MyDisposable: IMyDisposable
    {
        public MyDisposable()
        {
            Debug.WriteLine(String.Format("Creating MyDisposable. {0}", base.GetHashCode()));           
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Debug.WriteLine(String.Format("Disposing MyDisposable. {0}", base.GetHashCode()));
        }

        public string Hello()
        {
            return String.Format("Hello world! {0}", base.GetHashCode());
        }
    }
}