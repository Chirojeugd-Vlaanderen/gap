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
using ChiroGroepModel;

namespace cg2
{
    public partial class _Default : System.Web.UI.Page
    {
        private ChiroGroepEntities chiroGroepContext;

        protected void Page_Load(object sender, EventArgs e)
        {
            chiroGroepContext = new ChiroGroepEntities();

            ObjectQuery<Persoon> query = chiroGroepContext.Persoon.Include("Groep");

            lijst.DataSource = query;
            lijst.DataTextField = "Naam";
            lijst.DataBind();
        }
    }
}
