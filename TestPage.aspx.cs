using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SFLite
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = Request.Url.ToString();
            Response.Redirect("Logon.aspx?&wfid=34897&su=2&sc=855691&ai=0x0100000041463C964DBB76F9D9CEDBA59FF8DB4C3613982DB8F5EE170A39BF7DC192ACF4D6B24423529AD670");
        }
    }
}

