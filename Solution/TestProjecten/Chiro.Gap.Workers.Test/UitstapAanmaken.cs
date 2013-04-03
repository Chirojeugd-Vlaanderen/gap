using System;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
	[TestClass]
	public class UitstappenAanmaken
	{
		#region Additional test attributes
		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			
		}
		#endregion

		/// <summary>
		/// TODO zou een non-dummy test moeten worden, zodat complexer gedrag getest kan worden
		/// </summary>
		[TestMethod]
		public void UitstappenInHuidigWerkjaar()
		{
            //var testData = new DummyData();

            //var um = Factory.Maak<UitstappenManager>();

            //var uitstap = new Uitstap();
            //uitstap.IsBivak = true;
            //uitstap.Naam = "Testbivak";
            //uitstap.DatumVan = DateTime.Today.AddDays(1);
            //uitstap.DatumTot = DateTime.Today.AddDays(1);

            //var dummygwj = new GroepsWerkJaar();
            //dummygwj.WerkJaar = DateTime.Today.Year;
            //uitstap.GroepsWerkJaar = dummygwj;

            //um.Bewaren(uitstap, UitstapExtras.Geen, false);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}
	}
}
