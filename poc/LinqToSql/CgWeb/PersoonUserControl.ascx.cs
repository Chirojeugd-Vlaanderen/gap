using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CgWeb
{
    public partial class PersoonUserControl : System.Web.UI.UserControl
    {
        private PersonenServiceReference.Persoon persoon;

        public PersonenServiceReference.Persoon Persoon
        {
            get { return persoon; }
            set { persoon = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Toont gegevens van gekoppelde persoon.
        /// </summary>
        public void GegevensBijwerken()
        {
            geboorteDatumTextBox.Text = persoon.GeboorteDatum.ToString();
            naamTextBox.Text = persoon.Naam;
            voorNaamTextBox.Text = persoon.VoorNaam;
            persoonIDLabel.Text = String.Format("{0}", persoon.PersoonID);
        }

        /// <summary>
        /// Neemt formgegevens over in gekoppeld persoonsobject
        /// </summary>
        public void PersoonBijwerken()
        {
            persoon.GeboorteDatum = DateTime.Parse(geboorteDatumTextBox.Text);
            persoon.Naam = naamTextBox.Text;
            persoon.VoorNaam = voorNaamTextBox.Text;

            if (persoon.Status == CgWeb.PersonenServiceReference.EntityStatus.Geen)
            {
                persoon.Status = CgWeb.PersonenServiceReference.EntityStatus.Gewijzigd;
            }
        }

    }
}