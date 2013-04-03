using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Models;

namespace Chiro.Gap.WebApp.Controllers
{
    /// <summary>
    /// Controller voor de koppeling met de verzekeraar (IC-verzekeringen)
    /// </summary>
    public class VerzekeringController : BaseController
    {
        public VerzekeringController(IVeelGebruikt veelGebruikt) : base(veelGebruikt)
        {
        }

        public override ActionResult Index(int groepID)
        {
            string verzekeringsUrl;
            try
            {
                verzekeringsUrl = ServiceHelper.CallService<IGebruikersService, string>(svc => svc.VerzekeringsUrlGet(groepID));
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                if (ex.Detail.FoutNummer == FoutNummer.EMailVerplicht)
                {
                    var model = new MasterViewModel();
                    BaseModelInit(model, groepID);

                    return View("EmailOntbreekt", model);
                }
                else
                {
                    throw;
                }
            }
            return Redirect(verzekeringsUrl);
        }
    }
}
