﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Doxess.Data;
using Doxess.Web.WorkFlow;
using WorkFlow.BLL.Claim;
using System.Collections;
using System.Data.SqlClient;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace SFLite
{
    public partial class Uploader : System.Web.UI.Page
    {
        public DataTable index = new DataTable();
        public DataTable rms = new DataTable();

        private int userID
        {
            get { return (int)ViewState["userID"]; }
            set { ViewState["userID"] = value; }
        }
        private int FileId
        {
            get { return (int)ViewState["FileId"]; }
            set { ViewState["FileId"] = value; }
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
        public string JobName
        {
            get { return (string)ViewState["JobName"]; }
            set { ViewState["JobName"] = value; }
        }
        private string FileNo
        {
            get { return ViewState["FileNo"].ToString(); }
            set { ViewState["FileNo"] = value; }
        }
        private int WfId
        {
            get { return (int)ViewState["WfId"]; }
            set { ViewState["WfId"] = value; }
        }
        private int schId
        {
            get { return (int)ViewState["schId"]; }
            set { ViewState["schId"] = value; }
        }
        private string file
        {
            get { return (string)ViewState["file"]; }
            set { ViewState["file"] = value; }
        }
        public string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        private int SubProcessId { 
            get { return (int)ViewState["SubId"]; } 
            set { ViewState["SubId"] = value; } 
        }

        public int InvoiceId
        {
            get { return (int)ViewState["InvoiceId"]; }
            set { ViewState["InvoiceId"] = value; }
        }

        DataAccess dba = new DataAccess();
        private System.Collections.ArrayList errorList = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ClaimList.Attributes.Add("style", "display: block");
                pnlMismatch.Style["display"] = "none";

                //if (Request.Cookies["userID"] != null)
                //{
                //    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                //}
                if (Request.Cookies["MYLITEROLE"] != null)
                {
                    this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
                }
                if (Request.Cookies["CatID"] != null)
                {
                    this.cat = this.Request.Cookies["CatID"].Value.ToString();
                }
                this.WfId = Convert.ToInt32((string)Request.QueryString["wfid"]);
                
                this.schId = Convert.ToInt32((string)Request.QueryString["sc"]);
                this.userID = Convert.ToInt32((string)Request.QueryString["uid"]);
                this.file = Request.QueryString["fi"];
                this.SubProcessId = 1;
                if (this.file.Substring(this.file.Length-1) == "R") this.SubProcessId = 2;
                this.JobName = Request.QueryString["cl"];
                InitFileType();
                this.FileNo = GetFileNo(WfId);
                this.dllFileType.Attributes.Add("onchange", "chkUpload();");
                this.FileUpload.Attributes.Add("onchange", "chkUpload();");
                btnSubmit.Attributes.Add("onclick", "Spinner();");
                this.descRow.Style.Add("display", "none");
                this.tblInvDet.Style.Add("display", "none");
                lblmsg.Text = "";

                this.txtTotalAmount.Attributes.Add("onblur", "javascript:round_decimals(this, 2)");
                this.txtAmount.Attributes.Add("onblur", "SetGst(this);setSubTotal(this)");
                this.txtHST.Attributes.Add("onblur", "setTotalAmt(this);");
                this.txtHST.Attributes.Add("onfocus", "setFocusG();");
                txtTotalAmount.Enabled = false;
                
            }
            else
            {
                lblmsg.Text = "";
                string ctrlname = Request.Params.Get("__EVENTTARGET");
                if (ctrlname == "btnSubmit") btnUpload_Click();
                if (dllFileType.SelectedItem.ToString() == "Invoice")
                {
                    this.tblInvDet.Style.Add("display", "");
                    txtTotalAmount.Text=Convert.ToString(Convert.ToDouble(txtAmount.Text)+Convert.ToDouble(txtHST.Text));
                }
            }
        }

        public void getFile()
        {
            Response.Write(this.file);
        }
        public void getJobName()
        {
            Response.Write(this.file + "<br>" + this.JobName);
        }
        private int GetProcess(int wfId)
        {
            DataAccess dba = new DataAccess();
            return dba.dxGetIntData("Select ProcessId From wfMast Where WfId =" + wfId);
        }
        private string GetFileNo(int wfId)
        {
            DataAccess dba = new DataAccess();
            return dba.dxGetTextData("Select ProcessRef From wfMast Where WfId =" + wfId);
        }
        public void InitFileType()
        {
            DataTable tblDoc = WorkFlow.BLL.Claim.UploadDoc.GetUploadFileTypes();
            
            DataView dv = tblDoc.DefaultView;
            dv.RowFilter = "ProcessId=22";

            if (!Page.User.IsInRole("Crew - PM"))
            {
                if (SubProcessId == 1) dv.RowFilter += " and FileType not like '%Restoration%'"; else dv.RowFilter += " and FileType not like '%Emergency%'";
                if (this.cat == "20") dv.RowFilter += " and Usedby like '%V%'"; else dv.RowFilter += " and Usedby like '%" + this.LiteROLE + "%'";
            }
            else
            {
                dv.RowFilter += " and (Usedby like '%C%' or usedby like '%P%') and usedby not like '%I%'";
            }

			DataAccess dba = new DataAccess();
            int branchid = dba.dxGetIntData("Select branchid From employees Where userid =" + userID);
            if (branchid ==1734||branchid==1737)
            {
                dv.RowFilter += " and UsedBy like '%I%'";
            }
            dv.Sort="FileType";

            DataRowView dRow = dv.AddNew();
            dRow["FileType"] = "Select A Document";
            dRow["FileId"] = 0;
            dRow["ProcessId"] = 22;
            dRow["Usedby"] = "V";
            dRow.EndEdit();


            dllFileType.DataSource = dv; 
            dllFileType.DataTextField = "FileType";
            dllFileType.DataValueField = "FileId";
            dllFileType.DataBind();
            if (this.cat != "20")
            {
                dllFileType.Items.Add(new ListItem("Other", "6"));
            }
                if (dv.Count < 2) dllFileType.Enabled = false;
            dllFileType.SelectedValue = "0";
        }
        private bool CheckInput()
        {
            bool retVal = false;
            System.Drawing.Color ErrorColor = System.Drawing.Color.FromName("#FF7F7F");
            System.Text.StringBuilder sbError = new System.Text.StringBuilder();

            if ((txtInvoiceNumber.Text.Trim() == string.Empty))
            {
                txtInvoiceNumber.BackColor = ErrorColor;// Color.LavenderBlush;
                retVal = true;
            }
            else
            {
                txtInvoiceNumber.BackColor = System.Drawing.Color.White; 
            }

            if (txtInvoiceDate.Text == string.Empty)
            {
                txtInvoiceDate.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {

                txtInvoiceDate.BackColor = System.Drawing.Color.White; 
            }

            if (txtAmount.Text == string.Empty)
            {
                txtAmount.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {

                txtAmount.BackColor = System.Drawing.Color.White;
            }
            
            if (txtHST.Text == string.Empty)
            {
                txtHST.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {

                txtHST.BackColor = System.Drawing.Color.White;
            }

            if (retVal == true)
            {

                string myStringVariable = "Please Complete Highlighted Fields";
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + myStringVariable + "');", true);
            }
            return retVal;
        }
        protected void btnUpload_Click()
        {
            this.WfId = Convert.ToInt32((string)Request.QueryString["wfid"]);
            this.FileNo = GetFileNo(WfId);
            if (dllFileType.SelectedItem.ToString() == "Invoice")
            {
                if (CheckInput()) return;
            }
            if (dllFileType.SelectedItem.ToString() == "Other")
            {
                if (txtDesc.Text.Trim() == "") lblmsg.Text = "Description is required for document type of 'Other'";
                this.descRow.Style.Add("display", "");
            }

            if (this.FileUpload.PostedFile.FileName.Trim().Equals(""))
            {
                lblmsg.Text = "Please select 'Choose Files' before clicking Upload Files.";
                lblmsg.Visible = true;
                return;
            }

            int lastPos = FileUpload.PostedFile.FileName.Trim().LastIndexOf(@".");
            string suffix = FileUpload.PostedFile.FileName.Trim().Substring(lastPos);

            if (suffix.ToLower() == ".pdf" || suffix.ToLower() == ".jpg" || suffix.ToLower() == ".jpeg")
            {
                lblmsg.Text = "";
            }
            else
            {
                lblmsg.Text = "Please upload only PDF or images";
                lblmsg.Visible = true;
                return;
            }
            if (lblmsg.Text == "")
            {
                HttpFileCollection uploadedFiles = Request.Files;
                string errorList = "";
                DataAccess dba = new DataAccess();
                for (int i = 0; i < uploadedFiles.Count; i++)
                {
                    HttpPostedFile file = uploadedFiles[i];

                    try
                    {
                        if (file.ContentLength > 0)
                        {
                            
                            if (!file.FileName.Trim().Equals(""))
                            {
                                string descriptor = this.txtDesc.Text;
                                if (descriptor == "") descriptor = this.dllFileType.SelectedItem.ToString();
                                int ftyp = Convert.ToInt32(this.dllFileType.SelectedValue);
                                errorList += this.ProcessFile(ftyp, descriptor, file);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        //Span1.Text += "Error: <br>" + Ex.Message;
                    }
                }
                if (errorList == "")
                {
                    if (uploadedFiles.Count > 1)
                    {
                        
                        string strDBfilePath = dba.dxGetSPString("wfGetFilesRootPath");
                        string File;

                        string filePath;
                        filePath = strDBfilePath;
                        if (WfId > 0)
                        {
                            filePath += WfId + @"\";
                        }

                        for (int i = 1; i < uploadedFiles.Count; i++)
                        {
                            HttpPostedFile file = uploadedFiles[i];
                            string File1 = filePath + SetFileName(file);

                            File = filePath + SetFileName(uploadedFiles[0]);

                            MergeFile(File, File1);

                            int filekeyId = dba.dxGetIntData("select top 1 keyId from StrFilesDet where wfid=" + this.WfId + " and filename = '" + SetFileName(file) + "'");
                            if (filekeyId > 0)
                            {
                                SmartflowLite.UploadDoc doc = new SmartflowLite.UploadDoc(filekeyId);
                                string message = "";
                                int delCount = doc.DeleteFile(ref message);
                            }
                        }
                        File = filePath + SetFileName(uploadedFiles[0]);
                        System.IO.File.Delete(File.Replace(".pdf",".jpg"));
                    }
                    HttpPostedFile firstfile = uploadedFiles[0];

                    string fileonly = firstfile.FileName.Trim().Substring(firstfile.FileName.Trim().LastIndexOf("\\") + 1);
                    int OAid = dba.dxGetIntData("select projmanagerid from claimvalues where wfid=" + this.WfId.ToString() + " and subprocessid=" + SubProcessId.ToString());
                    if (dllFileType.SelectedItem.ToString() == "Inspection Report/Estimate") createGeneralTask(OAid);

                    if (dllFileType.SelectedItem.ToString() == "Customer Authorization (Emergency)")
                    {
                        Smartflow.BLL.Claim.PreliminaryReportsf pr = new Smartflow.BLL.Claim.PreliminaryReportsf(9999);
                        pr.CreateTasks(WfId, SubProcessId, 22435, userID);
                        int KeyId = dba.dxGetIntData("select top 1 keyId from StrFilesDet where wfid=" + this.WfId + " and filename = '" + fileonly + "'");
                        dba.dxExecuteNonQuery("update wfdet set sindexid=" + KeyId + " where actionid=22436 and wfid=" + this.WfId.ToString() + " and subprocessid=" + SubProcessId.ToString());
                    }
                    
                    if (dllFileType.SelectedItem.ToString().Contains("Scope Document"))
                    {
                        Smartflow.BLL.Claim.PreliminaryReportsf pr = new Smartflow.BLL.Claim.PreliminaryReportsf(9999);

                        if (dllFileType.SelectedItem.ToString().Contains("Emergency"))
                        {
                            pr.CreateTasks(WfId, 1, 22395, userID);
                        }
                        else
                        {
                            pr.CreateTasks(WfId, 2, 22395, userID);
                        }

                    }
                    int keyId = dba.dxGetIntData("select top 1 keyId from StrFilesDet where wfid=" + this.WfId + " and filename = '" + fileonly + "'");
                    if (keyId < 0)
                    {
                        lastPos = fileonly.LastIndexOf(@".");
                        suffix = fileonly.Substring(lastPos);
                        fileonly = fileonly.Replace(suffix, ".pdf");
                        keyId = dba.dxGetIntData("select top 1 keyId from StrFilesDet where wfid=" + this.WfId + " and filename = '" + fileonly + "'");
                    }
                    //this.FileId = keyId;
                    dba.dxExecuteNonQuery("update StrFilesDet set refId=" + this.schId + " where keyId=" + FileId.ToString());

                    if (dllFileType.SelectedItem.ToString().Contains("Invoice"))
                    {
                        dba.dxExecuteNonQuery("update StrFilesDet set FileId=20, Comments='AP Invoice' where keyId=" + FileId.ToString());
                        Save();
                    }
                    
                    string myStringVariable = "File upload and conversion succesfull";
                    ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + myStringVariable + "');", true);
                    //Response.Redirect("Claims.aspx");
                }
                else
                {
                    lblmsg.Text = errorList;
                }
            }
        }
        private string Save()
        {

            string strDefCulZone = Global.DefCulZone();
            string message = "";
            bool HasInvNo = false;
            int VendorId =0;
            DataAccess dba = new DataAccess();

            VendorId=dba.dxGetIntData("select accountid from wfusers where userid=" + userID);

            HasInvNo = WorkFlow.BLL.Invoice.APInvoice.HasInvoiceNo(this.txtInvoiceNumber.Text.Trim(), VendorId);

            DateTime invDate = DateTime.Parse(this.txtInvoiceDate.Text);

            if (HasInvNo)
            {
                message = "This invoice number already exists.";
                WorkFlow.Objects.BLL.ClientScript.SetFocus(this.txtInvoiceNumber, this);
                lblmsg.Text = message;
                return message;
            }
                
            dba.dxAddParameter("@schid", schId);    
            int POId = dba.dxGetIntData("Select poid from PurchaseOrders where schId = @schid");
            string vendorName = WorkFlow.Objects.BLL.Vendors.GetVendorName(VendorId);

            message = GenerateInvoiceAndTask(POId);

            if (message == string.Empty)
            {
                dba.dxClearParameters();

                dba.dxAddParameter("@InvoiceId", InvoiceId);
                dba.dxAddParameter("@VendorId", VendorId);
                dba.dxAddParameter("@UserId", 0);
                dba.dxAddParameter("@AbbrProvided", "0");
                dba.dxAddParameter("@Desc", " ");
                dba.dxAddParameter("@FileNumberProvidedCorrect", "0");

                dba.dxExecuteSP("InsertInvoiceExtendedData");
            }
            else
            {
                return message;
            }

            return message;

        }

        public string GenerateInvoiceAndTask(int POId)
        {
            try
            {
                string strDefCulZone = Global.DefCulZone();

                DataAccess dba = new DataAccess();
                using (SqlConnection conn = dba.dxConnection)
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand command = conn.CreateCommand();
                    command.Transaction = trans;

                    string message = string.Empty;
                    int claimId = 0;

                    claimId = dba.dxGetIntData("select projid from schedule where schid=" + schId);
                    SubProcessId = dba.dxGetIntData("select subprocessid from schedule where schid=" + schId);

                    ////string claimNo = this.ddlProject.SelectedItem.Text.Trim();

                    SmartFlowLite.APInvoice inv = null;

                    // Task variables
                    int nextActionID = 22854;  //task no this goes to
                    int assignedAccountId = 0;  //branch id
                    int assignedUserId = 0;      //user id

                    if ((this.SubProcessId == 1) || (this.SubProcessId == 2))
                    {
                        assignedUserId = WorkFlow.BLL.Invoice.APInvoice.GetPMforClaims(this.WfId, this.SubProcessId);
                        assignedAccountId = Claim.GetBranchID(this.WfId, SubProcessId);
                    }



                    Doxess.Web.WorkFlow.Process.Process dxProcess = new Doxess.Web.WorkFlow.Process.Process();

                    dba.dxAddParameter("@POId", POId);
                    int VendorId = dba.dxGetIntData("Select VendorId from PurchaseOrders where POId = @POId");
                    int CostCodeId = dba.dxGetIntData("Select defcostcode from vendors where vendorid ="+ VendorId);

                    // Add new invoice record
                    inv = new SmartFlowLite.APInvoice();
                    inv.InvoiceDate = DateTime.Parse(this.txtInvoiceDate.Text);
                    inv.Amount = Math.Round(Convert.ToDecimal(this.txtAmount.Text), 2);
                    inv.GSTAmount = Math.Round(Convert.ToDecimal(this.txtHST.Text), 2);

                    //inv.setProvincialTaxField(ref inv, SelectedVendor, SelectedBranch, Math.Round(Convert.ToDecimal(this.txtPSTAmt.Text), 2));

                    inv.TotalAmount = Math.Round((inv.Amount + inv.GSTAmount + inv.PSTAmount + inv.QSTAmount), 2);
                    inv.ClaimId = claimId;
                    inv.InvoiceNo = this.txtInvoiceNumber.Text.Trim();
                    inv.PONo = POId;
                    inv.VendorId = VendorId;
                    inv.VendorName = WorkFlow.Objects.BLL.Vendors.GetVendorName(VendorId);
                    inv.SubProcess = this.SubProcessId;
                    inv.FileId = this.FileId;
                    inv.CostCodeId = CostCodeId;
                    inv.History = false;
                    inv.BatchId = WorkFlow.BLL.Invoice.APInvoice.GetBatchId();
                    inv.AdminOrClaims = 1;
                    inv.ProcessId = 22;

                    int invoiceId = inv.AddNewWithProcessId(ref message, command, userID);

                    InvoiceId = invoiceId;

                    if (!message.Equals(string.Empty))
                    {
                        //delete the uplaoded file
                        WorkFlow.BLL.Claim.UploadDoc doc = new UploadDoc(this.FileId);
                        doc.DeleteFile(ref message);

                        //delete attached PO
                        WorkFlow.BLL.JobCost.PoData.DeletePO(POId);

                        //this.lblMessage.Text = message;
                        trans.Rollback();
                        return message;
                    }


                    string TaskDesc = "Approve Vendor Cost - " + inv.VendorName + " No. " + inv.InvoiceNo;
                     int DetId = dxProcess.wfTaskCreate(ref message, command, this.WfId, nextActionID, -1, assignedAccountId, assignedUserId, TaskDesc, "", "", InvoiceId, this.FileId, this.SubProcessId);
                    

                    if (!message.Equals(string.Empty) || DetId < 0)
                    {
                        WorkFlow.BLL.JobCost.PoData.DeletePO(POId);
                        WorkFlow.BLL.Invoice.APInvoice.DeleteIncomingInvoice(command, InvoiceId, this.WfId, this.SubProcessId);
                        WorkFlow.BLL.Claim.UploadDoc doc = new UploadDoc(this.FileId);
                        doc.DeleteFile(ref message);
                        //this.lblMessage.Text = message;
                        trans.Rollback();
                        return message;
                    }
                    else
                    {
                        if (cat == "20")
                        {
                            this.userName = dba.dxGetTextData("select resname from schresources where resindexid=(select accountid from wfusers where userid=" + this.userID.ToString() + ") and rescatid=20");
                        }
                        else
                        {
                            this.userName = TimebookCom.WeeklyTimebook.GetUserNameById(userID);
                        }
                        Notes.AddNewNote(this.WfId, "A", 0, DateTime.Now.ToString(), userID, "Invoice " + InvoiceId + " created by " + userName, this.SubProcessId);

                    }

                    dba.dxAddParameter("@UserId", assignedUserId);
                    int assignedToRoleId = 0;// = 211;
                    DataTable dtRoles = dba.dxGetTable("Select RoleId from wfUsersRoles where UserId = @UserId order by RoleId");
                    if (dtRoles.Rows.Count > 0)
                    {
                        for (int count = 0; count < dtRoles.Rows.Count; count++)
                        {
                            if (Convert.ToInt32(dtRoles.Rows[count]["RoleId"].ToString()) == 210)
                            {
                                assignedToRoleId = 210;
                                break;
                            }
                            else if (Convert.ToInt32(dtRoles.Rows[count]["RoleId"].ToString()) == 211)
                            {
                                assignedToRoleId = 211;
                                break;
                            }
                        }
                    }


                    if (!message.Equals(string.Empty) || DetId < 0)
                    {
                        WorkFlow.BLL.JobCost.PoData.DeletePO(POId);
                        WorkFlow.BLL.Invoice.APInvoice.DeleteIncomingInvoice(command, InvoiceId, this.WfId, this.SubProcessId);
                        WorkFlow.BLL.Claim.UploadDoc doc = new UploadDoc(this.FileId);
                        doc.DeleteFile(ref message);
                        //this.lblMessage.Text = message;
                        trans.Rollback();
                        return message;
                    }
                    else
                    {

                        message = dba.dxUpdate(command, "wfDet", "AssignedToRoleId", "DetId", assignedToRoleId, DetId);
                        message = dba.dxUpdate(command, "wfDetR2G", "AssignedToRoleId", "DetId", assignedToRoleId, DetId);
                        dba.dxUpdate(command, "PurchaseOrders", "POStatusId", "POId", 2, inv.PONo); //update purchase order status

                    }


                    trans.Commit();
                    SetExpiryDate(DetId);
                    return "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                
                WorkFlow.BLL.Claim.UploadDoc doc = new UploadDoc(this.FileId);
                doc.DeleteFile(ref message);
                return message;
            }

        }

        protected void SetExpiryDate(int DetId)
        {
            DateTime now = DateTime.Today;
            int Weekday = Convert.ToInt32(now.DayOfWeek);

            if (Weekday == 0)
            {
                now = now.AddDays(3);
            }
            else if (Weekday == 1)
            {
                now = now.AddDays(2);
            }
            else if (Weekday == 2)
            {
                now = now.AddDays(1);
            }
            else if (Weekday == 3)
            {
                now = now.AddDays(1);
            }
            else if (Weekday == 4)
            {
                now = now.AddDays(6);
            }
            else if (Weekday == 5)
            {
                now = now.AddDays(5);
            }
            else if (Weekday == 6)
            {
                now = now.AddDays(4);
            }
            string returndate = now.ToShortDateString() + " 12:00:00 AM";
            WorkFlow.Objects.BLL.TaskHelper.resetOverdueDate(DetId, Convert.ToDateTime(returndate));
        }
        private string ProcessFile(int fileType, string fileDesc, HttpPostedFile file)
        {
            
            SmartflowLite.UploadDoc doc = new SmartflowLite.UploadDoc(FileNo);
            if (file.FileName.Trim().Equals("")) return "";
            int count = doc.SaveFile(file, fileType, fileDesc, this.userID, true);
            this.FileId = doc.FileId;
            if (count == 0) return "";
            if (count == -1)
            {
                return "Error in saving File " + doc.FileName + ".";
            }
            else if (count == -3)
            {
                return "Error in PDF Conversion of File " + doc.FileName + " ";
            }
            else
            {
                return "Error in saving File Information of File " + doc.FileName + " ";
            }
        }
        private string SetFileName(System.Web.HttpPostedFile UploadedFile)
        {
            string FileName = UploadedFile.FileName.Replace(" ", "").Replace("#", "").Replace("&", "");
            FileName = GetFileName(FileName);
            string[] tokens = FileName.Split(".".ToCharArray());
            if (tokens.Length > 1)
            {
                string ext = tokens[tokens.Length - 1];
                FileName = FileName.Replace(".", "");
                int len = FileName.Length - ext.Length;
                FileName = FileName.Substring(0, len) + ".pdf";
            }
            else
            {
                FileName=FileName.Replace(".jpg",".pdf");
            }
            return FileName;
        }

        public static string GetFileName(string fileName)
        {
            fileName = fileName.Replace("/", @"\");
            int lastPos = fileName.LastIndexOf(@"\");
            return fileName.Substring(lastPos + 1);
        }
        public void MergeFile(string doc1, string merge)
        {
            DataAccess dba = new DataAccess();
            string doc2 = merge;
            PdfDocument outputDocument = PdfReader.Open(doc1);
            long flength = new System.IO.FileInfo(doc2).Length;
            if (flength > 0)
            {
                PdfDocument inputDocument = PdfReader.Open(doc2, PdfDocumentOpenMode.Import);
                int count = inputDocument.PageCount;    // Iterate pages 
                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }
            outputDocument.Save(doc1);
        }
        private void createGeneralTask(int OAid)
        {
            DataAccess dba = new DataAccess();
            SqlConnection conn = null;
            SqlCommand command = null;
            SqlTransaction trans = null;
            int wfDetId = 0;
            int gtId = 0;
            string retmessage = "";
            string report = dllFileType.SelectedItem.ToString();
            if (report == "Other") report = txtDesc.Text; 
            int branchid = Claim.GetBranchID(WfId, SubProcessId);
            using (conn = dba.dxConnection)
            {
                conn.Open();
                trans = conn.BeginTransaction();
                command = conn.CreateCommand();
                command.Transaction = trans;
                Doxess.Web.WorkFlow.Process.Process dxProcess = new Doxess.Web.WorkFlow.Process.Process();
                wfDetId = dxProcess.wfTaskCreate(ref retmessage, command, WfId, 25099, -1, branchid, OAid, "Generic task - " + report, "Generic task", "", -1, -1, SubProcessId);
                if (wfDetId > 0)
                {
                    dba.dxAddParameter("@wfDetId", wfDetId);
                    dba.dxAddParameter("@Details", "Report '" + report + "' has been uploaded - please review");
                    dba.dxAddParameter("@Response", "");
                    dba.dxAddParameter("@IindexId", 0);
                    dba.dxAddParameter("@id", SqlDbType.Int);
                    gtId = dba.dxExecuteSP("AddGeneralTask");
                    if (gtId < 1) retmessage = "error adding general task";
                }
                if ((gtId > 0) && (retmessage.Equals("")))
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                    this.lblmsg.Text = retmessage;
                    return;
                }
            }
        }

    }
}