using System;

namespace Chiro.Adf.ServiceModel.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public abstract class CallContext
	{
        /// <summary>
        /// 
        /// </summary>
		[ThreadStatic] // who needs OperationContext? ;-)
		private static CallContext _callContext;

        /// <summary>
        /// Gets the context of the current operation.
        /// </summary>
        /// <remarks></remarks>
		public static CallContext Current
		{
			get { return _callContext; }
			internal set { _callContext = value;}
		}


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <remarks></remarks>
		protected internal abstract void Initialize();


        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <remarks></remarks>
		protected internal abstract void Dispose();
	}
}