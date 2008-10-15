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
            set { PersoonToekennen(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void PersoonToekennen(PersonenServiceReference.Persoon persoon)
        {
            this.persoon = persoon;
            if (persoon.GeboorteDatum != null)
            {
                geboorteDatumCalendar.SelectedDate = (DateTime)persoon.GeboorteDatum;
                geboorteDatumCalendar.VisibleDate = (DateTime)persoon.GeboorteDatum;
            }
            naamTextBox.Text = persoon.Naam;
            voorNaamTextBox.Text = persoon.VoorNaam;
        }
    }
}