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
	public class LedenVerwijderen
	{
		public LedenVerwijderen()
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
		/// kind mag enkel non-actief gezet worden, nooit verwijderd
		/// leiding kan gewoon worden verwijderd
		/// 
		/// leiding aanmaken
		/// </summary>

		[TestMethod]
		public void TestMethod1()
		{

		}
	}
}
