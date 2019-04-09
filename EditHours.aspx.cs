using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Doxess.Data;
using System.Data;
using System.Collections;
using WorkFlow.BLL.Claim;
using WorkFlow.BLL.Email;
using System.IO;
using System.Configuration;

namespace SFLite
{
    public partial class EditHours : System.Web.UI.Page
    {
        public int userID
        {
            get { return (int)ViewState["userID"]; }
            set { ViewState["userID"] = value; }
        }
        public string LiteROLE
        {
            get { return (string)ViewState["LiteROLE"]; }
            set { ViewState["LiteROLE"] = value; }
        }
        public string JobName
        {
            get { return (string)ViewState["JobName"]; }
            set { ViewState["JobName"] = value; }
        }
        public string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        public string sday
        {
            get { return (string)ViewState["sday"]; }
            set { ViewState["sday"] = value; }
        }
        public string eday
        {
            get { return (string)ViewState["eday"]; }
            set { ViewState["eday"] = value; }
        }
        public string upParm
        {
            get { return (string)ViewState["upParm"]; }
            set { ViewState["upParm"] = value; }
        }
        // Sch properties
        private int SchId { get { return (int)ViewState["SchId"]; } set { ViewState["SchId"] = value; } }
        private int ResId { get { return (int)ViewState["ResId"]; } set { ViewState["ResId"] = value; } }
        private int ProjId { get { return (int)ViewState["ProjId"]; } set { ViewState["ProjId"] = value; } }
        private int ProcessId { get { return (int)ViewState["ProcessId"]; } set { ViewState["ProcessId"] = value; } }
        private int SubProcessId { get { return (int)ViewState["SubProcessId"]; } set { ViewState["SubProcessId"] = value; } }
        private int SchCatId { get { return (int)ViewState["SchCatId"]; } set { ViewState["SchCatId"] = value; } }
        private int BatchId { get { return (int)ViewState["BatchId"]; } set { ViewState["BatchId"] = value; } }
        private DateTime StartDate { get { return (DateTime)ViewState["StartDate"]; } set { ViewState["StartDate"] = value; } }
        private DateTime EndDate { get { return (DateTime)ViewState["EndDate"]; } set { ViewState["EndDate"] = value; } }
        private decimal Hours { get { return (decimal)ViewState["Hours"]; } set { ViewState["Hours"] = value; } }
        private string Desc { get { return (string)ViewState["Desc"]; } set { ViewState["Desc"] = value; } }
        private int wfid;

        private decimal LunchTime { get { return (decimal)ViewState["LunchTime"]; } set { ViewState["LunchTime"] = value; } }

        public int anchorLink = 0;
        DataAccess dba = new DataAccess();

        public string origstrt;
        public string origendt;
        public string origaddInfo;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ClaimList.Attributes.Add("style", "display: block");
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }
                if (Request.Cookies["MYLITEROLE"] != null)
                {
                    this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
                }
                if (Request.Cookies["CatID"] != null)
                {
                    this.cat = this.Request.Cookies["CatID"].Value.ToString();
                }
                this.JobName = Request.QueryString["fi"] + "</br>" + Request.QueryString["cl"];
                this.upParm = Request.QueryString["up"];
                wfid=Convert.ToInt32(Request.QueryString["wfid"]);
                int sc= Convert.ToInt32(Request.QueryString["sc"]);
                int uid= Convert.ToInt32(Request.QueryString["uid"]);

                if (upParm=="2") abtnSubmit.Visible = false;
                this.SubProcessId = 1;
                if (this.JobName.IndexOf("R</br>") > 0) this.SubProcessId = 2;
                this.SchId = Convert.ToInt32(Request.QueryString["sc"]);
                if (this.LiteROLE == "C")
                {
                    Page.Master.FindControl("mpSrch").Visible = false;
                    HtmlAnchor a1 = (HtmlAnchor)Page.Master.FindControl("action1");
                    a1.InnerText = "Scheduled Jobs";
                }
                DataTable index = new DataTable();

                if (wfid == 1191)
                {
                    dba.dxAddParameter("@userid",uid);
                    ResId = dba.dxGetIntData("select ResId from SchResources where Resindexid = (select empid from employees where Userid = @userid ) and ResCatId=10");
                    dba.dxClearParameters();
                    dba.dxAddParameter("@id", sc);
                    dba.dxAddParameter("@ResId", ResId);
                    index = dba.dxGetSPData("schschedulebyidstat");
                    abtnSubmit.Visible = false;

                }
                else
                {
                    index = dba.dxGetTable("select * from schedule where schid = " + this.SchId.ToString());
                }

                DataRow Sch = index.Rows[0];
                this.ResId = Convert.ToInt32(Sch["ResId"]);
                this.ProjId = Convert.ToInt32(Sch["ProjId"]);
                this.ProcessId = Convert.ToInt32(Sch["ProcessId"]);
                this.SchCatId = Convert.ToInt32(Sch["SchCatId"]);
                this.BatchId = Convert.ToInt32(Sch["BatchId"]);
                this.StartDate = Convert.ToDateTime(Sch["startDate"].ToString());
                this.EndDate = Convert.ToDateTime(Sch["endDate"].ToString());
                this.Hours = Convert.ToDecimal(Sch["Hours"].ToString());
                this.Desc = Sch["Description"].ToString();
                sday = this.StartDate.ToLongDateString();
                eday = this.EndDate.ToLongDateString();
                MakeTimeDropItems(ddlStartTime, DateTime.Parse(Sch["startDate"].ToString()));
                MakeTimeDropItems(ddlEndTime, DateTime.Parse(Sch["endDate"].ToString()));
                origstrt = ddlStartTime.SelectedValue.ToString();
                origendt = ddlEndTime.SelectedValue.ToString();
                origaddInfo = Sch["additionalInfo"].ToString();
                this.txtAddInfo.Value = origaddInfo;
                lblmsg.Text = "";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if ((this.txtAddInfo.Value != origaddInfo) || (origstrt != ddlStartTime.SelectedValue.ToString()) || (origendt != ddlEndTime.SelectedValue.ToString()))
            {
                string strDefCulzone = string.Empty;
                string sql = "";
                strDefCulzone = System.Configuration.ConfigurationSettings.AppSettings["defCulZone"].ToString();
                if (strDefCulzone == string.Empty) strDefCulzone = "en-US";
                IFormatProvider culture = new System.Globalization.CultureInfo(strDefCulzone, true);
                DateTime newStartDate = DateTime.Parse(sday + " " + ddlStartTime.SelectedValue, culture);
                DateTime newEndDate = DateTime.Parse(eday + " " + ddlEndTime.SelectedValue, culture);
                Decimal newHours = Convert.ToDecimal((newEndDate - newStartDate).TotalHours);
                string newAddInfo = this.txtAddInfo.Value.Replace("'", "''");

                if (IsOverlapSchedule(newStartDate, newEndDate))
                {
                    if (lblmsg.Text == "") lblmsg.Text = "Hours conflict with another schedule item.  Please re-try.";
                    return;
                }

                TimeSpan diff; 
                DateTime sDate = newStartDate;
                if (IsLunchIncluded(newStartDate, newEndDate))
                    diff = newEndDate.Subtract(sDate.AddMinutes((double)this.LunchTime));
                else
                    diff = newEndDate.Subtract(sDate);
                string hours = diff.Hours.ToString() + "." + (diff.Minutes.ToString() == "30" ? "50" : diff.Minutes.ToString());
                newHours = Decimal.Parse(hours);


                // update Schedule as required...
                sql = "SchScheduleEdit " + this.ProjId.ToString() + ", " + this.SchId.ToString() + ", '" + newStartDate.ToString() + "', '"
                    + newEndDate.ToString() + "', '" + newHours.ToString() + "', 1, " + this.SubProcessId.ToString() + ", '" + newAddInfo + "'";
                dba.dxExecuteNonQuery(sql);

                // create Timebook record if necessary...
                sql = "SELECT COUNT(*) FROM Timebook WHERE ResId = " + this.ResId.ToString() + " AND Schid = " + this.SchId.ToString()
                    + " AND ProjId = " + this.ProjId.ToString() + " AND ProcessId = " + this.ProcessId.ToString();
                int exists = dba.dxGetIntData(sql);
                if (exists < 1)  // doesn't exist
                {
                    sql = "INSERT INTO TimeBook (ResId, ProjId, ProcessId, StartDate, EndDate, Hours, Description, "
                        + "DescriptionUnchangeable, CreatedBy, SubprocessId, SchCatId, PlanEditId, PlanStatusId, BatchId, schId, AdditionalInfo, tbStatusId) "
                        + "VALUES (" + this.ResId.ToString() + ", " + this.ProjId.ToString() + ", " + this.ProcessId.ToString() + ", '"
                        + newStartDate.ToString() + "', '" + newEndDate.ToString() + "', " + newHours.ToString() + ", '" + this.Desc.Replace("'", "''")
                        + "', null, " + this.userID.ToString() + ", " + this.SubProcessId.ToString() + ", " + this.SchCatId.ToString()
                        + ", 1, 1, " + this.BatchId.ToString() + ", " + this.SchId.ToString() + ", '" + newAddInfo + "', 5)";
                    dba.dxExecuteNonQuery(sql);

                }
                else  // update it.
                {
                    sql = "Update TimeBook set StartDate = '" + newStartDate.ToString() + "', EndDate = '" + newEndDate.ToString()
                        + "', Hours = " + newHours.ToString() + ", AdditionalInfo = '" + newAddInfo + "', tbStatusId = 5 "
                        + "WHERE ResId = " + this.ResId.ToString() + " AND Schid = " + this.SchId.ToString()
                        + " AND ProjId = " + this.ProjId.ToString() + " AND ProcessId = " + this.ProcessId.ToString() + " AND SubProcessId = " + this.SubProcessId.ToString();
                    dba.dxExecuteNonQuery(sql);
                }

                // email note to the PM/OA as required...
                if (chkEmailPMOA.Checked)
                {
                    string userName = dba.dxGetTextData("select FirstName+' '+LastName from wfUsers where userid=" + this.userID.ToString());
                    string wko = dba.dxGetTextData("select Description from Schedule where schid=" + this.SchId.ToString());
                    string basepath = ConfigurationSettings.AppSettings["PictureBasePath"].Replace("\\StrWebflow\\ClaimFiles\\","\\SFLite\\").Replace("\\Strone\\ClaimFiles\\","\\SFLite");
                    string messagebody = File.ReadAllText(basepath + "email_template.txt");
                    messagebody = messagebody.Replace("@@FILE@@", Request.QueryString["fi"]).Replace("@@NAME@@", Request.QueryString["cl"]);
                    messagebody = messagebody.Replace("@@JOBDATE@@", sday + " - " + userName).Replace("@@WKO@@", wko).Replace("@@INFO@@", this.txtAddInfo.Value);
                    messagebody = messagebody.Replace("@@START@@", ddlStartTime.SelectedValue.ToString()).Replace("@@END@@", ddlEndTime.SelectedValue.ToString());
                    messagebody = messagebody.Replace("'", "''");
                    int wfid = dba.dxGetIntData("select wfid from claims where claimid=" + this.ProjId.ToString());
                    string OAmailId = dba.dxGetTextData("SELECT U.BusinessEmailAddress FROM ClaimValues CV INNER JOIN wfUsers U ON CV.AdministratorId = U.UserId WHERE CV.WfId = " + wfid.ToString() + " AND CV.SubProcessId = " + this.SubProcessId.ToString());
                    string PMmailId = dba.dxGetTextData("SELECT U.BusinessEmailAddress FROM ClaimValues CV INNER JOIN wfUsers U ON CV.ProjManagerId = U.UserId WHERE CV.WfId = " + wfid.ToString() + " AND CV.SubProcessId = " + this.SubProcessId.ToString());
                    string recipient = "", join = "";
                    if (OAmailId != "") recipient += OAmailId; join = "; ";
                    if (PMmailId != "") recipient += join + PMmailId; 

                    string subject = "Schedule for JOB " + Request.QueryString["fi"] + " has been updated.";

                    if (recipient != "")
                    {
                         string Mailreq = "EXEC msdb.dbo.sp_send_dbmail @profile_name = 'SmartFlow', @recipients = '" + recipient + "', ";
                         Mailreq += "@body = '" + messagebody + "', @subject ='" + subject + "', @file_attachments = null, @body_format = 'HTML'";
                         dba.dxExecuteNonQuery(Mailreq);
                    }
                }

                Response.Redirect("Claims.aspx");
            }
        }

        void MakeTimeDropItems(DropDownList dl, DateTime selectedItem)
        {
            DateTime date = new DateTime();
            for (int intCount = 1; intCount <= 2; intCount++)
            {
                for (int i = 0; i < 24; i++)
                {
                    string str = date.ToString("h:mm");
                    string strTimeMode = "";
                    int intIndex = 0;
                    if (intCount == 1)
                    {
                        strTimeMode = "AM";
                        intIndex = i;
                    }
                    else if (intCount == 2)
                    {
                        strTimeMode = "PM";
                        intIndex = i + 24;
                    }

                    ListItem item = new ListItem();
                    item.Value = (str + " " + strTimeMode);
                    item.Text = (str + " " + strTimeMode);
                    if (date.ToString("h:mm tt") == selectedItem.ToString("h:mm tt"))
                        item.Selected = true;
                    dl.Items.Add(item);
                    date = date.AddMinutes(30);
                }
            }
        }

        private bool IsLunchIncluded(DateTime StartTime, DateTime EndTime)
        {
            bool RetValue = false;
            IFormatProvider culture = new System.Globalization.CultureInfo(WorkFlow.Global.DefCulZone(), true);
            string strDefDateFormat = WorkFlow.Global.DefDateFormat();

            ScheduleCom.SchSysRec schRec = new ScheduleCom.SchSysRec().GetSystemRecord;
            DateTime LunchSTime = DateTime.Parse(StartTime.ToString(strDefDateFormat) + " " + schRec.LunchStartTime.ToString("hh:mm tt"), culture);
            DateTime LunchETime = DateTime.Parse(StartTime.ToString(strDefDateFormat) + " " + schRec.LunchEndTime.ToString("hh:mm tt"), culture);
            TimeSpan LunchDiff = LunchETime.Subtract(LunchSTime);
            if (LunchDiff.Hours > 0)
                this.LunchTime = (LunchDiff.Hours * 60) + LunchDiff.Minutes;
            else
                this.LunchTime = LunchDiff.Minutes;
            if (StartTime <= LunchSTime && LunchETime <= EndTime)
                RetValue = true;
            return RetValue;
        }

        bool IsOverlapSchedule(DateTime StartTime, DateTime EndTime)
        {
            lblmsg.Text = "";
            string qry = "select count(*) from schedule where (schid <> " + this.SchId + " and resid = " + this.ResId + ") " +
               "and ((CONVERT(date, startdate) = CONVERT(date, '" + StartTime.AddMinutes(29).ToString() + "')) and (CONVERT(date, enddate) = CONVERT(date, '" + EndTime.AddMinutes(-29).ToString() + "')) " +
               "and (((StartDate <= CONVERT(datetime,'" + EndTime.AddMinutes(-29).ToString() + "')) and (EndDate >= CONVERT(datetime,'" + StartTime.AddMinutes(29).ToString() + "'))))" +
               " or  ((EndDate >= CONVERT(datetime,'" + StartTime.AddMinutes(29).ToString() + "')) and (StartDate <= CONVERT(datetime,'" + EndTime.AddMinutes(-29).ToString() + "'))))";
            //lblmsg.Text = qry;      // just while testing

            int conflict = dba.dxGetIntData(qry);
            if (conflict > 0) return true;
            return false;
        }

    }
}