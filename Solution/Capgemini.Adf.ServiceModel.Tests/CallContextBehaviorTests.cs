using System.ServiceModel;
using Capgemini.Adf.ServiceModel.Extensions;
using NUnit.Framework;

namespace Capgemini.Adf.ServiceModel.Tests
{
	[TestFixture]
	public class CallContextBehavior_Applied_On_Operation_Tests
	{
		private const string address = "net.pipe://localhost/TestService";

		private ServiceHost host;
		private ITestService service;

		[TestFixtureSetUp]
		public void SetUp()
		{
			host = new ServiceHost(typeof(TestService));
			host.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), address);
			host.Open();

			var factory = new ChannelFactory<ITestService>(new NetNamedPipeBinding(), new EndpointAddress(address));
			service = factory.CreateChannel();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			host.Close();
		}

		[Test]
		public void CallContext_is_null_by_default()
		{
			Assert.IsTrue(service.OperationWithoutContext());
		}

		[Test]
		public void CallContext_should_be_available_after_applying_service_attribute()
		{
			Assert.IsTrue(service.OperationWithContext());
		}
	}

	internal class MyCallContext : CallContext
	{
		// any property we like in our context

		public new static MyCallContext Current
		{
			get { return CallContext.Current as MyCallContext; }
		}

		protected override void Initialize() { }
		protected override void Dispose() { }
	}

	[ServiceContract]
	internal interface ITestService
	{
		[OperationContract]
		bool OperationWithContext();

		[OperationContract]
		bool OperationWithoutContext();
	}

	public class TestService : ITestService
	{
		[CallContextBehavior(typeof(MyCallContext))]
		public bool OperationWithContext()
		{
			return MyCallContext.Current != null;
		}
		
		public bool OperationWithoutContext()
		{
			return MyCallContext.Current == null;
		}
	}
}
