/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
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
        public VerzekeringController(IVeelGebruikt veelGebruikt, ServiceHelper serviceHelper) : base(veelGebruikt, serviceHelper)
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
                if (ex.Detail.FoutNummer == FoutNummer.KoppelingLoginPersoonOntbreekt)
                {
                    var model = new MasterViewModel();
                    BaseModelInit(model, groepID);

                    return View("GavKoppelingOntbreekt", model);
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
