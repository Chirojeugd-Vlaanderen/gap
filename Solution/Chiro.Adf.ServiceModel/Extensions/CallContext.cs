using System;

namespace Chiro.Adf.ServiceModel.Extensions
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
	public abstract class CallContext
	{
		[ThreadStatic] // who needs OperationContext? ;-)
		private static CallContext _callContext;

		/// <summary>
		/// Gets the context of the current operation.
		/// </summary>
		public static CallContext Current
		{
			get { return _callContext; }
			internal set { _callContext = value;}
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
		protected internal abstract void Initialize();

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
		protected internal abstract void Dispose();
	}
}