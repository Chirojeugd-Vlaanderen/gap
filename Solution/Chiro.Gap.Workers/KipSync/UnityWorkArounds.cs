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

    public partial class SyncPersoonServiceClient
    {
        // Make sure tis constructor is used by Unity:
        [InjectionConstructor]
        public SyncPersoonServiceClient(object dummy) : this()
        {
        }
    }
}