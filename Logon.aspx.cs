using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using Doxess.Web.WorkFlow.Security;
using Doxess.Data;
using System.Web;

namespace SFLite
{
    public partial class Logon : System.Web.UI.Page
    {
        bool aliasedLogin = false;
        string roleType = "A";
        string vendor = "";

        string WfId;
        string SubId;
        string SchId;

        string id = string.Empty; //"sysadmin1";
        string pw = string.Empty; //"london";
        private int userId;
        private string[] safe_args = {"", "", ""};

        public string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }

        DataAccess dba = new DataAccess();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here

            txtPass.Attributes.Add("style", "display: none");
            WfId = Request.QueryString["wfid"]; if (WfId == null) WfId = "";
            SubId = Request.QueryString["su"]; if (SubId == null) SubId = "";
            SchId = Request.QueryString["sc"]; if (SchId == null) SchId = "";

            this.lblMessage.Text = "";

            if (!this.IsPostBack)
            {
                if (Request.ServerVariables["HTTP_USER_AGENT"] != null &&
                    Request.ServerVariables["HTTP_USER_AGENT"].ToString().IndexOf("MSIE") > -1)
                    Response.AddHeader("X-UA-Compatible", "IE=9");

                string QS = Request.QueryString["ai"]; if (QS == null) QS = "";
                if (QS != "")
                {
                    string decode = dba.dxGetTextData("SELECT dbo.Unencrypt('" + QS + "')");        // decrypted format:  safe:<LoginCode>:<target page>
                    safe_args = decode.Split(':');
                    if (safe_args[0] == "safe")
                    {
                        id = safe_args[1];
                        this.txtUserId.Value = id;
                        pw = dba.dxGetTextData("select Password from wfUsers where LoginCode = '" + id + "'");
                        this.txtPass.Value = pw;
                        btnLogon_Click();
                    }
                }
            }
            else
            {
                btnLogon_Click();
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        private string RemoteIP
        {
            get
            {
                return this.Request.ServerVariables["REMOTE_ADDR"];
            }

        }

        protected void btnLogon_Click()
        {
            if (this.txtPass.Value.Trim() == string.Empty)
            {
                return;
            }

            string ip = this.RemoteIP;
            string logUser = this.txtUserId.Value.ToString().Trim();
            string logAlias = "";

            EncryptManager enm = new EncryptManager();
            if (safe_args[0] == "") pw = enm.Encrypt(this.txtPass.Value.Trim());

            if (logUser.IndexOf("@") > 0)
            {
                vendor = logUser.Substring(0, logUser.IndexOf("@"));
                logUser = logUser.Substring(logUser.IndexOf("@") + 1);
                if ((vendor.IndexOf(".") > 0) || (vendor.ToLower() == "sysadmin1"))
                {
                    logAlias = vendor;
                    logUser = logUser.Substring(logUser.IndexOf("@") + 1);
                    this.aliasedLogin = true;
                    vendor = "";
                }
                else
                {
                    this.aliasedLogin = false;
                }
            }

            //if (logUser.IndexOf("@") > 0)
            //{
            //    logAlias = logUser.Substring(0, logUser.IndexOf("@"));
            //    logUser = logUser.Substring(logUser.IndexOf("@") + 1);

            //    this.aliasedLogin = true;
            //}
            //else
            //{
            //    this.aliasedLogin = false;
            //}

            if (logAlias != "")
            {
                if (!this.isSysAdmin(logAlias))
                {
                    this.lblMessage.Text = "Incorrect username or password combination.";
                    this.reportFailedLogon();
                    return;
                }
            }

            dba.dxAddParameter("@SALoginCode", logUser);
            userId = dba.dxGetIntData("SELECT UserId FROM wfUsers WHERE LoginCode = @SALoginCode");
            this.Response.Cookies.Add(new HttpCookie("userID", this.userId.ToString()));

            if (this.isSysAdmin(logAlias))
            {
                // System Administrators can log in as another user using a special login name
                // and their own password
                dba.dxAddParameter("@SALoginCode", logAlias);
                string sysAdminPW = dba.dxGetTextData("SELECT Password FROM wfUsers WHERE LoginCode = @SALoginCode");
                //string sysAdminPW = dba.dxGetTextData("SELECT Password FROM wfUsers WHERE LoginCode = '" + logAlias + "'");

                EncryptManager enm2 = new EncryptManager();
                string sysAdminPWEntered = enm2.Encrypt(this.txtPass.Value.Trim());

                if (sysAdminPW != sysAdminPWEntered)
                {
                    this.lblMessage.Text = "Incorrect username or password combination.";
                    this.reportFailedLogon();
                    return;
                }

                dba.dxClearParameters();
                dba.dxAddParameter("@LoginCode", logUser);

                try
                {
                    pw = dba.dxGetTextData("SELECT Password FROM wfUsers WHERE LoginCode = @LoginCode");
                }
                catch
                {
                    this.lblMessage.Text = "User ID not found";
                    return;
                }

                if (Security.wfLogin(logUser, pw, this.GetTimeout, Response))
                {
                    this.Request.Cookies.Remove("WfId");
                    this.Request.Cookies.Remove("detId");
                    this.Request.Cookies.Remove("userID");

                    this.Request.Cookies.Remove("PageID");
                    this.Request.Cookies.Remove("taskId");
                    this.Request.Cookies.Remove("RetPage");
                    this.Request.Cookies.Remove("ByGroupType");
                    this.Request.Cookies.Remove("pageIndex");
                    this.Request.Cookies.Remove("fileNo");
                    this.Request.Cookies.Remove("BranchSetting");
                    this.Request.Cookies.Remove("PmSetting");


                    this.SaveLogonInfo(logUser, ip, true);
                    SetClientSideCredential();
                    Session["logon"] = null;


                    //==== START of auto redirections =====
                    if ((safe_args[0] == "") && (this.roleType == "A")) Response.Redirect("https://www.mysmartflow.ca/strwebflow/logon.aspx?ai=" + logUser + "&ap=" + this.txtPass.Value.Trim());
                    if (safe_args[2] != "")
                    {
                        string url = Request.Url.ToString();
                        url = url.Substring(0, url.IndexOf("&ai=") - 1);
                        url = url.Replace("Logon.aspx", safe_args[2] + ".aspx").Replace("logon.aspx", safe_args[2] + ".aspx");
                        string jobName = Server.UrlEncode(dba.dxGetTextData("select JobName from claims where wfid=" + Request.QueryString["wfid"]));
                        string fileName = dba.dxGetTextData("select FileNo from claims where wfid=" + Request.QueryString["wfid"]);
                        if (Request.QueryString["su"] == "1") fileName += "E"; else fileName += "R";
                        url += "&fi=" + fileName + "&cl=" + jobName + "&uid=" + userId.ToString() + "&up=1";
                        this.cat = "10"; if (vendor != "") this.cat = "20";
                        this.Response.Cookies.Add(new HttpCookie("CatID", this.cat.ToString()));
                        Response.Redirect(url);
                    }
                    //==== END of auto redirections =====


                    if (!checkPasswordExpiry(logUser, pw))  // make sure credentials are not expired
                    {
                        CreateCookie("BranchSetting");
                        CreateCookie("PmSetting");
                        string qryparams = "?cat=";
                        if (vendor != "") qryparams += "20"; else qryparams += "10";
                        if (this.cat == "20")
                        {
                            Security.wfRedirect(this.txtUserId.Value.Trim(), "Claims.aspx" + qryparams, Response);
                        }
                        else
                        {
                            Security.wfRedirect(this.txtUserId.Value.Trim(), System.Configuration.ConfigurationSettings.AppSettings["HomePage"] + qryparams, Response);
                        }

                    }
                    else
                    {
                        this.lblMessage.Text = "Smartflow password has expired.";
                        this.reportFailedLogon();
                        return;
                    }

                    return;
                }
            }
            else  //all others but SysAdmin
            {
                if (!this.isActiveUser(logUser))
                {
                    this.lblMessage.Text =  "Incorrect username or password combination.";
                    this.reportFailedLogon();

                    return;
                }

                if  ((vendor != "") || (Security.wfLogin(logUser, pw, this.GetTimeout, Response)) )
                {
                    this.Request.Cookies.Remove("WfId");
                    this.Request.Cookies.Remove("detId");
                    this.Request.Cookies.Remove("userID");

                    this.Request.Cookies.Remove("PageID");
                    this.Request.Cookies.Remove("taskId");
                    this.Request.Cookies.Remove("RetPage");
                    this.Request.Cookies.Remove("ByGroupType");
                    this.Request.Cookies.Remove("pageIndex");
                    this.Request.Cookies.Remove("fileNo");

                    this.Request.Cookies.Add(new HttpCookie("BranchSetting", ""));
                    this.Request.Cookies.Add(new HttpCookie("PmSetting", ""));

                    this.SaveLogonInfo(logUser, ip, true);
                    SetClientSideCredential();
                    Session["logon"] = null;


                    //==== START of auto redirections =====
                    if ((safe_args[0] == "") && (this.roleType == "A")) Response.Redirect("https://www.mysmartflow.ca/strwebflow/logon.aspx?ai=" + logUser + "&ap=" + this.txtPass.Value.Trim());
                    if (safe_args[2] != "")
                    {
                        string url = Request.Url.ToString();
                        url = url.Substring(0, url.IndexOf("&ai="));
                        url = url.Replace("Logon.aspx", safe_args[2] + ".aspx").Replace("logon.aspx", safe_args[2] + ".aspx");
                        string jobName = Server.UrlEncode(dba.dxGetTextData("select JobName from claims where wfid=" + Request.QueryString["wfid"]));
                        string fileName = dba.dxGetTextData("select FileNo from claims where wfid=" + Request.QueryString["wfid"]);
                        if (Request.QueryString["su"] == "1") fileName += "E"; else fileName += "R";
                        url += "&fi=" + fileName + "&cl=" + jobName + "&uid=" + userId.ToString() + "&up=1";
                        this.cat = "10"; if (vendor != "") this.cat = "20";
                        this.Response.Cookies.Add(new HttpCookie("CatID", this.cat.ToString()));

                        Response.Redirect(url);
                    }
                    //==== END of auto redirections =====


                    if (!checkPasswordExpiry(logUser, pw))  // make sure credentials are not expired
                    {
                        CreateCookie("BranchSetting");
                        CreateCookie("PmSetting");
                        string qryparams = "?cat=";
                        if (vendor != "") qryparams += "20"; else qryparams += "10";
                        if (qryparams == "?cat=20")
                        {
                            this.Response.Cookies.Add(new HttpCookie("CatID", "20"));
                            Security.wfRedirect(this.txtUserId.Value.Trim(), "Claims.aspx" + qryparams, Response);
                            
                        }
                        else
                        {
                            Security.wfRedirect(this.txtUserId.Value.Trim(), System.Configuration.ConfigurationSettings.AppSettings["HomePage"] + qryparams, Response);
                        }
                    }
                    else
                    {
                        this.lblMessage.Text = "Smartflow password has expired.";
                        this.reportFailedLogon();
                        return;
                    }
                    return;
                }
            }

            SetClientSideCredential();
            this.SaveLogonInfo(logUser, ip, false);
            CheckLogon();
            this.lblMessage.Text = "Incorrect username or password combination.";
            string focus = "<Script>document.getElementById('" + this.txtPass.ID + "').focus();</Script>";
            Page.RegisterStartupScript("password", focus);
        }

        private void SaveLogonInfo(string logonId, string ip, bool success)
        {
            //Security.						
            int rev = Doxess.Web.WorkFlow.Security.wfLoginTrack.wfSaveLogonInfo(logonId, ip, success);
        }

        private bool checkPasswordExpiry(string logonName, string password)
        {
            bool bolRetValue = false;
            Object objDate = Doxess.Web.WorkFlow.Security.WfUser.wfGetPassExpiryDate(logonName, password);
            if (objDate != null && objDate != DBNull.Value)
            {
                DateTime PWExpiryDate = (DateTime)objDate;
                if (PWExpiryDate <= DateTime.Today.Date.AddDays(1))
                    bolRetValue = true;
            }
            else
                bolRetValue = true;
            return bolRetValue;
        }

        private void CheckLogon()
        {
            // if logon 
            if (Session["logon"] == null)
            {
                Session["logon"] = 1;
            }
            else
            {
                int logonTime = (int)Session["logon"] + 1;
                int allowLogon = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["AllowLogonTime"]);
                if (logonTime >= allowLogon)
                {
                    // Capture the user name and IP (if possible) that 
                    // has failed whilst attemping to log in. Send an
                    // email to the system administrator with this information
                    this.reportFailedLogon();

                    Session.Abandon();
                    Response.Redirect("public/Unauthorized.aspx");
                }
                else
                {
                    Session["logon"] = logonTime;
                }
            }
        }

        /*
         * Get Timeout from configure file
        */

        private void SetClientSideCredential()
        {
            EncryptManager enm = new EncryptManager();

            string lnm = this.txtUserId.Value.Trim();
            if (lnm.IndexOf("@") > 0) lnm = lnm.Substring(lnm.IndexOf("@") + 1);
            userId = 0;
            dba.dxClearParameters();
            dba.dxAddParameter("@LoginCode", lnm);

            userId = dba.dxGetIntData("SELECT UserId FROM wfUsers WHERE LoginCode = @LoginCode");
            if (vendor != "") roleType = "C";
            else
            {
                roleType = "A";
                int isCrew = dba.dxGetIntData("SELECT count(*) FROM [Strwebflow].[dbo].[wfUsersRoles] where userid=" + userId.ToString() + " and roleid in (290,295)");
                if (isCrew > 0) roleType = "C";
                int isPM = dba.dxGetIntData("SELECT count(*) FROM [Strwebflow].[dbo].[wfUsersRoles] where userid=" + this.userId.ToString() + " and roleid in (211)");
                if (isPM > 0) roleType = "P";
            }
            HttpCookie idCookie = new HttpCookie("MYSTRONEWFID", this.txtUserId.Value.Trim());
            HttpCookie pwCookie = new HttpCookie("MYSTRONEWFPASSWORD", enm.Encrypt(this.txtPass.Value.Trim()));
            HttpCookie roleCookie = new HttpCookie("MYLITEROLE", roleType);
            idCookie.Expires = DateTime.Today.AddDays(30);
            pwCookie.Expires = DateTime.Today.AddDays(30);
            roleCookie.Expires = DateTime.Today.AddDays(30);
            this.Response.Cookies.Add(idCookie);
            this.Response.Cookies.Add(pwCookie);
            this.Response.Cookies.Add(roleCookie);
        }
        private int GetTimeout
        {
            get
            {
                string timeOut = System.Configuration.ConfigurationSettings.AppSettings["LogonTimeout"];
                try
                {
                    return int.Parse(timeOut);
                }
                catch (Exception ex)
                {
                    return 60;   // Default timeout
                }
            }
        }

        private bool isSysAdmin(string loguser)
        {
            userId = 0;
            dba.dxClearParameters();
            dba.dxAddParameter("@LoginCode", loguser);
            userId = dba.dxGetIntData("SELECT UserId FROM wfUsers WHERE LoginCode = @LoginCode");

            if (dba.dxGetIntData("SELECT COUNT(*) FROM wfUsersRoles WHERE UserId = " + userId.ToString() + " AND RoleId = 100") > 0)
            {
                return true;
            }

            return false;
        }
        private bool isAdjuster(string loguser)
        {
            userId = 0;
            dba.dxClearParameters();
            dba.dxAddParameter("@LoginCode", loguser);
            userId = dba.dxGetIntData("SELECT UserId FROM wfUsers WHERE LoginCode = @LoginCode");
            if (dba.dxGetIntData("SELECT COUNT(*) FROM wfUsersRoles WHERE UserId = " + userId.ToString() + "  AND RoleId = 280") > 0)
            {
                return true;
            }

            return false;
        }
        private bool isActiveUser(string loguser)
        {
            int isActive = 0;
            dba.dxClearParameters();
            dba.dxAddParameter("@LogUser", loguser);
            isActive = dba.dxGetIntData("SELECT IsActive FROM wfUsers WHERE LoginCode = @LogUser");

            if (isActive == 1)
            {
                return true;
            }

            return false;
        }

        public string ipAddress()
        {
            // Obtains the IP address of the current user

            string strIpAddress;
            strIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (strIpAddress == null)
            {
                strIpAddress = Request.ServerVariables["REMOTE_ADDR"];
            }

            return strIpAddress;
        }

        public string remoteHost()
        {
            // Obtains the Remote Hostname

            try
            {
                System.Net.IPHostEntry host;
                host = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]);
                return host.HostName;
            }
            catch
            {
                return "HostName Not Available";
            }
        }

        private void reportFailedLogon()
        {
            lblMessage.Text = "Incorrect User ID / Password.";
            txtUserId.Value = "";
            txtPass.Value="";
        }

        private void CreateCookie(string CookieName)
        {
            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Value = "";
            cookie.Expires = DateTime.Now.AddHours(3);
            Response.Cookies.Add(cookie);
        }

    }
}

