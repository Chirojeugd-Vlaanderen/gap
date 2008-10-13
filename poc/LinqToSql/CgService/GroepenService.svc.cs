using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace CgService
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        private GroepsDataAccess da = new GroepsDataAccess();

        public ChiroGroep ChiroGroepGet(int groepID)
        {
            var result = da.ChiroGroepGet(groepID);

            System.Diagnostics.Debug.WriteLine("--> " + result.Groep.Naam);
            return result;
        }

        public Groep ChiroGroepGroepGet(int groepID)
        {
            return da.ChiroGroepGroepGet(groepID);
        }
    }
}
