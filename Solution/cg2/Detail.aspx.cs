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

namespace cg2
{
    public partial class Detail : System.Web.UI.Page
    {
        private Groep groep;
        private ChiroGroepEntities db = DBFactory.Databaseinstance;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string stamnr = Request.QueryString["stamnr"];
                if (stamnr != null)
                {
                    groep = db.Groep.FirstOrDefault(g => g.Code == stamnr);

                    if (groep != null)
                    {
                        groep.Persoon.Load();
                        Titel.Text = "Personen van de groep " + groep.Naam;

                        GridView1.DataSource = groep.Persoon;
                        GridView1.DataBind();
                    }
                    else
                    {
                        Titel.Text = "Groep niet gevonden";
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
