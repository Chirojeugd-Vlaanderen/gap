using System;
namespace Capgemini.Adf.ServiceModel.Extensions
{
	public abstract class CallContext
	{
		[ThreadStatic] // who needs OperationContext? ;-)
		private static CallContext callContext;

		/// <summary>
		/// Gets the context of the current operation.
		/// </summary>
		public static CallContext Current
		{
			get { return callContext; }
			internal set { callContext = value;}
		}

		protected internal abstract void Initialize();

		protected internal abstract void Dispose();
	}
}