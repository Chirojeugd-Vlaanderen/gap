using Microsoft.Practices.Unity;

namespace Chiro.Gap.Workers.KipSync
{
    //public class WorkaroundSyncPersoonServiceClient : SyncPersoonServiceClient
    //{
    //    public WorkaroundSyncPersoonServiceClient() : base() { }
    //}

    public partial class SyncPersoonServiceClient
    {
        // Make sure tis constructor is used by Unity:
        [InjectionConstructor]
        public SyncPersoonServiceClient(object dummy) : this()
        {
        }

    }

}