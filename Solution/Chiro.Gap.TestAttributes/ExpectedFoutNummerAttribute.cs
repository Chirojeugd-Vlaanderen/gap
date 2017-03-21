using System;
using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.FaultContracts;
using NUnit.Framework;

namespace Chiro.Gap.TestAttributes
{
    /// <summary>
    /// Attribuut om in unittests het foutnummer te controleren van een 
    /// FoutNummerException of een FaultException van het type
    /// FoutNummerFault.
    /// </summary>
    /// <remarks>
    /// Deze code is een aangepaste versie van wat ik vond op
    /// http://www.cookcomputing.com/blog/archives/unit-testing-with-expectedexception
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ExpectedFoutNummerAttribute : ExpectedBaseExceptionAttribute
    {
        public Type ExpectedException { get; set; }
        public FoutNummer ExpectedFoutNummer { get; set; }

        public ExpectedFoutNummerAttribute(Type expectedException)
            : this(expectedException, 0, "")
        {
        }

        public ExpectedFoutNummerAttribute(Type expectedException,
                                              FoutNummer errorCode)
            : this(expectedException, errorCode, "")
        {
        }

        public ExpectedFoutNummerAttribute(Type expectedException,
                                              FoutNummer errorCode, string noExceptionMessage)
            : base(noExceptionMessage)
        {
            if (expectedException == null)
                throw new ArgumentNullException("exceptionType");
            if (!typeof (Exception).IsAssignableFrom(expectedException))
                throw new ArgumentException("Expected exception type must be "
                                            + "System.Exception or derived from System.Exception.",
                                            "expectedException");
            ExpectedException = expectedException;
            ExpectedFoutNummer = errorCode;
        }

        protected override void Verify(Exception exception)
        {
            if (exception.GetType() != ExpectedException)
            {
                base.RethrowIfAssertException(exception);
                string msg = string.Format("Test method {0}.{1} "
                                           + "threw exception {2} but {3} was expected.",
                                           base.TestContext.FullyQualifiedTestClassName, base.TestContext.TestName,
                                           exception.GetType().FullName, ExpectedException.FullName);
                throw new Exception(msg);
            }

            var foutNummerException = exception as FoutNummerException;
            var faultException = exception as FaultException<FoutNummerFault>;

            if (foutNummerException != null && foutNummerException.FoutNummer != ExpectedFoutNummer)
            {
                string msg = string.Format("Test method {0}.{1} threw expected "
                                           + "exception {2} with error code {4} but error code {5} was expected.",
                                           base.TestContext.FullyQualifiedTestClassName, base.TestContext.TestName,
                                           exception.GetType().FullName, ExpectedException.FullName,
                                           foutNummerException.FoutNummer, ExpectedFoutNummer);
                throw new Exception(msg);
            }
            else if (faultException != null && faultException.Detail.FoutNummer != ExpectedFoutNummer)
            {
                string msg = string.Format("Test method {0}.{1} threw expected "
                                           + "exception {2} with error code {4} but error code {5} was expected.",
                                           base.TestContext.FullyQualifiedTestClassName, base.TestContext.TestName,
                                           exception.GetType().FullName, ExpectedException.FullName,
                                           faultException.Detail.FoutNummer, ExpectedFoutNummer);
                throw new Exception(msg);               
            }
            else if (foutNummerException == null && faultException == null)
            {
                string msg = string.Format("Test method {0}.{1} threw "
                                           + "exception {2} without error code, but error code {4} was expected.",
                                           base.TestContext.FullyQualifiedTestClassName, base.TestContext.TestName,
                                           exception.GetType().FullName, ExpectedException.FullName, ExpectedFoutNummer);
                throw new Exception(msg);               
                
            }

        }
    }
}
