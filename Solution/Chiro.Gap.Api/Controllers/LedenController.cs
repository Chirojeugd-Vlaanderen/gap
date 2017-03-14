// Source: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Api.Models;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Api.Controllers
{
    [RoutePrefix("api/Leden")]
    public class LedenController : BaseController
    {
        public LedenController(ServiceHelper serviceHelper) : base(serviceHelper)
        {
        }

        [Authorize]
        [Route("{groepId}")]
        public IHttpActionResult Get(int groepId)
        {
            var svcResult = ServiceHelper.CallService<ILedenService, IList<PersoonLidInfo>>(
                svc => svc.ActieveLedenOphalen(groepId));
            var result = Mapper.Map<IList<PersoonLidInfo>, PersoonModel[]>(svcResult);

            // Return dummy data.
            return Ok(result);
        }

    }
}