using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Cg2.Core.Domain;

namespace WebServices.Test
{
    [TestFixture]
    public class ServiceTest
    {
        public ServiceTest() { }

        [Test]
        public void GroepOphalen()
        {
            using (ServiceReference1.GroepenServiceClient service = new WebServices.Test.ServiceReference1.GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);

                Assert.AreEqual(g.Naam, "VALENTIJN");
            }
        }
    }
}
