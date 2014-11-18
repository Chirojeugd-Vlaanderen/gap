using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.UpdateApi
{
    public class PersoonModule: Nancy.NancyModule
    {
        public PersoonModule()
        {
            Get["/"] = _ => "Hello World!";
        }
    }
}