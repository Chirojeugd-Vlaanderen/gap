using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.TestService.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var svc = new ServiceReference1.Service1Client())
            {
                Debug.WriteLine(svc.Hello());
            }
        }
    }
}
