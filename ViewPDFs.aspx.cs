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
    public partial class ViewPDFs : System.Web.UI.Page
    {
        public DataTable rms = new DataTable();
        DataAccess dba = new DataAccess();

        private int DocID
        {
            get { return (int)ViewState["DocID"]; }
            set { ViewState["DocID"] = value; }
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

        private string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                hfParams.Value = Request.QueryString.ToString();
                hfStage.Value = "page";

                    this.DocID = Convert.ToInt32((string)Request.QueryString["DocID"]);

                DisplayPDF(DocID);
                
            }
        }
        protected void DisplayPDF(int DocID)
        {
            WorkFlow.BLL.Claim.UploadDoc doc = new WorkFlow.BLL.Claim.UploadDoc(DocID);

            this.Response.ContentType = "octet/stream";  //application/pdf msword
            this.EnableViewState = false;
            Response.Charset = "";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + doc.FileName);
            this.Response.WriteFile(doc.FileFullPath);
            this.Response.End();
        }
        

    }
}