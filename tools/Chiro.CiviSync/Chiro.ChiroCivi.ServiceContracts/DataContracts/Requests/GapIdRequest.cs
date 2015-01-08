using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviCrm.BehaviorExtension;
using Newtonsoft.Json;

namespace Chiro.ChiroCivi.ServiceContracts.DataContracts.Requests
{
    [JsonConvertible]
    public class GapIdRequest: BaseRequest
    {
        [JsonProperty("custom_10")]
        public int GapId { get; set; }

        public GapIdRequest() : base()
        {
        }

        public GapIdRequest(int gapId) : this()
        {
            this.GapId = gapId;
        }
    }
}
