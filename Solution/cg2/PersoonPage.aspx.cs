using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ChiroGroepModel;
using CgDal;
using cg2.Usercontrols.persoon;

namespace cg2
{
    public partial class PersoonPage : System.Web.UI.Page
    {
        private Persoon persoon;
        private ChiroGroepEntities db = DBFactory.Databaseinstance;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int id = int.Parse(Request.QueryString["id"]);
                if (id != 0)
                {
                    persoon = db.Persoon.FirstOrDefault(p => p.PersoonID == id);

                    if (persoon != null)
                    {
                        titel.Text = "Details voor " + persoon.Naam + " " + persoon.VoorNaam;
                        naam.Text = persoon.Naam;
                        voornaam.Text = persoon.VoorNaam;

                        persoon.PersoonsAdres.Load();

                        //controleren of de persoon een adres heeft.
                        if (persoon.PersoonsAdres.Count() > 0)
                        {
                            geldigadres UC = new geldigadres();
                            UC.Persoon = persoon;
                        }
                        else
                        {
                            
                        }
                    }
                    else
                    {
                        titel.Text = "Persoon niet gevonden";
                    }
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }
    }
}
