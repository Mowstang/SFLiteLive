using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Doxess.Data;
using System.Data;
using System.Web.UI.HtmlControls;

namespace SFLite
{
    public partial class _Default : System.Web.UI.Page
    {
        private string LiteROLE
        {
            get { return (string)ViewState["LiteROLE"]; }
            set { ViewState["LiteROLE"] = value; }
        }
        private string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        DataAccess dba = new DataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.cat = Request.QueryString["cat"];
                if (this.cat != null) this.Response.Cookies.Add(new HttpCookie("CatID", this.cat.ToString()));
            }
            if (Request.Cookies["MYLITEROLE"] != null)
            {
                this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
            }
            if (this.LiteROLE == "C")
            {
                Page.Master.FindControl("mpSrch").Visible = false;
                HtmlAnchor a1 = (HtmlAnchor)Page.Master.FindControl("action1");
                a1.InnerText = "Scheduled Jobs";
                //Page.Master.FindControl("title");
            }
        }

        public void getNews()
        {
            string newsCards = "";
            this.cat = this.Request.Cookies["CatID"].Value.ToString();


            DataTable index = new DataTable();
            index = dba.dxGetTable("SELECT top 5 * FROM StrNews where PostedUntildate>=getdate() order by PostedUntildate desc");
            if (this.cat != "20")
            {
                newsCards += "        <br><br>";
                newsCards += "        <div id='wrapper' style='text-align: center'>";
                newsCards += "            <div style='display: inline-block;'>";
                newsCards += "                <img src='Images/SFLsplashvendors.png' width='70%' />";
                newsCards += "            </div>";
                newsCards += "        </div>";
            }
            if (index.Rows.Count == 0)
            {
                newsCards += "        <br><br>";
                newsCards += "        <div id='wrapper' style='text-align: center'>";  
                newsCards += "            <div style='display: inline-block;'>";
                newsCards += "                <img src='Images/SFLsplash.png' width='70%' />";
                newsCards += "            </div>";
                newsCards += "        </div>";
            }
            else
            {
                foreach (DataRow row in index.Rows)
                {
                    newsCards += "<br><div class='collapse-card'>";
                    newsCards += "    <div class='collapse-card__heading'>";
                    if (row["ImageVirtualPath"].ToString() != "")
                    {
                        newsCards += "        <img src='https://www.mysmartflow.ca" + row["ImageVirtualPath"].ToString() + "' width='96%' />";
                    }
                    newsCards += "        <h5 class='collapse-card__title'>";
                    newsCards += "            <i class='fa fa-hand-o-right fa-2x fa-fw'></i>";
                    newsCards += "            &nbsp;&nbsp;<b>" + row["NewsTitle"].ToString() + "</b>";
                    newsCards += "        </h5>";
                    newsCards += "    </div>";
                    newsCards += "    <div class='collapse-card__body'>";
                    newsCards += "        <b>" + row["NewsTitle"].ToString() + "</b><br />";
                    string story = row["NewsDetail"].ToString();
                    string[] parts = story.Split('>');
                    int x = parts.Length;
                    int mx = 0;
                    do
                    {
                        if (parts[x - 1].Replace("-", "") == "")
                        {
                            parts[x - 1] = "";
                            x -= 1;
                        }
                        else mx = x;
                    } while (mx == 0);
                    newsCards += story + "<br />";
                    newsCards += "    </div>";
                    newsCards += "</div>";
                }
            }
            Response.Write(newsCards);
        }
    }

}
