using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Doxess.Data;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace SFLite
{
    public partial class Claims : System.Web.UI.Page
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
        public string cat
        {
            get { return (string)ViewState["cat"]; }
            set { ViewState["cat"] = value; }
        }
        private int SubProcessId { get { return (int)ViewState["SubProcessId"]; } set { ViewState["SubProcessId"] = value; } }

        public int anchorLink = 0;
        string passNm = "";
        DataAccess dba = new DataAccess();

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
                this.userName = dba.dxGetTextData("select FirstName+' '+LastName from wfUsers where userid=" + this.userID.ToString());
                if (cat == "20")
                    this.userName = dba.dxGetTextData("select resname from schresources where resindexid=(select accountid from wfusers where userid=" + this.userID.ToString() + ") and rescatid=20");
                
                passNm = this.userName;
                if (this.LiteROLE == "P") this.userName = "Active Claims - " + this.userName;
                if (this.LiteROLE == "C")
                {
                    this.userName = "Scheduled Jobs - " + this.userName;
                    Page.Master.FindControl("mpSrch").Visible = false;
                    HtmlAnchor a1 = (HtmlAnchor)Page.Master.FindControl("action1");
                    a1.InnerText = "Scheduled Jobs";
                }
            }
        }

        public void getActiveClaims()
        {
            string indexCards = "";
            if (this.LiteROLE == "C") indexCards = getCrewlist(); else indexCards = getPMlist();
            Response.Write(indexCards);
        }
        public string getPMlist()
        {
            string indexCards = "";
            DataTable index = new DataTable();
            if (userID == 15283 || userID == 15248||userID==21095)
            {
                index = dba.dxGetTable("oGetJobIndex 'CV.ProjManagerId in (15283,15248,21095)', 1");
            }
            else
            {
                index = dba.dxGetTable("oGetJobIndex 'CV.ProjManagerId=" + userID.ToString() + "', 1");
            }
            foreach (DataRow row in index.Rows)
            {
                string fil = row["FileNo"].ToString().Trim().Replace("-E", "E").Replace("-R", "R").Replace("-FS","FS");
                this.SubProcessId = 1;
                if (row["FileNo"].ToString().IndexOf("-R") > 0) this.SubProcessId = 2;

                int DocCount = dba.dxGetIntData("select count(*) from strfilesdet where wfid=" + row["WfId"].ToString()+" and FileId not in(22,7,30)");
                string upDocsPic = "Images/upDocs";

                string cli = Server.UrlEncode(row["ClientName"].ToString());
                string Params = "&wfid=" + row["WfId"].ToString() + "&fi=" + fil + "&cl=" + cli + "&uid=" + this.userID.ToString() + "&cat=" + cat;

                string onclick="'goDocs(\"" + Params + "\");'";
                if (DocCount == 0)
                {
                    upDocsPic = upDocsPic + ".png";
                }
                else if (DocCount < 6)
                {
                    upDocsPic = upDocsPic + DocCount.ToString() + ".jpg";
                    onclick = "'goViewDocs(\"" + Params + "\");'";
                }
                else
                {
                    upDocsPic = upDocsPic + "6.jpg";
                    onclick = "'goViewDocs(\"" + Params + "\");'";
                }

                string prPic = "Images/pr_report.png";
                string PRComp = row["PRComp"].ToString();
                if (PRComp.Substring(0, 1) == "1") prPic = "Images/pr_report_comp.png";
                if (PRComp.Substring(PRComp.Length-1)=="1" && SubProcessId==2) prPic= "Images/Fill.png";
                //if (Convert.ToInt32(row["PrComp"].ToString()) > 0) prPic = "Images/pr_report_comp.png";


                indexCards += "<div style='margin: .8rem .4rem 0 .4rem;'>";
                indexCards += "  <div class='ui card'>";
                indexCards += "    <div class='content'>";
                indexCards += "      <div class='header'>";
                indexCards += "        <div class='ui floated hlabel'>" + fil + "<br>" + row["ClientName"] + "</div>";
                indexCards += "        <img class='right floated mini ui image' src='Images/upPics.png' onclick='goRooms(\"" + Params + "\");'/>";
                indexCards += "        <img class='right floated mini ui image' src="+upDocsPic+" onclick=" +onclick + "' />";//'goDocs(\"" + Params + "\");' />";
                if (prPic != "Images/Fill.png")
                {
                    indexCards += "        <img class='right floated mini ui image' src='" + prPic + "' onclick='goPR(\"" + Params + "\");' />";
                }
                indexCards += "      </div>";
                indexCards += "    </div>";
                indexCards += "    <div class='extra content'>";
                indexCards += "      <div class='ui floated hlabel'>";
                string adr = row["LossAddress"].ToString();
                string adr1 = adr.Substring(0, adr.IndexOf(",")).Trim();
                string adr2 = adr.Substring(adr.IndexOf(",") + 1).Trim(); if (adr2.Substring(0, 1) == ",") adr2 = adr2.Substring(1);
                indexCards += row["ClName"].ToString() + "<br>" + adr1 + "<br>" + adr2;

                DataTable cdata = dba.dxGetTable("select ContactTelDesc + ': ' + Tel + isnull(LTRIM(' '+Extension),'') as phones "
                    + "from StrContactTel where ContactId=(select ClntContact_1Id from Claims where wfid=" + row["WfId"].ToString() + ")");
                for (int i = 0; i < cdata.Rows.Count; i++)
                {
                    indexCards += "<br><a href='#' onclick='callThis(\"" + cdata.Rows[i]["phones"].ToString() + "\");'><img src='Images/Handset.png' width='14px' />&nbsp;" + cdata.Rows[i]["phones"].ToString() + "</a>";
                }

                indexCards += "   </div><img class='right floated mini ui image' src='Images/Directions.png' onclick='getDirections(\"" + row["ClaimValueId"].ToString() + "|" + row["LossAddress"].ToString() + "\");'/>";
                indexCards += "    </div>";
                indexCards += "    <div class='extra content'>";
                indexCards += "      <div class='ui floated hlabel'>";
                indexCards += row["InsName"].ToString() + " - " + row["InsAdjuster"].ToString();
                
                
                cdata = dba.dxGetTable("select top 1 rtrim(StrContactTel.ContactTelDesc + ': ' + StrContactTel.Tel + ' ' + isnull(StrContactTel.Extension,'')) as phones "
                    + "from StrContactTel where StrContactTel.ContactId = (select contactId from Adjusters where AdjusterId = "
                    + "(select InsAdjusterId from ClaimValues where wfid=" + row["WfId"].ToString() + " and subprocessid=" + this.SubProcessId.ToString() + "))");
                for (int i = 0; i < cdata.Rows.Count; i++)
                {
                    indexCards += "<br><a href='#' onclick='callThis(\"" + cdata.Rows[i]["phones"].ToString() + "\");'><img src='Images/Handset.png' width='14px' />&nbsp;" + cdata.Rows[i]["phones"].ToString() + "</a>";
                }
          
      
                indexCards += "   </div>";
                indexCards += "    </div>";
                indexCards += "  </div>";
                indexCards += "</div>";
            }
            return indexCards;
        }
        public string getCrewlist()
        {
            string dt = "";
            string cc = "blue";
            string indexCards = "";
            DateTime anchorDate;
            int anchor = 0;
            anchorLink = 0;
            anchorDate = DateTime.Now;  //Convert.ToDateTime("2016-06-09");
            DataTable index = new DataTable();
            index = dba.dxGetTable("oGetJobIndexCrew " + userID.ToString() + ", " + this.cat);
            foreach (DataRow row in index.Rows)
            {
                if (row["FileNo"].ToString() == "900006" && cat == "20")
                {
                }
                else
                {

                    string PMName = dba.dxGetTextData("select FirstName+' '+LastName from wfUsers where userid=" + row["PMId"].ToString());
                    string PMPhone = dba.dxGetTextData("select BusCellPhone from Employees where userid=" + row["PMId"].ToString());
                    int DocCount = dba.dxGetIntData("select count(*) from strfilesdet where uploadbyid=" +userID + " and wfid=" + row["WfId"].ToString()+" and FileId not in(22,7,30)");
                    string upDocsPic= "Images/upDocs";
                    string cli = Server.UrlEncode(row["ClientName"].ToString());
                    string fil = row["FileNo"].ToString().Trim().Replace("-E", "E").Replace("-R", "R").Replace("-FS", "FS");
                    string sch = row["SchId"].ToString();
                    string Params = "&wfid=" + row["WfId"].ToString() + "&fi=" + fil + "&cl=" + cli + "&sc=" + sch + "&uid=" + this.userID.ToString() + "&cat=" + cat;

                    string onclick = "'goDocs(\"" + Params + "\");'";
                    if (DocCount == 0)
                    {
                        upDocsPic = upDocsPic + ".png";
                    }
                    else if (DocCount < 6)
                    {
                        upDocsPic = upDocsPic + DocCount.ToString() + ".jpg";
                        onclick = "'goViewDocs(\"" + Params + "\");'";
                    }
                    else
                    {
                        upDocsPic = upDocsPic + "6.jpg";
                        onclick = "'goViewDocs(\"" + Params + "\");'";
                    }


                    //if (cat == "10")
                    //{
                    //    onclick = "'goDocs(\"" + Params + "\");'";
                    //}
                    
                    this.SubProcessId = 1;
                    if (row["FileNo"].ToString().IndexOf("-R") > 0) this.SubProcessId = 2;

                    string prPic = "Images/pr_report.png";
                    string PRComp = row["PRComp"].ToString();
                    if (PRComp.Substring(0, 1) == "1") prPic = "Images/pr_report_comp.png";
                    if (PRComp.Substring(PRComp.Length - 1) == "1" && SubProcessId == 2) prPic = "Images/Fill.png";

                    if (PMPhone.Length > 0)
                    {
                        PMPhone = PMPhone.Replace("-", "").Replace("(", "").Replace(")", "");
                        PMPhone = "Cell: (" + PMPhone.Substring(0, 3) + ") " + PMPhone.Substring(3, 3) + "-" + PMPhone.Substring(6);
                    }
                    DateTime strt = Convert.ToDateTime(row["startDate"].ToString());
                    DateTime endt = Convert.ToDateTime(row["endDate"].ToString());
                    string thisdt = strt.ToShortDateString();


                    
                    
                    string docImg = "Images/Edit.png";
                    string upParm = "&up=1";
                    if ((row["TbStatusId"].ToString() == "5") || (row["TbStatusId"].ToString() == "3")) { docImg = "Images/Edited.png"; upParm = "&up=0"; }
                    if (row["TbStatusId"].ToString() == "4") { docImg = "Images/Approved.png"; upParm = "&up=2"; }
                    string upPic = "Images/upPics.png";
                    if (Convert.ToInt32(row["PhotoCount"].ToString()) > 0) upPic = "Images/upPicsDone.jpg";

                    Params += upParm;
                    if (thisdt != dt)
                    {
                        anchor += 1;
                        if ((strt.Date >= anchorDate.Date) && (anchorLink == 0))
                        {
                            anchorLink = anchor;
                            indexCards += "<a id='starthere' href='#'></a>";
                        }
                        else indexCards += "<a id='" + anchor.ToString() + "' href='#'></a>";
                        indexCards += "<div class='ui card' style='background-color:#C0C0C0'>";
                        indexCards += "    <div class='content'>";
                        indexCards += "        <div class='header'><span style='font-size:medium; color:#000000;'>"
                            + strt.ToLongDateString() + ":</span>";
                        indexCards += "        </div>";
                        indexCards += "    </div>";
                        indexCards += "</div>";
                        dt = thisdt;
                        if (cc == "blue") cc = "brown"; else cc = "blue";
                    }
                    indexCards += "<div style='margin: .8rem .4rem 0 .4rem;'>";
                    indexCards += "  <div class='ui " + cc + " card'>";
                    indexCards += "    <div class='content'>";
                    indexCards += "      <div class='header'>";
                    indexCards += "        <div class='ui floated hlabel'>" + fil + "<br>" + row["ClientName"] + "</div>";
                    indexCards += "        <img class='right floated mini ui image' src='" + upPic + "' onclick='goRooms(\"" + Params + "\");' />";
                    indexCards += "        <img class='right floated mini ui image' src=" + upDocsPic + " onclick=" + onclick + "' />";//'goDocs(\"" + Params + "\");' />";
                    //indexCards += "        <img class='right floated mini ui image' src='Images/upDocs.png' onclick='goDocs(\"" + Params + "\");' />";
                    if (Page.User.IsInRole("Crew - PM"))
                    {
                        if (prPic != "Images/Fill.png")
                        {
                            indexCards += "        <img class='right floated mini ui image' src='" + prPic + "' onclick='goPR(\"" + Params + "\");' />";
                        }
                    }
                    indexCards += "      </div>";
                    indexCards += "    </div>";
                    indexCards += "    <div class='extra content'>";
                    indexCards += "      <div class='ui floated hlabel'>";
                    string adr = row["LossAddress"].ToString();
                    string adr1 = adr.Substring(0, adr.IndexOf(",")).Trim();
                    string adr2 = adr.Substring(adr.IndexOf(",") + 1).Trim(); if (adr2.Substring(0, 1) == ",") adr2 = adr2.Substring(1);
                    indexCards += row["ClName"].ToString() + "<br>" + adr1 + "<br>" + adr2;

                    DataTable cdata = dba.dxGetTable("select ContactTelDesc + ': ' + Tel + isnull(LTRIM(' '+Extension),'') as phones "
                        + "from StrContactTel where ContactId=(select ClntContact_1Id from Claims where wfid=" + row["WfId"].ToString() + ")");
                    for (int i = 0; i < cdata.Rows.Count; i++)
                    {
                        indexCards += "<br><a href='#' onclick='callThis(\"" + cdata.Rows[i]["phones"].ToString() + "\");'><img src='Images/Handset.png' width='14px' />&nbsp;" + cdata.Rows[i]["phones"].ToString() + "</a>";
                    }

                    indexCards += "   </div><img class='right floated mini ui image' src='Images/Directions.png' onclick='getDirections(\"" + row["ClaimValueId"].ToString() + "|" + row["LossAddress"].ToString() + "\");'/>";
                    indexCards += "    </div>";

                    indexCards += "    <div class='extra content'>";
                    indexCards += "      <div class='ui floated hlabel'>";
                    //indexCards += "Project Mgr:&nbsp;&nbsp;" + PMName + "<br>" + PMPhone + "</div>";
                    //indexCards += "      <img class='right floated mini ui image' src='Images/Phone.png' onclick='callThis(\"" + PMPhone + "\");'/>";
                    if (PMPhone.Length > 0)
                    {
                        indexCards += "Project Mgr:&nbsp;&nbsp;" + PMName + "<br><a href='#' onclick='callThis(\"" + PMPhone + "\");'><img src='Images/Handset.png' width='14px' />&nbsp;" + PMPhone + "</a>";
                    }
                    indexCards += "    </div></div>";
                    indexCards += "    <div class='extra content'>";
                    indexCards += "      <img class='right floated mini ui image popped' src='" + docImg + "' ";
                    if (upParm != "") indexCards += "onclick='goEdit(\"" + Params + "\");'";
                    indexCards += "          /><div class='collapse-card'>";
                    indexCards += "        <div class='collapse-card__heading'>";
                    indexCards += "            <h5 class='collapse-card__title'>";
                    indexCards += "                &nbsp;&nbsp;<span style='color:#404040;'>" + strt.ToString("hh:mm tt") + "&nbsp;&nbsp;to&nbsp;&nbsp;" + endt.ToString("hh:mm tt") + "&nbsp;&nbsp;(" + row["hours"].ToString() + " hrs).</span>";
                    indexCards += "                <i class='fa fa-clipboard fa-2x fa-fw'></i>";
                    indexCards += "            </h5>";
                    indexCards += "        </div>";
                    indexCards += "        <div class='collapse-card__body'>";
                    indexCards += "             <p>" + row["Description"].ToString() + "</p>";
                    indexCards += "        </div>";
                    indexCards += "      </div>";
                    indexCards += "    </div>";

                    indexCards += "    </div>";
                    indexCards += "  </div>";
                    indexCards += "</div>";
                }
            }
                return indexCards;
            
            
        }
    }
}
