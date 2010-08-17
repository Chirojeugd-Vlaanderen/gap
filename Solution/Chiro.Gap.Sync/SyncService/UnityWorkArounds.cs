using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

namespace Chiro.Gap.Sync.SyncService
{
	/// <summary>
	/// Breid SyncService uit met constructor zodat unity weet dewelke te kiezen.
	/// </summary>
	public partial class SyncPersoonServiceClient
	{
		/// <summary>
		/// Zorgt ervoor dat Unity de constructor zonder parameters gebruikt voor IoC.
		/// </summary>
		/// <param name="dummy">Parameter zonder doel, maar deze signature is blijkbaar wel nodig voor
		/// de InjectionConstructor</param>
		[InjectionConstructor]
		public SyncPersoonServiceClient(object dummy): this() {}
	}
}
