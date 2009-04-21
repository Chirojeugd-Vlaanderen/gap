using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NUnit.Framework;

namespace Capgemini.Adf.ServiceModel.Tests
{
	[TestFixture]
	public class ShieldExceptionsAttributeTests
	{
		private const string serviceUri = "net.pipe://localhost/TestService";
		private readonly Binding binding = new NetNamedPipeBinding();

		private readonly ServiceHost host = new ServiceHost(typeof(TestService), new Uri(serviceUri));

		[TestFixtureSetUp]
		public void Setup()
		{
			host.AddServiceEndpoint(typeof(ITestService), binding, serviceUri);

			host.Open();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			host.Close();
		}

		[Test]
		[ExpectedException(typeof(FaultException<ValidationFault>))]
		public void Calling_operation_that_throws_shielded_validation_exception_should_throw_corresponding_fault_exception_on_client()
		{
			// Arrange
			var svc = ChannelFactory<ITestService>.CreateChannel(binding, new EndpointAddress(serviceUri));

			// Act
			svc.OperationOne();
		}

		
		[Test]
		public void Catching_fault_of_shielded_exception_should_contain_mapped_data()
		{
			// Arrange
			var svc = ChannelFactory<ITestService>.CreateChannel(binding, new EndpointAddress(serviceUri));

			// Act
			try
			{
				svc.OperationOne();
			}
			catch (FaultException<ValidationFault> ex)
			{
				Assert.IsNotNull(ex.Detail.FaultProperty);
			}
		}

		[Test]
		public void Calling_operation_with_unexpected_exception_should_throw_fault_exception_with_exception_detail()
		{
			// Arrange
			var svc = ChannelFactory<ITestService>.CreateChannel(binding, new EndpointAddress(serviceUri));

			// Act
			try
			{
				svc.OperationTwo();
			}
			catch (Exception ex)
			{
				Assert.IsNotNull(ex);
			}
		}

		[ServiceContract]
		[ShieldExceptions(new[] { typeof(ValidationException), typeof(Exception) }, new[] { typeof(ValidationFault), typeof(ExceptionDetail) })]
		private interface ITestService
		{
			[OperationContract]
			void OperationOne();
			[OperationContract]
			void OperationTwo();
		}

		private class TestService : ITestService
		{
			public void OperationOne()
			{
				throw new ValidationException() { CustomProperty = "OperationOne finds the request invalid." };
			}

			public void OperationTwo()
			{
				throw new System.NotImplementedException();
			}
		}

		private class ValidationException : Exception
		{
			public string CustomProperty { get; set; }
		}

		[DataContract]
		private class ValidationFault
		{
			// .ctor maps exception to fault
			public ValidationFault(ValidationException exception)
			{
				FaultProperty = exception.CustomProperty;
			}

			[DataMember]
			public string FaultProperty { get; set; }
		}
	}
}