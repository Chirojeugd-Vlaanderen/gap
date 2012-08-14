using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.SyncInterfaces
{
    public interface IGroepenSync
    {
        /// <summary>
        /// Updatet de gegevens van groep <paramref name="g"/> in Kipadmin. Het stamnummer van <paramref name="g"/>
        /// bepaalt de groep waarover het gaat.
        /// </summary>
        /// <param name="g">Te updaten groep in Kipadmin</param>
        void Bewaren(Groep g);
    }
}
