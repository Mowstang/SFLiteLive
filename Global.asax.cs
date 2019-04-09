using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Mail;
using System.Web.SessionState;
using System.Security;
using System.Security.Principal;
using System.Web.Security;
using Doxess.Web.WorkFlow.Security;
using Doxess.Web.WorkFlow;
using Doxess.Data;
//using WorkFlow.Objects.BLL;

namespace SFLite
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        /// 

        private System.ComponentModel.IContainer components = null;

        public Global()
        {
            InitializeComponent();
        }

        protected void Application_Start(Object sender, EventArgs e)
        {


        }

        protected void Session_Start(Object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            WfUser principal = Security.wfGetUser(Request); //Wf_Sercurity.GetUser(Request);

            if (principal != null)
            {
                this.Context.User = principal;
            }
            return;
        }

        public static string ctx(string resource)
        {
            return HttpContext.GetGlobalResourceObject("ctx", resource).ToString();
        }

        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //    //if ( !(Request.Url.ToString().ToLower().IndexOf("localhost") > 0) )
        //    {
        //        WfUser principal = Security.wfGetUser(Request);
        //        string loguserid = string.Empty;
        //        if (principal != null)
        //            loguserid = TimebookCom.WeeklyTimebook.GetUserNameById(Convert.ToInt32(principal.wfUserID)).ToString() + " (" + principal.wfUserID + ")";
        //        MailMessage msg = new MailMessage();
        //        SmtpMail.SmtpServer = "smtp.win.terago.ca";// string.Empty;
        //        //msg.BodyFormat = MailFormat.Html;
        //        msg.Body = "Error: " + Server.GetLastError().InnerException.ToString() + "\n\n Logon user: " + loguserid + "\n\n" + System.DateTime.Now.ToString();
        //        msg.Body += "\n\nWfId: " + this.GetIntCookie("WfId").ToString() + " SubProcess: " + this.GetIntCookie("SubProcessId").ToString() + "\n\n" + Request.Url;
        //        msg.To = "darren.monks@strone.ca";
        //        msg.From = "errors@strone.ca";
        //        msg.Subject = "Application Error";
        //        SmtpMail.Send(msg);

        //        WorkFlow.Objects.BLL.PageErrorLog.WritePageErrorLog(Convert.ToInt32(principal.wfUserID), DateTime.Now, Request.Url.ToString(), Server.GetLastError().InnerException.ToString());
        //    }

        //}

        private int GetIntCookie(string cookieName)
        {
            if (this.Request.Cookies[cookieName] != null)
            {
                if (Doxess.Web.WorkFlow.Util.IsNumeric(this.Request.Cookies[cookieName].Value))
                {
                    return Convert.ToInt32(this.Request.Cookies[cookieName].Value);
                }
                return 0;
            }
            return 0;
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            if (Session["excel"] != null)
            {
                if (System.IO.File.Exists(Session["excel"].ToString()))
                {
                    System.IO.File.Delete(Session["excel"].ToString());
                }
            }
        }

        protected void Application_End(Object sender, EventArgs e)
        {

        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion

        public static int GetIntCookie(System.Web.HttpRequest req, string cookieName)
        {
            if (req.Cookies[cookieName] != null)
            {
                if (Doxess.Web.WorkFlow.Util.IsNumeric(req.Cookies[cookieName].Value))
                {
                    return Convert.ToInt32(req.Cookies[cookieName].Value);
                }
                return 0;
            }
            return 0;
        }

        public static void GoBack(System.Web.UI.Page page)
        {
            int retPage = GetIntCookie(page.Request, "RetPage");
            if (retPage == 0) retPage = 100;
            GotoPage(page, retPage);
        }

        public static void GotoPage(System.Web.UI.Page page, int pid)
        {
            page.Response.Cookies.Set(new HttpCookie("RetPage", GetIntCookie(page.Request, "PageID").ToString()));
            page.Response.Cookies.Set(new HttpCookie("PageID", pid.ToString()));
            string url = Doxess.Web.WorkFlow.Util.dxGetAppRootUrl(page.Request) + Security.wfGetPageUrl(pid, 0, 0);
            page.Response.Redirect(url);
            page.Response.End();
        }



        public static int GetTaskPageID(Page page, int wfId)
        {
            Doxess.Web.WorkFlow.wfMaster master = new Doxess.Web.WorkFlow.wfMaster(wfId);
            int processId = master.ProcessId;


            int userId = Convert.ToInt32(page.User.Identity.Name);
            Doxess.Web.WorkFlow.wfUserPreference userPref = new Doxess.Web.WorkFlow.wfUserPreference(userId, processId);
            DataAccess dba = new DataAccess();
            dba.dxAddParameter("@userId", userId);
            dba.dxAddParameter("@processId", processId);
            int pageId = dba.dxGetSPInt("wfGetTabPageId");
            if (pageId == 165 || pageId == 175)
            {
                if (userPref.DefImageTabView.Equals("T"))
                {
                    return 185;
                }
            }
            return pageId;
        }

        public static string DefDateFormat()
        {
            string strDefDateFormat = string.Empty;
            strDefDateFormat = System.Configuration.ConfigurationSettings.AppSettings["defDateFormat"].ToString();
            if (strDefDateFormat == string.Empty)
                strDefDateFormat = "MM/dd/yyyy";
            return strDefDateFormat;
        }

        public static string DefCulZone()
        {
            string strDefCulzone = string.Empty;
            strDefCulzone = System.Configuration.ConfigurationSettings.AppSettings["defCulZone"].ToString();
            if (strDefCulzone == string.Empty)
                strDefCulzone = "en-US";
            return strDefCulzone;
        }

        public static string GetDefPassword()
        {
            try
            {
                DataAccess dba = new DataAccess();
                return dba.dxGetTextData("select DefPassword from wfSysRec");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string AppName
        {
            //get { return System.Configuration.ConfigurationSettings.AppSettings["AppName"].ToString().Replace("/", "").Trim(); }
            get { return "StrWebFlow"; }
        }

        public static string UserControlPath
        {
            get { return @"\" + Global.AppName + @"\UserControls\"; }
        }

        public static void ctxTable(ref DataTable loaded, string tableName, string fieldname, string valuename)
        {
            string Culture = CultureInfo.CurrentCulture.Name;
            if (Culture.Substring(0, 2) != "en")
            {
                DataAccess dba = new DataAccess();
                // if there are entries for this table/collumn/culture combination, we'll use them
                if (dba.dxGetIntData("SELECT count(*) from [" + tableName + "] where [" + Culture + "-" + fieldname + "] > ''") > 0)
                {
                    string item;
                    foreach (DataRow row in loaded.Rows)
                    {
                        item = dba.dxGetTextData("SELECT ([" + Culture + "-" + fieldname + "]) From [" + tableName + "] where " + valuename + " = '" + row[1].ToString() + "' ");
                        row[0] = item; if (item == "") row[0] = "ERROR";
                    }
                }
            }
        }

    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Security;
//using System.Web.SessionState;

//namespace SFLite
//{
//    public class Global : System.Web.HttpApplication
//    {

//        void Application_Start(object sender, EventArgs e)
//        {
//            // Code that runs on application startup

//        }

//        void Application_End(object sender, EventArgs e)
//        {
//            //  Code that runs on application shutdown

//        }

//        void Application_Error(object sender, EventArgs e)
//        {
//            // Code that runs when an unhandled error occurs

//        }

//        void Session_Start(object sender, EventArgs e)
//        {
//            // Code that runs when a new session is started

//        }

//        void Session_End(object sender, EventArgs e)
//        {
//            // Code that runs when a session ends. 
//            // Note: The Session_End event is raised only when the sessionstate mode
//            // is set to InProc in the Web.config file. If session mode is set to StateServer 
//            // or SQLServer, the event is not raised.

//        }

//    }
//}
