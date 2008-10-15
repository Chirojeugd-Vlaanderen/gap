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
        private PersonenServiceReference.Persoon persoon = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (PersonenServiceReference.PersonenServiceClient service = new CgWeb.PersonenServiceReference.PersonenServiceClient())
            {
                persoon = service.PersoonGet(int.Parse(Request["Id"]));
            }
            PersoonUserControl1.Persoon = persoon;

            if (!IsPostBack)
            {
                // Als geen postback: gegevens persoon tonen
                // In het andere geval niet, want anders weten we
                // niet meer wat de nieuw ingevulde gegevens zijn.

                PersoonUserControl1.GegevensBijwerken();    
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
