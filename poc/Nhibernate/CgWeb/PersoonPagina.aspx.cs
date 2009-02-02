using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CgDal;

namespace CgWeb
{
    public partial class PersoonPagina : System.Web.UI.Page
    {

        private Persoon persoon = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (PersonenServiceReference.PersonenServiceClient service = new CgWeb.PersonenServiceReference.PersonenServiceClient())
                {
                    persoon = service.PersoonMetDetailsGet(int.Parse(Request["Id"]));
                    Session["persoon"] = persoon;
                    PersoonUserControl1.Persoon = persoon;

                    PersoonUserControl1.GegevensBijwerken();

                }
            }
            else
            {
                persoon = (Persoon)Session["persoon"];
                PersoonUserControl1.Persoon = persoon;

            }

        }

        protected void bewarenButton_Click(object sender, EventArgs e)
        {
            PersoonUserControl1.PersoonBijwerken();
            using (PersonenServiceReference.PersonenServiceClient service = new CgWeb.PersonenServiceReference.PersonenServiceClient())
            {
                service.PersoonUpdaten(persoon);
            }
            
        }
    }
}
