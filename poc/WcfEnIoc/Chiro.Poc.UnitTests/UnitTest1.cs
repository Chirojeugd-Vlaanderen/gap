using Chiro.Poc.Ioc;
using Chiro.Poc.ServiceGedoe;
using Chiro.Poc.WcfService.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Poc.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void InitTests()
        {
            Factory.ContainerInit();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange

            var serviceMock = new Mock<IService1>();
            serviceMock.Setup(m => m.Hallo()).Returns("Antwoord van mock");

            Factory.InstantieRegistreren(serviceMock.Object);

            // Act

            string result = ServiceHelper.CallService<IService1, string>(svc => svc.Hallo());

            // Assert

            Assert.AreEqual(result, "Antwoord van mock");
        }
    }
}
