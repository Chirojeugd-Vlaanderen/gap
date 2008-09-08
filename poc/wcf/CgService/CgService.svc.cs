using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgBll;
using CgDal;

namespace CgService
{
    // NOTE: If you change the class name "CgService" here, you must also update the reference to "CgService" in Web.config.
    public class CgService : ICgService
    {
        public Persoon PersoonGet(int persoonID)
        {
            return new PersoonBll().PersoonGet(persoonID);
        }
        public String Hello()
        {
            return "Hallo, service!";
        }
    }
}
