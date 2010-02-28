using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using System.IO;
using System.Runtime.Serialization;
using Chiro.Gap.Dummies;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Summary description for LedenAanmaken
	/// </summary>
	[TestClass]
	public class LedenAanmaken
	{
		public LedenAanmaken()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
		}

		[TestInitialize]
		public void setUp()
		{
			
		}

		/// <summary>
		/// Nodige tests:
		/// persoon is al lid
		/// persoon is te jong
		/// persoon is te oud
		/// er is geen afdeling
		/// is geen gav van de gelieerdepersoon
		/// is geen gav van het groepswerkjaar
		/// 
		/// kind aanmaken
		/// er is geen afdeling van de gewenste leeftijd
		/// de persoon heeft geen geboortedatum
		/// de aangepaste leeftijd moet in [6,18] zitten
		/// de echte leeftijd moet in [6,20] zitten
		/// tests: eindeinstap periode is juist gezet, afdeling is juist gezet
		/// 
		/// leiding aanmaken
		/// </summary>

		[TestMethod]
		public void TestMethod1()
		{

		}
	}
}
