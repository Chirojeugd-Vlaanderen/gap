using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Validatie;
using System.Web.Mvc;
using Cg2.Fouten;

namespace Chiro.Gap.WebApp
{
    /// <summary>
    /// Een ModelStateWrapper pakt een ModelStateDictionary in
    /// in een IValidatieDictionary, zodat die door
    /// Cg2.Validatie gebruikt kan worden.
    /// </summary>
    public class ModelStateWrapper: IValidatieDictionary
    {
        private ModelStateDictionary _modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public void BerichtToevoegen(string key, string bericht)
        {
            _modelState.AddModelError(key, bericht);
        }

        public bool IsGeldig
        {
            get { return _modelState.IsValid; }
        }
    }
}
