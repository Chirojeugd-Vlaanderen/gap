﻿using System;
using System.Linq.Expressions;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;

using Moq;

namespace Chiro.Gap.Workers.Test.CustomIoc
{
    
    
    /// <summary>
    ///This is a test class for GroepsWerkJaarManagerTest and is intended
    ///to contain all GroepsWerkJaarManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class GroepsWerkJaarManagerTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}


		///<summary>
		///Controleert of groepswerkjaarcache gecleard wordt bij bewaren groepswerkjaar
		///</summary>
		[TestMethod()]
		public void BewarenTest()
		{
			// Arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarResetten(testData.DummyGroep.ID)).Verifiable();
			Factory.InstantieRegistreren(veelGebruiktMock.Object);

			var groepsWerkJaarDaoMock = new Mock<IGroepsWerkJaarDao>();
			groepsWerkJaarDaoMock.Setup(
				dao => dao.Bewaren(testData.HuidigGwj, It.IsAny<Expression<Func<GroepsWerkJaar, object>>[]>())).Returns(
					testData.HuidigGwj);
			Factory.InstantieRegistreren(groepsWerkJaarDaoMock.Object);

			var target = Factory.Maak<GroepsWerkJaarManager>();

			// Act

			target.Bewaren(testData.HuidigGwj, GroepsWerkJaarExtras.Geen);

			// Assert
			veelGebruiktMock.Verify(vgb => vgb.GroepsWerkJaarResetten(testData.DummyGroep.ID), Times.Once());
		}
	}
}