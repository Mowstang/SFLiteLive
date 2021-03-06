﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Doxess.Data;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;

namespace SFLite
{
    public partial class ViewDocs : System.Web.UI.Page
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
        private string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        public string JobName
        {
            get { return (string)ViewState["JobName"]; }
            set { ViewState["JobName"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                hfParams.Value = Request.QueryString.ToString();
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
            this.cat = Request.QueryString["cat"];
            this.JobName = Request.QueryString["cl"];
            hfParams.Value = "wfid=" + this.WfId.ToString() + "&cat=" + cat + "&uid=" + this.userID.ToString();
            Params.Value = hfParams.Value + "&fi=" + file + "&cl=" + JobName;
            
        }

        public void getRoom()
        {
            //Response.Write(this.loc + " - " + this.file);
        }
        public void getSlides()
        {

            string imgroot = dba.dxGetTextData("select PortalWebAddress from wfSysRec");
            string strDBfilePath = "https://www.mysmartflow.ca/strwebflow/claimfiles/";
            string pdfDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
            var uploadPath = System.Configuration.ConfigurationSettings.AppSettings["PictureBasePath"] + WfId + "\\DocTN\\";

            if (!System.IO.Directory.Exists(uploadPath))
            {
                System.IO.Directory.CreateDirectory(uploadPath);
            }

            
            DataTable data;
            dba.dxAddParameter("@wfid", WfId);
            dba.dxAddParameter("@UserID", userID);
            dba.dxAddParameter("@cat", cat);
            data = dba.dxGetSPData("oGetDocListByFileNoUserId");


            string slides = "";
            int j = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];
                string FileName = row["FileName"].ToString();
                string filePath = row["FilePath"].ToString();
                

                int lastPos = FileName.LastIndexOf(@".");
                string suffix = FileName.Substring(lastPos);
                string prefix = FileName.Substring(0, lastPos);
                FileName=prefix+".png";
                filePath = WfId.ToString();

                string file = uploadPath + FileName;
                
                if (!System.IO.File.Exists(file))
                {
                    string file_pdf = pdfDBfilePath + filePath + '\\' + prefix + suffix;
                    ConvertPdfToImage(file_pdf,file);
                }


                file = strDBfilePath+filePath + "/DocTN/" + prefix + ".png";
                string id = "lblFileComments" + i.ToString();
                
                slides += "<div class='col-1-3' onclick='ShowPDF(" + row["KeyId"] +");' >";
                slides += "<img style='padding-right:5px;' src='" + file + "' alt = '' width='115px' height='115px' />";
                slides += "<div class='ui floated hlabel'><br><b>" + row["Comments"] + "</b></div>";
                slides += "</div>";
            }
            Response.Write(slides); 
        }
        static void ConvertPdfToImage(string FileName,string saveName)
        {
            // Convert PDF 1st page to PNG file.
            
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            // this property is necessary only for registered version.
            f.Serial = "10275840041";

            //string pdfPath = @"..\..\..\..\..\simple text.pdf";
            string imagePath = saveName; //Path.ChangeExtension(FileName, ".png");

            f.OpenPdf(FileName);

            if (f.PageCount > 0)
            {
                //save 1st page to png file, 120 dpi
                f.ImageOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;
                f.ImageOptions.Dpi = 120;
                if (f.ToImage(imagePath, 1) == 0)
                {
                    // 0 - converting successfully                
                    // 2 - can't create output file, check the output path
                    // 3 - converting failed
                    System.Diagnostics.Process.Start(imagePath);
                }
            }
            f.ClosePdf();
        }

   

    }
}