using System;
using Chiro.Cdf.Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Mailer;


// Unit tests zonder veel structuur.  Achteraf op te kuisen.


namespace DommeTests.cs
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MailNaarGmail()
        {
            Factory.ContainerInit(); // TODO: bij begin van tests, niet in test

            IMailer mailer = Factory.Maak<IMailer>();

            var resultaat = mailer.Verzenden("johan.vervloet@gmail.com", "unit test " + DateTime.Now.ToString(), "unit test");

            Assert.IsTrue(resultaat);
        }

        [TestMethod]
        public void MailNaarChiro()
        {
            Factory.ContainerInit(); // TODO: bij begin van tests, niet in test

            IMailer mailer = Factory.Maak<IMailer>();

            var resultaat = mailer.Verzenden("johan.vervloet@chiro.be", "unit test " + DateTime.Now.ToString(), "unit test");

            Assert.IsTrue(resultaat);
        }

    }
}
