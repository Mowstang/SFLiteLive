using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Doxess.Data;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SFLite
{
    public partial class ViewImages : System.Web.UI.Page
    {
        public DataTable rms = new DataTable();
        DataAccess dba = new DataAccess();
        private string rootUrl = "";

        private int userID
        {
            get { return (int)ViewState["userID"]; }
            set { ViewState["userID"] = value; }
        }
        private string LiteROLE
        {
            get { return (string)ViewState["LiteROLE"]; }
            set { ViewState["LiteROLE"] = value; }
        }
        public string userName
        {
            get { return (string)ViewState["userName"]; }
            set { ViewState["userName"] = value; }
        }
        private int WfId
        {
            get { return (int)ViewState["WfId"]; }
            set { ViewState["WfId"] = value; }
        }
        private string file
        {
            get { return (string)ViewState["file"]; }
            set { ViewState["file"] = value; }
        }
        private string loc
        {
            get { return (string)ViewState["loc"]; }
            set { ViewState["loc"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }
                if (Request.Cookies["MYLITEROLE"] != null)
                {
                    this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
                }
                this.userName = dba.dxGetTextData("select FirstName+' '+LastName from wfUsers where userid=" + this.userID.ToString());
                if (this.LiteROLE == "P") this.userName = "Active Claims for " + this.userName;
                if (this.LiteROLE == "C")
                {
                    this.userName = "Scheduled Jobs for " + this.userName;
                    Page.Master.FindControl("mpSrch").Visible = false;
                    HtmlAnchor a1 = (HtmlAnchor)Page.Master.FindControl("action1");
                    a1.InnerText = "Scheduled Jobs";
                }
            }

            this.WfId = Convert.ToInt32((string)Request.QueryString["wfid"]);
            this.userID = Convert.ToInt32((string)Request.QueryString["uid"]);
            this.file = Request.QueryString["fi"];
            this.loc = Request.QueryString["loc"];
            string tgt = Request["__EVENTTARGET"];
        }

        public void getRoom()
        {
            Response.Write(this.loc + " - " + this.file);
        }
        public void getSlides()
        {
            string fileNo = dba.dxGetTextData("select fileNo from claims where wfid = " + this.WfId.ToString());
            DataTable data = dba.dxGetTable("ImageGetListByFileNo '" + fileNo + "', '" + loc + "'");
            string imgroot = "/strwebflow/claimfiles/" + this.WfId.ToString() + "/";

            string slides = "";
            for(int i = 0; i < data.Rows.Count; i++) 
            {
				DataRow row = data.Rows[i];
                string filePath = row["imagePath"].ToString();
                string imageSrc = imgroot + row["imageName"].ToString();
                slides += "<div class='swiper-slide'><img src='" + imageSrc + "' alt = '' width='120px' height='120px'></div>";
            }
            Response.Write(slides); 
        }
        public string ImageVirtualTNFullPath(string filepath, string appRootPath)
        {
            int pos = 0;
            try
            {
                filepath = filepath.ToLower().Replace(@"\", "/").Trim();

                //pos = this._filePath.IndexOf(appRootPath.ToLower());
                pos = filepath.IndexOf("/claimfiles/");
                return "~" + filepath.Substring(pos);
            }
            catch (Exception ex)
            {
                return ex.Message + "pos: " + filepath;
            }

        }

    }
}