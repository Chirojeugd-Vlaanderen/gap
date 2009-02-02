using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CgDal;

namespace CgWeb
{
    public partial class PersoonUserControl : System.Web.UI.UserControl
    {
        private Persoon persoon;

        public Persoon Persoon
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

            telefoonNrGrid.DataSource = from cv in persoon.CommunicatieVorms
                                        where cv.CommunicatieTypeID == 1
                                        select cv;

            telefoonNrGrid.DataBind();
        }

        /// <summary>
        /// Neemt formgegevens over in gekoppeld persoonsobject
        /// </summary>
        public void PersoonBijwerken()
        {
            persoon.GeboorteDatum = DateTime.Parse(geboorteDatumTextBox.Text);
            persoon.Naam = naamTextBox.Text;
            persoon.VoorNaam = voorNaamTextBox.Text;


        }

        protected void toevoegenButton_Click(object sender, EventArgs e)
        {
            CommunicatieVorm tel = new CommunicatieVorm();
            tel.Status = EntityStatus.Nieuw;
            tel.Nummer = nieuwNrTextBox.Text;
            tel.CommunicatieTypeID = 1;
            tel.PersoonID = persoon.PersoonID;
            tel.Voorkeur = false;
            tel.Status = EntityStatus.Nieuw;
            
            persoon.CommunicatieVorms.Add(tel);

            telefoonNrGrid.DataSource = from cv in persoon.CommunicatieVorms
                                        where cv.CommunicatieTypeID == 1
                                        select cv;

            telefoonNrGrid.DataBind();
        }

    }
}