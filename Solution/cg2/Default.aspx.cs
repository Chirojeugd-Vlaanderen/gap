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
using CgDal;
using ChiroGroepModel;

namespace cg2
{
    public partial class Default : System.Web.UI.Page
    {
        private ChiroGroepEntities db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = DBFactory.Databaseinstance;
            GridView1.DataSource = db.Groep;
            GridView1.DataBind();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
