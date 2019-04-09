using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Doxess.Web.WorkFlow.Security;

namespace SFLite
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private int userID
        {
            get { return (int)ViewState["userID"]; }
            set { ViewState["userID"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                try
                {
                    if (Request.Cookies["userID"] != null)
                    {
                        this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                    }
                    else
                    {
                        this.Response.Redirect("logon.aspx");
                    }
                }
                catch (Exception)
                {
                    this.Response.Redirect("logon.aspx");
                }
            }

            this.Page.ClientScript.GetPostBackEventReference(this, string.Empty);
            string tgt = Request["__EVENTTARGET"];
            string arg = hf0.Value.ToString();

            if (tgt == "Search")
            {
                this.Response.Redirect("SearchResults.aspx?tm=" + arg);
            }
        }

    }
}
