using Chiro.Gap.Domain;
using Chiro.Gap.Validatie;
using NUnit.Framework;
using System;

namespace Chiro.Gap.Algemeen.Test
{
    
    
    /// <summary>
    ///This is a test class for ValidatorTest and is intended
    ///to contain all ValidatorTest Unit Tests
    ///</summary>
    [TestFixture]
    public class ValidatorTest
    {
        /// <summary>
        /// Domme validator die altijd zegt dat het OK is.
        /// </summary>
        private class MijnValidator: Validator<int>
        {
            public override FoutNummer? FoutNummer(int teValideren)
            {
                return null;
            }
        }

        /// <summary>
        /// Controleert of Validator.Valideer <c>true</c> is als het foutnummer <c>null</c> is.
        /// </summary>
        [Test]
        public void ValideerTest()
        {
            // Arrange

            var a = new MijnValidator();

            // Assert

            Assert.IsTrue(a.Valideer(3));
        }
    }
}
