using System;

namespace Cg2.Adf
{
	/// <summary>
	/// Represent the error that occurs when no registered ServiceProvider manages to resolve a service interface.
	/// </summary>
	public class ServiceActivationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the ServiceActivationException class.
		/// </summary>
        public ServiceActivationException() {}

		/// <summary>
		/// Initializes a new instance of the ServiceActivationException class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
        public ServiceActivationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of the ServiceActivationException class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, 
		/// or a null refernce if no inner exception is specified.
		/// </param>
		public ServiceActivationException(string message, Exception innerException) : base(message, innerException) { }

	}
}