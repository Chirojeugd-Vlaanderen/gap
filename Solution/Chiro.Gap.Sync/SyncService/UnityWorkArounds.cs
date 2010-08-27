// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
		public SyncPersoonServiceClient(object dummy) : this() { }
	}
}
