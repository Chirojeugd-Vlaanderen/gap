using System;
namespace Cg2.Adf.ServiceModel.Extensions
{
    /// <summary>
    /// TODO: Documenteren!
    /// </summary>
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

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
		protected internal abstract void Initialize();

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
		protected internal abstract void Dispose();
	}
}