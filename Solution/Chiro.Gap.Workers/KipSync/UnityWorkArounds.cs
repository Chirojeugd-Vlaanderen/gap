// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Microsoft.Practices.Unity;

namespace Chiro.Gap.Workers.KipSync
{
    // public class WorkaroundSyncPersoonServiceClient : SyncPersoonServiceClient
    // {
    //    public WorkaroundSyncPersoonServiceClient() : base() { }
    // }

	/// <summary>
	/// TODO (#190): Documenteren
	/// </summary>
    public partial class SyncPersoonServiceClient
    {
		/// <summary>
		/// Make sure this constructor is used by Unity:
		/// </summary>
		/// <param name="dummy"></param>
        [InjectionConstructor]
        public SyncPersoonServiceClient(object dummy) : this()
        {
        }
    }
}