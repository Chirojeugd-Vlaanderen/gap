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

namespace cg2
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ChiroGroepEntities chiroGroepContext = new ChiroGroepEntities();
            GridView1.DataSource = chiroGroepContext.Persoon;
            GridView1.DataBind();
        }
    }
}
