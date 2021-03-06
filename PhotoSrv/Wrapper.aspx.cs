﻿using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Doxess.Web.WorkFlow.Security;
using Doxess.Data;
using Doxess.Web.WorkFlow;
using System.Data.SqlClient;
using ICSharpCode.SharpZipLib.Zip;

namespace SFLite.PhotoSrv
{
    public partial class Wrapper : System.Web.UI.Page
    {
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
        DataAccess dba = new DataAccess();

        #region Private Property
        private string errMessage;
		private int detId;
        private int WfId;
        private string file;
        private string room;
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {

            // Put user code to initialize the page here
            if (!this.IsPostBack)
            {
                this.uploader.Attributes.Add("src", "Uploader.aspx?" + Request.QueryString);
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
            this.file = Request.QueryString["fi"];
            this.room = Request.QueryString["rm"];
            if (this.hidUpup.Value != "") CheckUploadResults();
        }

        public void getFile()
        {
            Response.Write(this.file + "<br>" + dba.dxGetTextData("select item from wfOptions where optype='rooms' and id=" + this.room));
        }

        private void CheckUploadResults()
        {
            if (hidUpup.Value.Substring(0, 7) == "Success")
            {
                // Add file to 'zip' archive
                // If the zip archive does not already exist, create it and add
                // an entry into the StrFilesDet table, treating it as though it
                // were a single uploaded photo (E.g. it appears in Pictures tab for the claim)

                // Create an instance of the FastZip class and add the files
                // that have just been uploaded
                errMessage = "";
                DataAccess dba = new DataAccess();
                string PicBasePath = ConfigurationSettings.AppSettings["PictureBasePath"];

                FastZip fz = new FastZip();
                //fz.CreateZip(PicBasePath + this.WfId.ToString() + "\\PictureBundle.zip", PicBasePath + this.WfId.ToString(), false, ".jpg;-tn_", null);

                this.AddZipToImagesTable(PicBasePath);

                //// we want to just close here...
                //ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.open('close.htm', '_self', null);", true);
                this.Response.Redirect("../Claims.aspx");

            }
            else
            {
                errMessage = hidUpup.Value;
            }
        }

        private int AddZipToImagesTable(string PicBasePath)
        {
            //WfUser user = (WfUser)this.User;
            //if (user != null)
            //{
            //    this.userId = Convert.ToInt32(user.wfUserID);
            //}

            DataAccess dba = new DataAccess();

            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                SqlCommand command = conn.CreateCommand();

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ZipSaveUpload";
                command.Parameters.Clear();
                SqlParameter fileId = command.Parameters.Add("@fileId", SqlDbType.Int);
                fileId.Direction = ParameterDirection.Output;

                command.Parameters.Add("@wfId", this.WfId);
                command.Parameters.Add("@fileName", "PictureBundle.zip");
                command.Parameters.Add("@filePath", PicBasePath + this.WfId.ToString() + "\\");
                command.Parameters.Add("@desc", "Photo pack");
                command.Parameters.Add("@userId", userID);
                command.Parameters.Add("@viewAdjuster", true);
                command.Parameters.Add("@viewClient", true);
                command.Parameters.Add("@viewIC", true);

                try
                {
                    command.ExecuteNonQuery();
                    if (fileId != null)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (SqlException ex)
                {
                    return -1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }


    }
}