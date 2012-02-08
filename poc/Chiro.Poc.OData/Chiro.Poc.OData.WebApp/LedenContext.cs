using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Poc.OData.WebApp
{
    public class LedenContext
    {
        public IQueryable<LidDetail> Leden { get { return Business.AlleLedenOphalen().AsQueryable(); } }
    }
}