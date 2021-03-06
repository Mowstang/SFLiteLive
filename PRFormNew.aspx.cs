﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Doxess.Web.WorkFlow.Security;
using Doxess.Web.WorkFlow.Process;
using Doxess.Web.WorkFlow;
using WorkFlow.Claims.Actions;
using WorkFlow.BLL;
using WorkFlow.BLL.Email;
using WorkFlow.BLL.Claim;
using WorkFlow.BLL.ClaimCombined;
using Doxess.Data;
using System.Web.Mail;
using System.IO;
using System.Globalization;
using WorkFlow.Objects.BLL;
using WorkFlow.BLL.XactAnalysis;

namespace SFLite
{
    public partial class PRFormNew : System.Web.UI.Page
    {

        
        private bool isEmergency
        {
            get { return (bool)ViewState["Emergency"]; }
            set { ViewState["Emergency"] = value; }
        }

        private int PRCompletion
        {
            get { return (int)ViewState["PRCompletion"]; }
            set { ViewState["PRCompletion"] = value; }
        }

        private int SubProcessId
        {
            get { return (int)ViewState["subprocess"]; }
            set { ViewState["subprocess"] = value; }
        }

        private int PictureUploadCompletion
        {
            get { return (int)ViewState["PictureUploadCompletion"]; }
            set { ViewState["PictureUploadCompletion"] = value; }
        }

        private int ScheduleInspectionCompletion
        {
            get { return (int)ViewState["ScheduleInspectionCompletion"]; }
            set { ViewState["ScheduleInspectionCompletion"] = value; }
        }

        private int ScheduleAppointmentCompletion
        {
            get { return (int)ViewState["ScheduleAppointmentCompletion"]; }
            set { ViewState["ScheduleAppointmentCompletion"] = value; }
        }
        //private void SetEmergency()
        //{
        //    int subprocId = this.GetIntCookie("SubProcessId");
        //    if (subprocId == 1)
        //    {
        //        this.isEmergency = true;
        //    }
        //    else if (subprocId == 2)
        //    {
        //        this.isEmergency = false;
        //    }
        //}

        private int userId
        {
            get {return (int)ViewState["userId"];}
            set {ViewState["userId"] = value;}
        }

        private int detId
        {
            get {return (int)ViewState["detId"];}
            set {ViewState["detId"] = value;}
        }

        private int ClaimID
        {
            get{return (int)ViewState["claimid"];}
            set{ViewState["claimid"] = value;}
        }

        private int WfID
        {
            get{return (int)ViewState["wfid"];}
            set{ViewState["wfid"] = value;}
        }

        private int ClaimPrelimReportID
        {
            get {return (int)ViewState["cprid"];}
            set {ViewState["cprid"] = value;}
        }

        private int BranchId
        {
            get { return (int)ViewState["BranchId"]; }
            set { ViewState["BranchId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.Cookies["userID"] != null)
                {
                    userId = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }

                this.InitPage();

                this.txtEmergencyReserveAmount.Attributes.Add("onkeyup", "NoDecimal(event)");
                this.txtEmergencyReserveAmount.Attributes.Add("onkeydown", "NoDecimal(event)");
                this.txtEmergencyReserveAmount.Attributes.Add("onblur", "formatCurrency(this)");

                this.txtContentBudgetReserve.Attributes.Add("onkeyup", "NoDecimal(event)");
                this.txtContentBudgetReserve.Attributes.Add("onkeydown", "NoDecimal(event)");
                this.txtContentBudgetReserve.Attributes.Add("onblur", "formatCurrency(this)");

                this.txtRestorationReserveAmount.Attributes.Add("onkeyup", "NoDecimal(event)");
                this.txtRestorationReserveAmount.Attributes.Add("onkeydown", "NoDecimal(event)");
                this.txtRestorationReserveAmount.Attributes.Add("onblur", "formatCurrency(this)");

               
            }
            //btnsub.Click += new EventHandler(btnsubmit_Click);
            btnret.Click += new EventHandler(btnret_Click);

        }
        private void LoadEmployee(int BranchId)
        {
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@BranchId", BranchId);
            DataTable Employee = dba.dxGetSPData("LoadEmployeeListByRole");

            DataRow dr1 = Employee.NewRow();
            dr1["EmpName"] = " ";
            dr1["EmpId"] = 0;
            dr1["RoleId"] = 211;
            dr1["AccountId"] = 0;
            Employee.Rows.InsertAt(dr1, 0);
            int pmid = dba.dxGetIntData("Select ProjManagerId from ClaimValues where ClaimId = " + this.ClaimID + "and WfId = " + this.WfID);
            string pmname = dba.dxGetTextData("Select FirstName + ' ' + LastName from Employees where userid = " + pmid);

            DataRow dr2 = Employee.NewRow(); dr1["EmpName"] = pmname;
            dr2["EmpId"] = pmid;
            dr2["RoleId"] = 211;
            dr2["AccountId"] = 0;
            Employee.Rows.InsertAt(dr2, 1);


            DataView dv = Employee.DefaultView;

            dv.RowFilter = "RoleId = " + 211;
            ddlPM.DataSource = dv.Sort = "EmpName";
            ddlPM.DataSource = Employee.DefaultView.ToTable(true, "EmpName", "EmpId");

            if (BranchId == 0)
            {
                dv.RowFilter = "AccountId <> " + BranchId + "and RoleId = 211";
            }

            
            ddlPM.DataTextField = "EmpName";
            ddlPM.DataValueField = "EmpId";
            ddlPM.DataBind();

            dba.dxClearParameters();

            try
            {
                int PMID = dba.dxGetIntData("Select ProjManagerId from ClaimValues where ClaimId = " + this.ClaimID + "and WfId = " + this.WfID);
                ddlPM.SelectedValue = PMID.ToString();
            }
            catch
            {
                ddlPM.SelectedIndex = 0;
            }
        }

        private void InitMultilineTetBoxs()
        {
            txtProperty.Attributes.Add("onblur", "CheckLength(this, 1000);");
        }

        private void InitPage()
        {
            DataAccess dba = new DataAccess();
            
            WfID = Convert.ToInt32((string)Request.QueryString["wfid"]);
            ClaimID = Claim.GetClaimIdByWfId(WfID);

            string file = Request.QueryString["fi"]; 
            lblTitle.Text = file;
            lblJobName.Text = Request.QueryString["cl"];



            SetParameters();
            
            PreliminaryReportcs pr = new PreliminaryReportcs(this.detId);
            //this.ClaimID = pr.ClaimID;
            //this.WfID = pr.WfID;

            InitMultilineTetBoxs();

            string TOL = "";
            Equip.Visible = false;

            if (SubProcessId == 1)
            {
                this.isEmergency = true;
                DataTable claimData = WorkFlow.Objects.Process.ClaimDetail.wfGetClaimDetail_Task(WfID, SubProcessId);
                if (claimData.Rows.Count > 0)
                {
                    if (claimData.Rows[0].ItemArray[4] != DBNull.Value)
                    {
                        TOL = (string)claimData.Rows[0].ItemArray[4];  //  r001404="Type of loss: "
                    }
                }
                if (TOL.ToLower() == "water")
                {
                    Equip.Visible = true;
                }

            }
            else
            {
                this.isEmergency = false;
                Equip.Visible = false;
                //trEmergInfo.Visible = false;

                string display = "none";
                this.divEmergInfo.Style.Add("display", display); 

            }

            if (Claim.GetPrincipalCarrierId(ClaimID) == 52)
            {
                trRestorationComments.Visible = false;
                Equip.Visible = false;
                Literal7.Text = "Other Comments";
                lblPropertyStructure.Text = "Other Comments";
            }

            bool IsTaskComplete = Process.wfCanPostAction(this.detId,userId);
            this.btnsub.Visible = IsTaskComplete;

            BranchId = pr.BranchID;

            txtProperty.Text = pr.Property;

            BranchId = Claim.GetBranchID(WfID, SubProcessId);
            LoadEmployee(BranchId);

            if (pr.InspectionBy == null)
            {
                ddlPM.SelectedValue = userId.ToString();
            }
            else
            {
                string CompletedByUserid = dba.dxGetTextData("select userid from wfusers where name='" + pr.InspectionBy + "'");
                ddlPM.SelectedValue = CompletedByUserid;
            }

            
            txtRestoration.Text = pr.Restoration;
            txtResponded.Text = pr.Responded;
            txtDamageLocation.Text = pr.dmLoc;
            txtItemsOfHighValue.Text = pr.IHV;
            txtPreExistingDamage.Text = pr.DPCTB;
            txtRequiredRepairs.Text = pr.DRR;
            ddlDryingStrategy.SelectedValue = pr.dryst;

            ddlTearOutRequired.SelectedValue = pr.toreq;


            txtAgeOfProperty.Text = pr.AgeOfProperty;
            txtSquareFootAffectedAreas.Text = pr.SFAA;
            txtSFOfBuilding.Text = pr.SFOB;
            txtNumRoomsAffected.Text = pr.RoomAffected;
            ddlWaterType.SelectedValue = pr.wattype;
            ddlFireType.SelectedValue = pr.firetype;

            //OthCom.Text = pr.OthCom;
            
            txtResponded.Text = pr.Responded;
            txtConditionOfBuilding.Text = pr.COBuild;
            ddlTypeOfBuilding.SelectedValue = pr.ddlTOBuild;
            ddlQualityOfFinish.SelectedValue = pr.qualoffinish;

            rbtnAdjusterNeeded.SelectedValue = Convert.ToString(Convert.ToInt32(pr.AdjusterNeeded));
            rbtnBuildingDamage.SelectedValue = Convert.ToString(Convert.ToInt32(pr.BuildingDamage));
            //rbtnPhotosTaken.SelectedIndex = Convert.ToInt32(pr.PhotosTaken);
            rbtnEmergencyService.SelectedValue = Convert.ToString(Convert.ToInt32(pr.EmergencyService));
            rbtnContentDamage.SelectedValue = Convert.ToString(Convert.ToInt32(pr.ContentDamage));
            rbtnVacatePremises.SelectedValue = Convert.ToString(Convert.ToInt32(pr.vacateprem));
            rbtnHazardousMaterials.SelectedValue = Convert.ToString(Convert.ToInt32(pr.amhazm));
            rbtnKitchenAffected.SelectedValue = Convert.ToString(Convert.ToInt32(pr.kitaffect));
            rbtnBathRoomAffected.SelectedValue = Convert.ToString(Convert.ToInt32(pr.bathaffect));

            if (IsTaskComplete)
            {
                txtecompdate.Visible = false;
                txtEmergCompletionDate.Visible = true;
            }
            else
            {
                txtEmergCompletionDate.Visible = false;
                txtecompdate.Text = Convert.ToDateTime(pr.EmergCompDate).ToShortDateString();
                txtecompdate.Visible = true;
            }


            int TaskStatus = dba.dxGetIntData("Select ActionStatusId from wfDet where DetId = " + this.detId);

            if (pr.EmergReserve != -99)
                txtEmergencyReserveAmount.Text = pr.EmergReserve.ToString("###,##0");
            else
                txtEmergencyReserveAmount.Text = "";

            if (pr.RestReserve != -99)
                txtRestorationReserveAmount.Text = pr.RestReserve.ToString("###,##0");
            else
                txtRestorationReserveAmount.Text = "";

            if (pr.conbudget != -99)
                txtContentBudgetReserve.Text = pr.conbudget.ToString("###,##0");
            else
                txtContentBudgetReserve.Text = "";

            int subProcessId = Claim.GetSubProcessIdByDetId(this.detId);
            //this.txtInspectionBy.Text = Claim.getProjectManagerNameByClaimId(this.ClaimID, subProcessId);

            Process process = new Process();

            //DataTable detInfo = process.wfTaskDetailinfo(this.detId);
            //if (detInfo.Rows.Count > 0)
            //{
            //    DataRow row = detInfo.Rows[0];

            //    this.SetEmergency();
            //    //this.LitTask.Text  += this.Request.Cookies["taskId"].Value  +  " - "   +  row["Desc"].ToString().Trim() ; 

            //}

            dba.dxClearParameters();
            int PMId = dba.dxGetIntData("select Isnull(ProjManagerId, 0) as ProjManagerId from claimValues where wfId=" + this.WfID + " and SubProcessId = " + subProcessId);// this.GetIntCookie("SubProcessId"));



            hidBlnCreateRFile.Value = dba.dxGetIntData("SELECT COUNT(ClaimValueId) FROM ClaimValues WHERE wfId=" + WfID + " AND  SubProcessId = 2") == 0 ? "" : "false";
            hidBlnCloseFile.Value = isEmergency ? "" : "false";

            if (SubProcessId == 1)
            {
                DataTable MoistureReadings = WorkFlow.BLL.Claim.Claim.GetMoistureReadings(this.WfID);
                if (MoistureReadings.Rows.Count > 0)
                {
                    DataRow Row = MoistureReadings.Rows[0];
                    txtAffectedTemp.Text = (Row["AffectedTemp"].ToString());
                    txtAffectedRH.Text = (Row["AffectedRH"].ToString());
                    txtAffectedGPP.Text = (Row["AffectedGPP"].ToString());
                    txtNonAffectedTemp.Text = (Row["NonAffectedTemp"].ToString());
                    txtNonAffectedRH.Text = (Row["NonAffectedRH"].ToString());
                    txtNonAffectedGPP.Text = (Row["NonAffectedGPP"].ToString());
                    txtExternalTemp.Text = (Row["ExternalTemp"].ToString());
                    txtExternalRH.Text = (Row["ExternalRH"].ToString());
                    txtExternalGPP.Text = (Row["ExternalGPP"].ToString());
                    txtFans.Text = (Row["Fans"].ToString());
                    txtDehus.Text = (Row["Dehus"].ToString());
                    DateTime dt;
                    if (pr.EmergCompDate != null) dt = DateTime.Parse(pr.EmergCompDate.ToString());
                    else dt = DateTime.Now;
                    txtEmergCompletionDate.Text = dt.ToString();
                }
            }
        }


        private void SetParameters()
        {
            DataAccess dba = new DataAccess();

            this.detId = dba.dxGetIntData("Select detid from wfdet where actionid=22120 and wfid=" + WfID);

            string file = Request.QueryString["fi"]; 
            if (file.Substring(file.Length - 1) == "R")
            {
                SubProcessId = 2;
            }
            else
            {
                SubProcessId = 1;
            }
        }




        

        private bool CheckInputWritingPR()
        {
            bool retVal = false;
            Color ErrorColor = System.Drawing.Color.FromName("#FF7F7F");
            string EmergencyReserveAmount = txtEmergencyReserveAmount.Text.Trim();
            string RestorationReserveAmount = txtRestorationReserveAmount.Text.Trim();
            string Property = txtProperty.Text.Trim();
            //string Damage = txtDamage.Text.Trim();
            string Restoration = txtRestoration.Text.Trim();
            string Responded = txtResponded.Text.Trim();
            string Inspection = ddlPM.SelectedItem.Text.Trim();
            double Num;
            bool isNum = double.TryParse(EmergencyReserveAmount, out Num);
            System.Text.StringBuilder sbError = new System.Text.StringBuilder();

            if ((EmergencyReserveAmount == string.Empty) || (Num < 0))
            {
                txtEmergencyReserveAmount.BackColor = ErrorColor;// Color.LavenderBlush;
                retVal = true;
            }
            else
            {
                txtEmergencyReserveAmount.BackColor = Color.White; ;
            }

            isNum = double.TryParse(RestorationReserveAmount, out Num);
            if ((RestorationReserveAmount == string.Empty) || (Num < 0))
            {
                txtRestorationReserveAmount.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {
                txtRestorationReserveAmount.BackColor = Color.White;
            }
            isNum = double.TryParse(txtContentBudgetReserve.Text.Trim(), out Num);
            if ((txtContentBudgetReserve.Text == string.Empty) || (Num < 0))
            {
                retVal = true;
                txtContentBudgetReserve.BackColor = ErrorColor;
            }
            else
            {
                txtContentBudgetReserve.BackColor = Color.White;
            }
            if (Property == string.Empty)
            {
                txtProperty.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {
                txtProperty.BackColor = Color.White;
            }
            if (trRestorationComments.Visible == true)
            {
                if (Restoration == string.Empty)
                {
                    txtRestoration.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtRestoration.BackColor = Color.White;
                }
            }
            if (Responded == string.Empty)
            {
                txtResponded.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {
                txtResponded.BackColor = Color.White;
            }
            //if (Inspection == string.Empty)
            //{
            //    txtInspectionBy.BackColor = ErrorColor;
            //    retVal = true;
            //}
            //else
            //{
            //    txtInspectionBy.BackColor = Color.White;
            //}
            if ((rbtnAdjusterNeeded.SelectedIndex != 0) && (rbtnAdjusterNeeded.SelectedIndex != 1))
            {

                retVal = true;
            }
            if ((rbtnBuildingDamage.SelectedIndex != 0) && (rbtnBuildingDamage.SelectedIndex != 1))
            {
                retVal = true;
            }
            if ((rbtnContentDamage.SelectedIndex != 0) && (rbtnContentDamage.SelectedIndex != 1))
            {
                retVal = true;
            }
            if ((rbtnEmergencyService.SelectedIndex != 0) && (rbtnEmergencyService.SelectedIndex != 1))
            {
                retVal = true;
            }

            if (divEmergInfo.Style.Value == "display:block")
            {
                if (this.txtEmergCompletionDate.Text == string.Empty)
                {
                    txtEmergCompletionDate.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {

                    if (DateHelper.IsWeekend(Convert.ToDateTime(this.txtEmergCompletionDate.Text), WfID, Claim.GetSubProcessIdByDetId(this.detId)))
                    {
                        retVal = true;
                    }
                }

                
                
                
            }
            if (txtAgeOfProperty.Text == "")
            {
                txtAgeOfProperty.BackColor = ErrorColor;
                retVal = true;
            }
            else
            {
                txtAgeOfProperty.BackColor = Color.White;
            }

            if (txtSFOfBuilding.Text == "")
            {
                retVal = true;
                txtSFOfBuilding.BackColor = ErrorColor;
            }
            else
            {
                txtSFOfBuilding.BackColor = Color.White;
            }
            if (txtConditionOfBuilding.Text == "")
            {
                retVal = true;
                txtConditionOfBuilding.BackColor = ErrorColor;
            }
            else
            {
                txtConditionOfBuilding.BackColor = Color.White;
            }
            if (ddlQualityOfFinish.SelectedValue == "")
            {
                retVal = true;
                ddlQualityOfFinish.BackColor = ErrorColor;
            }
            else
            {
                ddlQualityOfFinish.BackColor = Color.White;
            }
            if (ddlTypeOfBuilding.SelectedValue == "")
            {

                retVal = true;
                ddlTypeOfBuilding.BackColor = ErrorColor;
            }
            else
            {
                ddlTypeOfBuilding.BackColor = Color.White;
            }
            if (ddlWaterType.SelectedValue == "")
            {
                retVal = true;
                ddlWaterType.BackColor = ErrorColor;
            }
            else
            {
                ddlWaterType.BackColor = Color.White;
            }
            if (ddlFireType.SelectedValue == "")
            {
                retVal = true;
                ddlFireType.BackColor = ErrorColor;
            }
            else
            {
                ddlFireType.BackColor = Color.White;
            }
            if (ddlDryingStrategy.SelectedValue == "")
            {
                retVal = true;
                ddlDryingStrategy.BackColor = ErrorColor;
            }
            else
            {
                ddlDryingStrategy.BackColor = Color.White;
            }
            if (ddlTearOutRequired.SelectedValue == "")
            {
                retVal = true; ;
                ddlTearOutRequired.BackColor = ErrorColor;
            }
            else
            {
                ddlTearOutRequired.BackColor = Color.White;
            }
            if (txtDamageLocation.Text == "")
            {
                retVal = true;
                txtDamageLocation.BackColor = ErrorColor;
            }
            else
            {
                txtDamageLocation.BackColor = Color.White;
            }
            if (txtSquareFootAffectedAreas.Text == "")
            {
                retVal = true;
                txtSquareFootAffectedAreas.BackColor = ErrorColor;
            }
            else
            {
                txtSquareFootAffectedAreas.BackColor = Color.White;
            }
            if (txtNumRoomsAffected.Text == "")
            {
                retVal = true;
                txtNumRoomsAffected.BackColor = ErrorColor;
            }
            else
            {
                txtNumRoomsAffected.BackColor = Color.White;
            }
            if (txtItemsOfHighValue.Text == "")
            {
                retVal = true;
                txtItemsOfHighValue.BackColor = ErrorColor;
            }
            else
            {
                txtItemsOfHighValue.BackColor = Color.White;
            }
            if (txtPreExistingDamage.Text == "")
            {
                retVal = true;
                txtPreExistingDamage.BackColor = ErrorColor;
            }
            else
            {
                txtPreExistingDamage.BackColor = Color.White;
            }

            if (Equip.Visible == true)
            {
                if (!IsNumericCheck(txtFans.Text))
                {
                    txtFans.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtFans.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtDehus.Text))
                {
                    txtDehus.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtDehus.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtAffectedTemp.Text))
                {
                    txtAffectedTemp.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtAffectedTemp.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtAffectedRH.Text))
                {
                    txtAffectedRH.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtAffectedRH.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtAffectedGPP.Text))
                {
                    txtAffectedGPP.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtAffectedGPP.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtNonAffectedTemp.Text))
                {
                    txtNonAffectedTemp.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtNonAffectedTemp.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtNonAffectedRH.Text))
                {
                    txtNonAffectedRH.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtNonAffectedRH.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtNonAffectedGPP.Text))
                {
                    txtNonAffectedGPP.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtNonAffectedGPP.BackColor = Color.White;
                }

                if (!IsNumericCheck(txtExternalTemp.Text))
                {
                    txtExternalTemp.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtExternalTemp.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtExternalRH.Text))
                {
                    txtExternalRH.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtExternalRH.BackColor = Color.White;
                }
                if (!IsNumericCheck(txtExternalGPP.Text))
                {
                    txtExternalGPP.BackColor = ErrorColor;
                    retVal = true;
                }
                else
                {
                    txtExternalGPP.BackColor = Color.White;
                }

            }


            if (txtRequiredRepairs.Text == "")
            {
                retVal = true;
                txtRequiredRepairs.BackColor = ErrorColor;
            }
            else
            {
                txtRequiredRepairs.BackColor = Color.White;
            }

            try
            {
                if (ddlPM.SelectedIndex == 0)
                {
                    retVal = true;
                    ddlPM.BackColor = ErrorColor;
                }
                else
                {
                    ddlPM.BackColor = Color.White;
                }
            }
            catch
            {
                retVal = true;
            }
            if (retVal == true)
            {

                string myStringVariable = "Please Complete Highlighted Fields";
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + myStringVariable + "');", true);
            }
            return retVal;
        }

        private bool IsNumericCheck(string TxtBox)
        {
            decimal n;
            bool isNumeric = decimal.TryParse(TxtBox, out n);
            return isNumeric;
        }

        private string GetLocationOfLoss(int lolAddressId)
        {
            string lolAddress = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (lolAddressId != 0)
            {
                DataTable AddressInfo = Claim.GetLocationOfLoss(lolAddressId);
                if (AddressInfo.Rows.Count > 0)
                {
                    DataRow row = AddressInfo.Rows[0];
                    if (row["Line1"] != DBNull.Value)
                        sb.Append((string)row["Line1"]);
                    if (row["Line2"] != DBNull.Value)
                        sb.Append(", " + (string)row["Line2"]);
                    if (row["City"] != DBNull.Value)
                        sb.Append("<BR>" + (string)row["City"] + ", ");
                    if (row["Province"] != DBNull.Value)
                        sb.Append((string)row["Province"] + ", ");
                    if (row["PostalCode"] != DBNull.Value)
                        sb.Append((string)row["PostalCode"] + " ");
                }
                if (sb.Length > 0)
                    lolAddress = sb.ToString();
            }
            return lolAddress;
        }

        protected void btnret_Click(object sender, System.EventArgs e)
        {
            this.Goback();
        }

        private void Goback()
        {
            this.Response.Redirect("Claims.aspx");
        }

        private int GetIntCookie(string cookieName)
        {
            if (this.Request.Cookies[cookieName] != null)
            {
                if (Doxess.Web.WorkFlow.Util.IsNumeric(this.Request.Cookies[cookieName].Value))
                    return Convert.ToInt32(this.Request.Cookies[cookieName].Value);
                return 0;
            }
            return 0;
        }

        protected void grpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployee(0);
            ddlPM.Focus();
        }

        protected void btnsubmt_Click(object sender, EventArgs e)
        {
            // Check 
            string message = string.Empty;
            this.lblMessage.Text = string.Empty;

            if (CheckInputWritingPR()) return;


            SetParameters();           
            Process process = new Process();
            DataTable detInfo = process.wfTaskDetailinfo(this.detId);
            if (detInfo.Rows.Count > 0)
            {
                DataRow row = detInfo.Rows[0];
                //this.SetEmergency();
                int CreatedByUserId = Convert.ToInt32(row["CreatedByUserId"].ToString());
            }

            //Get All the value from the Page

            string property = txtProperty.Text.Trim();

            string restoration = txtRestoration.Text.Trim();
            bool isAdjusterNeeded = Convert.ToBoolean(Convert.ToInt16(rbtnAdjusterNeeded.SelectedValue));
            bool isBuildingDamage = Convert.ToBoolean(Convert.ToInt16(rbtnBuildingDamage.SelectedValue));

            bool isEmergency = Convert.ToBoolean(Convert.ToInt16(rbtnEmergencyService.SelectedValue));
            bool isContentDamage = Convert.ToBoolean(Convert.ToInt16(rbtnContentDamage.SelectedValue));
            string responded = txtResponded.Text.Trim();
            string inpectionBy = ddlPM.SelectedItem.Text;
            string PMComment = "";
            string RequiredRepairs = Convert.ToString(txtRequiredRepairs.Text.Trim());
            string PreExistingDamage = Convert.ToString(txtPreExistingDamage.Text.Trim());
            string drystrat = ddlDryingStrategy.SelectedValue.Trim();

            string TearOutRequired = ddlTearOutRequired.SelectedValue.Trim();

            string AgeOfProperty = Convert.ToString(txtAgeOfProperty.Text.Trim());
            string SFOfBuilding = Convert.ToString(txtSFOfBuilding.Text.Trim());
            string ConditionOfBuilding = Convert.ToString(txtConditionOfBuilding.Text.Trim());
            string QualityOfFinish = ddlQualityOfFinish.SelectedValue.Trim();
            string TypeOfBuilding = ddlTypeOfBuilding.SelectedValue.Trim();
            string SFAffectedAreas = Convert.ToString(txtSquareFootAffectedAreas.Text.Trim());

            string NumRoomsAffected = Convert.ToString(txtNumRoomsAffected.Text.Trim());
            string WaterType = ddlWaterType.SelectedValue.Trim();

            bool isKitchenAffected = Convert.ToBoolean(Convert.ToInt16(rbtnKitchenAffected.SelectedValue));
            bool isBathRoomAffected = Convert.ToBoolean(Convert.ToInt16(rbtnBathRoomAffected.SelectedValue));
            bool isHazardousMaterials = Convert.ToBoolean(Convert.ToInt16(rbtnHazardousMaterials.SelectedValue));
            bool VacatePremises = Convert.ToBoolean(Convert.ToInt16(rbtnVacatePremises.SelectedValue));

            string FireType = ddlFireType.SelectedValue.Trim();

            string DamageLocation = Convert.ToString(txtDamageLocation.Text.Trim());
            string ItemsOfHighValue = Convert.ToString(txtItemsOfHighValue.Text.Trim());
            decimal Conbudget = decimal.Parse(txtContentBudgetReserve.Text.Trim());

            Smartflow.BLL.Claim.PreliminaryReportsf pr = new Smartflow.BLL.Claim.PreliminaryReportsf(this.detId);
            pr.ClaimID = this.ClaimID;

            pr.PMComment = PMComment;
            pr.DPCTB = PreExistingDamage;
            pr.DRR = RequiredRepairs;
            pr.vacateprem = VacatePremises;
            pr.dryst = drystrat; 
            pr.amhazm = isHazardousMaterials;
            pr.toreq = TearOutRequired;

            pr.AgeOfProperty = AgeOfProperty;
            pr.SFOB = SFOfBuilding;
            pr.COBuild = ConditionOfBuilding;
            pr.qualoffinish = QualityOfFinish;
            pr.DateSent = Convert.ToString(DateTime.Now);
            pr.Property = property;

            pr.Restoration = restoration;
            pr.AdjusterNeeded = isAdjusterNeeded;
            pr.BuildingDamage = isBuildingDamage;
 
            pr.EmergencyService = isEmergency;
            pr.ContentDamage = isContentDamage;
            pr.Responded = responded;
            pr.InspectionBy = inpectionBy;

            pr.ddlTOBuild = TypeOfBuilding;

            pr.SFAA = SFAffectedAreas;

            pr.RoomAffected = NumRoomsAffected;
            pr.wattype = WaterType;
            pr.kitaffect = isKitchenAffected;
            pr.firetype = FireType;
            pr.bathaffect = isBathRoomAffected;
            pr.dmLoc = DamageLocation;
            pr.IHV = ItemsOfHighValue;
            pr.EmergReserve = Decimal.Parse(txtEmergencyReserveAmount.Text.Trim());
            pr.RestReserve = Decimal.Parse(txtRestorationReserveAmount.Text.Trim());

            pr.conbudget = Conbudget;


            if (divEmergInfo.Visible == true)
            {
                pr.EmergCompDate = txtEmergCompletionDate.Text.ToString();
            }
            else
            {
                pr.EmergCompDate = DateTime.Today.ToString(Global.DefDateFormat());
            }

            pr.WfID = this.WfID;
            pr.DocId = (int)Claim.wfDocuments.Preliminary_Report;
            pr.UserId = this.userId;
            pr.CreatedDate = DateTime.Today.ToString(Global.DefDateFormat()); // DateTime.Today.Month + "/" + DateTime.Today.Day + "/" + DateTime.Today.Year;
            pr.subProcessId = SubProcessId;


            WfDocument wfDoc = new WfDocument(22120, this.detId, this.userId);
            
            DataAccess dba = new DataAccess();
            int[] nextTaskIds = null; //new int[nextActionIDs.Length];
            Doxess.Web.WorkFlow.Process.Process dxProcess = new Doxess.Web.WorkFlow.Process.Process();
            using (SqlConnection conn = dba.dxConnection)
            {
                conn.Open();
                message = string.Empty;
                SqlTransaction transaction = conn.BeginTransaction();
                // begin Transaction -- all database process should under the Transaction 
                // if any step fails, Transaction rollback
                SqlCommand command = conn.CreateCommand();   // This command object must be used for any database update jobs following.								
                try
                {
                    command.Transaction = transaction;

                    // first to perform application related database update jobs
                    // in the jobs the already created command object MUST BE USED to force Transaction 
                    // Don't create and use any other SqlCommand object in these database update jobs! That is very important!
                    // Don't call transaction.Commit() iniside any step method.
                    //*********************************************************

                    //application related database update jobs here
                    pr.Update(ref message, command);

                    if (!message.Equals(""))
                    {
                        transaction.Rollback();
                        lblMessage.Text = message;
                        return;
                    }

                    //********************************************************/  

                    // WorkFlow related jobs as follows 
                    //ref string message, SqlCommand command, int currentDetId, int userId, int[] actionIds, int assignedToAccountId, int assignedToUserId
                    //nextTaskIds = dxProcess.wfMarkTaskAsCompleted(ref message, command, this.detId,userId, nextActionIDs, nextAccountID, assignedToUserId);
                    if (Equip.Visible == true)
                    {
                        int msg = PreliminaryReportcs.AddMoistureReadings(WfID, 1, Convert.ToDecimal(txtAffectedTemp.Text), Convert.ToDecimal(txtAffectedRH.Text), Convert.ToDecimal(txtAffectedGPP.Text), Convert.ToDecimal(txtNonAffectedTemp.Text), Convert.ToDecimal(txtNonAffectedRH.Text), Convert.ToDecimal(txtNonAffectedGPP.Text), Convert.ToDecimal(txtExternalTemp.Text), Convert.ToDecimal(txtExternalRH.Text), Convert.ToDecimal(txtExternalGPP.Text), Convert.ToInt32(txtFans.Text), Convert.ToInt32(txtDehus.Text));

                        if (msg != 1)
                        {
                            lblMessage.Text = Global.ctx("r002043") + message;  //  r002043="Fail update this task as completed."
                            return;
                        }
                    }

                    nextTaskIds = dxProcess.wfTaskComplete(ref message, command, this.detId, userId); //, nextActionIDs
                    if (!message.Equals(""))
                    {
                        transaction.Rollback();
                        //goto outline;
                        lblMessage.Text = Global.ctx("r002043") + message;  //  r002043="Fail update this task as completed."
                        return;
                    }
                    foreach (int item in nextTaskIds)
                    {
                        if (item < 0)
                        {
                            transaction.Rollback();
                            lblMessage.Text = Global.ctx("r002028");  //  r002028="Adding new tasks fails."
                            return;
                            //goto outline;
                        }
                    }

                    // any optional update WfDet fields 					
                    if (wfDoc.DocIdList.Count > 0)
                    {
                        message = wfDoc.AddDocuments(command);
                        if (!message.Equals(""))
                        {
                            transaction.Rollback();
                            this.lblMessage.Text = message;
                            return;
                        }
                    }

                    if (message.Equals(string.Empty))
                    {
                        transaction.Commit();

                        dba.dxClearParameters();
                        //delete the Row for docId = 23 i.e Blank PR Report
                        dba.dxAddParameter("@WfId", this.WfID);
                        dba.dxAddParameter("DocId", 23);
                        dba.dxExecuteSP("DeleteWfDoc");

                        // Save Reserve amounts
                        //int subProcessId = Claim.GetSubProcessIdByDetId(this.detId);
                        //ClaimInfoData claimExistingInfo = new ClaimInfoData(this.ClaimID, subProcessId);

                        //claimExistingInfo.ReserveAmt1 = Convert.ToDecimal(this.txtEmergencyReserveAmount.Text.Trim());
                        //claimExistingInfo.ReserveAmt2 = Convert.ToDecimal(this.txtRestorationReserveAmount.Text.Trim());

                        //claimExistingInfo.conbudget = Convert.ToDecimal(this.txtContentBudgetReserve.Text.Trim());

                        //message = claimExistingInfo.Update();

                        if (message != string.Empty)
                        {
                            this.lblMessage.Text = Global.ctx("r002044") + message;  //  r002044="The following error occurred when attempting to update the reserve values: "
                            return;
                        }

                        if (hidBlnCloseFile.Value == "true")
                        {
                            if (ClaimCombined.isCombined_wfid(WfID))
                            {
                                ClaimCombined.CloseEmergencyFSFromPR(WfID);
                            }
                            else
                            {
                                int claimId = WorkFlow.BLL.Claim.Claim.GetClaimIdByWfId(WfID);
                                DataTable clTable = WorkFlow.BLL.Claim.Claim.GetClaimData(claimId, SubProcessId);
                                DataTable tblProfile = WorkFlow.BLL.Claim.Claim.GetProfile(Int32.Parse(clTable.Rows[0]["InsPrincipalId"].ToString()));

                                bool emergencyHas660Action = Claim.Has660Action(WfID, 1);
                                bool restorationHas660Action = Claim.Has660Action(WfID, 2);

                                if (tblProfile != null)
                                {
                                    if (tblProfile.Rows.Count != 0)
                                    {
                                        DataRow row = tblProfile.Rows[0];
                                        string message1 = string.Empty;
                                        int nextAction = 0;

                                        DataAccess dba1 = new DataAccess();
                                        Doxess.Web.WorkFlow.Process.Process dxProcess1 = new Doxess.Web.WorkFlow.Process.Process();
                                        using (SqlConnection conn1 = dba.dxConnection)
                                        {
                                            conn1.Open();
                                            SqlTransaction transaction1 = conn1.BeginTransaction();
                                            // begin Transaction -- all database process should under the Transaction 
                                            // if any step fails, Transaction rollback
                                            SqlCommand command1 = conn1.CreateCommand();   // This command object must be used for any database update jobs following.								
                                            try
                                            {
                                                command1.Transaction = transaction1;
                                                //decimal FeeWithoutSub = Decimal.Parse(row["FeeWithoutSub"].ToString());
                                                int nextAccountID = Int32.Parse(clTable.Rows[0]["BranchAccId"].ToString());
                                                message1 = Invoice.CancelledAllOpenInvoice(command1, WfID, SubProcessId);
                                                if (!message1.Equals(string.Empty))
                                                {
                                                    transaction1.Rollback();
                                                    this.litMessage.Text = Global.ctx("r002045");  //  r002045="Unable to cancel open invoices."
                                                    return;
                                                }

                                                // Check if tasks completed
                                                this.PictureUploadCompletion = Claim.CheckIfTaskCompleted(command, this.WfID, 0, 22052);
                                                this.ScheduleInspectionCompletion = Claim.CheckIfTaskCompleted(command, this.WfID, this.SubProcessId, 22051);
                                                this.ScheduleAppointmentCompletion = Claim.CheckIfTaskCompleted(command, this.WfID, this.SubProcessId, 22055);
                                                this.PRCompletion = Claim.CheckIfTaskCompleted(command, this.WfID, this.SubProcessId, 22120);

                                                message1 = dxProcess1.CloseAllPreviousTasks(command1, this.WfID, SubProcessId);
                                                if (!message1.Equals(string.Empty))
                                                {
                                                    transaction1.Rollback();
                                                    this.litMessage.Text = Global.ctx("r002046");  //  r002046="Unable to cancel previous tasks."
                                                    return;
                                                }

                                                // Create Task 660
                                                nextAction = 22660;
                                                this.detId = dxProcess1.wfTaskCreate(ref message1, command1, WfID, nextAction, -1, nextAccountID, -1, "", "", "", -1, -1, SubProcessId);
                                                if (!message1.Equals(string.Empty) || detId < 0)
                                                {
                                                    this.litMessage.Text = message1;
                                                    transaction1.Rollback();
                                                    return;
                                                }

                                                message1 = Claim.UpdateFileStatus(command1, claimId, SubProcessId, 12);
                                                if (!message1.Equals(""))
                                                {
                                                    transaction1.Rollback();
                                                    litMessage.Text = Global.ctx("r002007");  //  r002007="Fail to update Claims File status."
                                                    return;
                                                }

                                                int destProcess = 2;
                                                string destTxt = "Restoration";
                                                if (SubProcessId == 2)
                                                {
                                                    destProcess = 1;
                                                    destTxt = "Emergency";
                                                }
                                                if (PRCompletion == 1)
                                                {
                                                    message = Claim.TransferTaskSubProcess(command, WfID, destProcess, 22120);
                                                    if (!message.Equals(""))
                                                    {
                                                        transaction.Rollback();
                                                        litMessage.Text = "Failed to transfer PR task to the " + destTxt + " file.";
                                                        return;
                                                    }
                                                }
                                                if (PictureUploadCompletion == 1)
                                                {
                                                    message = Claim.TransferTaskSubProcess(command, WfID, destProcess, 22052);
                                                    if (!message.Equals(""))
                                                    {
                                                        transaction.Rollback();
                                                        litMessage.Text = "Failed to transfer Picture Upload task to the " + destTxt + " file.";
                                                        return;
                                                    }
                                                }
                                                else if ((PictureUploadCompletion <= 0) &&
                                                         (((destProcess == 1) && (ScheduleInspectionCompletion == 2)) || ((destProcess == 2) && (ScheduleAppointmentCompletion == 2)))
                                                       )
                                                {
                                                    int pictureUploadUser = Claim.ProjectManagerId(command, WfID, SubProcessId);
                                                    int pictureUploadBranch = Invoice.getBranchIdByWfIdSubProcessId(command, WfID, SubProcessId);
                                                    Claim.CreatePictureUploadTask(command, WfID, SubProcessId == 1 ? 2 : 1, pictureUploadBranch, pictureUploadUser);
                                                }




                                                //Add Log Entry
                                                int intWfLog = WorkFlow.BLL.Claim.Notes.AddNewNote(Convert.ToInt32(WfID), "U", 0,
                                                    DateTime.Now.ToString(), Convert.ToInt32(User.Identity.Name), "Claim closed for fee", SubProcessId);
                                                if (intWfLog != 0)
                                                {
                                                    transaction1.Rollback();
                                                    message1 = Global.ctx("r002048");  //  r002048="Unable to add a log entry."
                                                    return;
                                                }
                                                /**************** END OF ADDING BY JWALIN ON 27-fEB-2007 ***** ISR:1470 *******/
                                                transaction1.Commit();

                                            }
                                            catch (Exception ex)
                                            {
                                                this.litMessage.Text = ex.Message;
                                                transaction1.Rollback();
                                                return;
                                            }
                                        }
                                    }
                                    else
                                        this.litMessage.Text = "No Insurance Company is not defined for this claim. Please contact the system administrator";
                                }
                                else
                                    this.litMessage.Text = "No Insurance Company is not defined for this claim. Please contact the system administrator";
                            }
                        }

                        if (pr.RestReserve>0)
                        {
                            if (CreateRestoration())
                            {
                                litMessage.Text = "All processes have completed though there was an error creating the requested Restoration file. This will now have to be manually created.";
                                return;
                            }
                        }

                    }
                    else
                        this.lblMessage.Text = message;
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                    transaction.Rollback();
                }
                finally
                {
                    //  Notes.AddNewNote(WfID, "A", this.detId, DateTime.Now.ToString(), Convert.ToInt32(this.User.Identity.Name.ToString()), this.Title, 0);
                    string Milestone = "Preliminary Report Completed";
                    Notes.ClaimHistoryAdd(WfID, Claim.GetSubProcessIdByDetId(detId), Milestone, "", "", DateTime.Now.ToString(), 0, "A", userId, "PR Report", 3, 1, 0);
                    pr.CreateTasks(WfID, SubProcessId, 22120, userId);
                    updateEstimatingTimelines();
                    this.Goback();
                    // clear up objects
                    if (transaction != null) transaction.Dispose();
                    if (command != null) command.Dispose();
                }
            }
        }
        private void updateEstimatingTimelines()
        {
            DataAccess dba1 = new DataAccess();
            using (SqlConnection conn = dba1.dxConnection)
            {
                conn.Open();
                string message = string.Empty;
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand command = conn.CreateCommand();   // This command object must be used for any database update jobs following.								
                try
                {
                    command.Transaction = transaction;
                    message = Claim.EstimatingTimeLinesUpdate(command, WfID, SubProcessId);


                    if (!message.Equals(""))
                    {
                        transaction.Rollback();
                        return;
                    }
                    // Success to commit Transaction 
                    transaction.Commit();
                }

                catch (SqlException ex)
                {
                    transaction.Rollback();
                    message = ex.Message;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    message = ex.Message;
                }
                finally
                {
                    // clear up objects
                    if (transaction != null) transaction.Dispose();
                    if (command != null) command.Dispose();
                }
            }
        }
        private bool CreateRestoration()
        {
            DataAccess dba = new DataAccess();
            int Ropen = dba.dxGetIntData("select count(*) from ClaimValues where wfid=" + this.WfID.ToString() + " and subprocessid=2");
            if (Ropen > 0)
            {
                this.litMessage.Text = "Restoration file is already created";
                return false;
            }

            int nextAction = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["RestorationAction"]);
            int actionId;
            int wfDetId;

            int subProcessId = Doxess.Web.WorkFlow.Process.Process.wfGetSubProcessId(nextAction, this.detId);
            DataTable clTable = WorkFlow.BLL.Claim.Claim.GetClaimData(ClaimID, 1);
            DateTime InsReceivedDate = DateTime.Parse(clTable.Rows[0]["InsReceivedDate"].ToString());
            DateTime InsDateOfLoss = DateTime.Parse(clTable.Rows[0]["InsDateOfLoss"].ToString());
            string message = string.Empty;
            if (clTable.Rows.Count > 0 && clTable.Rows[0]["BranchAccId"] != DBNull.Value && clTable.Rows[0]["BranchAccId"].ToString() != string.Empty)
            {
                int nextAccountID = Int32.Parse(clTable.Rows[0]["BranchAccId"].ToString());  //  Doxess.Web.WorkFlow.Process.Process.wfGetAccountIdByActionId(nextAction);

                int insAdjusterId = Int32.Parse(clTable.Rows[0]["InsAdjusterId"].ToString()); // adjuster ID
                int InsTemplateId = Int32.Parse(clTable.Rows[0]["InsTemplateId"].ToString()); // Ins TemplateId
                string InsPolicyNo = clTable.Rows[0]["InsPolicyNo"].ToString(); // Cliam No / Policy No
                decimal InsDeductibleAmt = Decimal.Parse(clTable.Rows[0]["InsDeductibleAmt"].ToString()); // InsDeductibleAmt
                int insAccId = Int32.Parse(clTable.Rows[0]["insAccId"].ToString()); // Insurance Account Id
                int InsPrincipalId = Int32.Parse(clTable.Rows[0]["InsPrincipalId"].ToString()); // Insurance Principal ID
                int projectAdminId = Int32.Parse(clTable.Rows[0]["AdministratorId"].ToString()); // Project Administrator ID

                Doxess.Web.WorkFlow.Process.Process dxProcess = new Doxess.Web.WorkFlow.Process.Process();
                using (SqlConnection conn = dba.dxConnection)
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    // begin Transaction -- all database process should under the Transaction 
                    // if any step fails, Transaction rollback
                    SqlCommand command = conn.CreateCommand();   // This command object must be used for any database update jobs following.								
                    try
                    {
                        command.Transaction = transaction;
                        this.detId = dxProcess.wfTaskCreate(ref message, command, this.WfID, nextAction, -1, nextAccountID, projectAdminId, "", "", "", -1, -1, subProcessId);
                        if (!message.Equals(string.Empty) || detId < 0)
                        {
                            this.litMessage.Text = message;
                            transaction.Rollback();
                            return false;
                        }
                        // Update JobType to ER
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@wfId", this.WfID);
                        command.CommandText = "CreateRestorationProcess";
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        decimal DefMargin = Claim.GetDefTargetMargin(2);
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@wfId", this.WfID);
                        command.Parameters.AddWithValue("@SubProcId", 2);
                        command.Parameters.AddWithValue("@TargetMargin", DefMargin);
                        command.CommandText = "UpdateTargetMargin800";
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        if (insAdjusterId == 0 & InsTemplateId != 2)  // Need to assign adjuster later
                        {
                            actionId = 22020;
                            wfDetId = dxProcess.wfTaskCreate(ref message, command, this.WfID, actionId, -1, nextAccountID, -1, "", "", "", -1, -1, subProcessId);
                        }
                        if ((InsTemplateId == 0 || InsTemplateId == 1))
                        {
                            if (InsPolicyNo.Trim().Equals(string.Empty))  // Need to assign adjuster later
                            {
                                actionId = 22023;
                                wfDetId = dxProcess.wfTaskCreate(ref message, command, this.WfID, actionId, -1, nextAccountID, -1, "", "", "", -1, -1, subProcessId);
                            }
                            if (InsDeductibleAmt == 0)
                            {
                                actionId = 22022;
                                wfDetId = dxProcess.wfTaskCreate(ref message, command, this.WfID, actionId, -1, nextAccountID, -1, "", "", "", -1, -1, subProcessId);
                            }
                        }
                        if (InsTemplateId == 0)
                        {
                            if (insAccId != 0)
                            {
                                string strType = WorkFlow.Objects.BLL.Insurance.GetInsCompanyType(insAccId);//Int32.Parse(this.ddlCompany.SelectedValue.ToString()));
                                if (strType == "A" || insAccId.ToString() == "1000")
                                {
                                    if (InsPrincipalId == 0)
                                    {
                                        actionId = 22021;
                                        wfDetId = dxProcess.wfTaskCreate(ref message, command, this.WfID, actionId, -1, nextAccountID, -1, "", "", "", -1, -1, subProcessId);
                                    }
                                }
                            }
                        }
                        else if (InsTemplateId == 1)
                        {
                            if (InsPrincipalId == 0) //if (this.ddlPrincipalCarrier.SelectedValue.Equals("0"))
                            {
                                actionId = 22021;
                                wfDetId = dxProcess.wfTaskCreate(ref message, command, this.WfID, actionId, -1, nextAccountID, -1, "", "", "", -1, -1, subProcessId);
                            }
                        }
                        message = Claim.EstimatingTimeLinesInsert(command, this.WfID, subProcessId);
                        if (!message.Equals(""))
                        {
                            transaction.Rollback();
                            return false;
                        }
                        // Success to commit Transaction 
                        transaction.Commit();
                        Notes.AddNewNote(this.WfID, "A", 0, DateTime.Now.ToString(), Convert.ToInt32(Page.User.Identity.Name), "Restoration added to claim", 2);
                        string Milestone = "Date Of Loss";

                        Notes.ClaimHistoryAdd(WfID, 2, Milestone, "", "", InsDateOfLoss.ToString(Global.DefDateFormat()), 0, "A", userId, "0", 0, 0, 0);
                        Milestone = "Claim Received";
                        Notes.ClaimHistoryAdd(WfID, 2, Milestone, "", "", InsReceivedDate.ToString(Global.DefDateFormat()), 0, "A", userId, "0", 0, 0, 0);
                        this.Response.Cookies.Add(new HttpCookie("claimMessage", message));
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        message = ex.Message;
                        this.litMessage.Text = message;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        message = ex.Message;
                        this.litMessage.Text = message;
                        return false;
                    }
                    finally
                    {
                        // clear up objects
                        if (transaction != null) transaction.Dispose();
                        if (command != null) command.Dispose();
                    }

                }
            }
            else
            {
                message = "Branch data should not be blank. Please contact the system administrator.";
                this.litMessage.Text = message;
                return false;
            }


        }
    }
}


        
       

