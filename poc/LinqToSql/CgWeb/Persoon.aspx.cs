using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CgWeb
{
    public partial class Persoon : System.Web.UI.Page
    {
        private PersonenServiceReference.Persoon persoon;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (PersonenServiceReference.PersonenServiceClient service = new CgWeb.PersonenServiceReference.PersonenServiceClient())
            {
                persoon = service.PersoonGet(int.Parse(Request["Id"]));
            }
            PersoonUserControl1.Persoon = persoon;
        }
    }
}
