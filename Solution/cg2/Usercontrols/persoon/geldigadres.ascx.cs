using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CgDal;

namespace cg2.Usercontrols.persoon
{
    public partial class geldigadres : System.Web.UI.UserControl
    {

        private Persoon persoon;
        protected void Page_Load(object sender, EventArgs e)
        {
            PersoonsAdres persoonsAdres = persoon.PersoonsAdres.First(a => a.IsStandaard == true);

            //de nodige gegevens laden
            persoonsAdres.AdresReference.Load();
            persoonsAdres.Adres.SubgemeenteReference.Load();
            persoonsAdres.Adres.Subgemeente.PostNrReference.Load();
            persoonsAdres.Adres.StraatReference.Load();

            gemeente.Text = persoonsAdres.Adres.Subgemeente.Naam;
            postcode.Text = persoonsAdres.Adres.Subgemeente.PostNr.PostNr1.ToString();
            straat.Text = persoonsAdres.Adres.Straat.Naam;
            //nr.Text = persoonsAdres.Adres.HuisNr.ToString();
        }

        public Persoon Persoon
        {
            get
            {
                return persoon;
            }
            set
            {
                persoon = value;
            }
        }
    }
}