using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cg2.Core.Domain;
using System.Web.UI.WebControls;

namespace WebApp.Views.Groepen
{
    public partial class GroepenLijstControl : System.Web.Mvc.ViewUserControl<Groep>
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            List<Groep> lijst = new List<Groep>();
            lijst.Add(ViewData.Model);

            groepenDataLijst.DataSource = lijst;
            groepenDataLijst.DataBind();
        }
    }
}
