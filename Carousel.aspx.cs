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
    public partial class Carousel : System.Web.UI.Page
    {
        public DataTable rms = new DataTable();
        DataAccess dba = new DataAccess();

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
                hfParams.Value = Request.QueryString.ToString();
                hfStage.Value = "page";
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }
                if (Request.Cookies["MYLITEROLE"] != null)
                {
                    this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
                }
                this.userName = dba.dxGetTextData("select FirstName+' '+LastName from wfUsers where userid=" + this.userID.ToString());
            }

            this.WfId = Convert.ToInt32((string)Request.QueryString["wfid"]);
            this.userID = Convert.ToInt32((string)Request.QueryString["uid"]);
            this.file = Request.QueryString["fi"];
            this.loc = Request.QueryString["loc"];
            string img = Request.QueryString["img"];
            // hfParams.Value = "wfid=" + this.WfId.ToString() + "&fi=" + this.file + "&uid=" + this.userID.ToString();
            ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:PicsCallBack(" + img + "); ", true);
        }

        public void getRoom()
        {
            Response.Write(this.loc + " - " + this.file);
        }

        public void getCarousel()  
        {
            string cul=dba.dxGetTextData("Select Culture from wfuserpref where userid=" + userID);
            
           
            
            
            string imgroot = dba.dxGetTextData("select PortalWebAddress from wfSysRec");
            if (imgroot.IndexOf(".strone.ca") > 0)
                imgroot = "http://www.mysmartflow.ca:41459/StroneFilesDirect/Photos/" + this.WfId.ToString() + "/";
            else imgroot = "http://localhost:41459/StroneFilesDirect/Photos/" + this.WfId.ToString() + "/";
            string fileNo = dba.dxGetTextData("select fileNo from claims where wfid = " + this.WfId.ToString());
            DataTable data;
            if (cul == "en")
            {
                data = dba.dxGetTable("ImageGetListByFileNo '" + fileNo + "', '" + loc + "'");
            }
            else
            {
                data = dba.dxGetTable("ImageGetListByFileNo_fr '" + fileNo + "', '" + loc + "'");
            }
            string swiper = "";
            int j = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];
                string filePath = row["imagePath"].ToString();
                string imageSrc = imgroot + row["imageName"].ToString();
                swiper += "<li><img src='" + imageSrc + "' width='400' height='500'/></li>";
                // +row["imageName"].ToString() + "' href='#'><img src='" + imageSrc + "' width='500' /></a></div>";
            }
            Response.Write(swiper);
        }

    }
}