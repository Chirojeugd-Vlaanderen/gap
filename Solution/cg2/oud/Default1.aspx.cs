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
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using CgDal;

namespace cg2
{
    public partial class _Default : System.Web.UI.Page
    {
        private ChiroGroepEntities chiroGroepContext;

        protected void Page_Load(object sender, EventArgs e)
        {
            VulGridView();
        }

        /// <summary>
        /// VulGridView vult het 'grid' met alle personen gerelateerd aan de groep
        /// met Id DommeConstanten.TestGroepId.
        /// </summary>
        private void VulGridView()
        {
            chiroGroepContext = new ChiroGroepEntities();

            ObjectQuery<Groep> query = chiroGroepContext.Groep.Include("Persoon");

            Console.WriteLine(query.Count());

            var mijnGroep = from g in query
                            where g.GroepID == 315//DommeConstanten.TestGroepId
                            select g;

            groepsLabel.Text = mijnGroep.First().Naam;

            var mijnPersonen = mijnGroep.First().Persoon;

            var teTonenInfo = from p in mijnPersonen
                              orderby p.Naam
                              select p; // new { VoorNaam = p.VoorNaam, Naam = p.Naam };

            lijst.DataSource = teTonenInfo;
            lijst.DataBind();
        }

        protected void lijst_SelectedIndexChanged(object sender, EventArgs e)
        {
            domLabel.Text = "HALLO!";
        }

      }
}
