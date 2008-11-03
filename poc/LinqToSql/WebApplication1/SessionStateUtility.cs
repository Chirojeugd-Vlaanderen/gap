using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CgDal;

namespace WebApplication1
{
    /// <summary>
    /// Klasse die sessievariabelen beheert
    /// </summary>
    public static class SessionStateUtility
    {
        public static Persoon GetoondePersoon
        {
            get
            {
                return HttpContext.Current.Session["GetoondePersoon"] as Persoon;
            }
            set
            {
                HttpContext.Current.Session["GetoondePersoon"] = value;
            }
        }
    }
}
