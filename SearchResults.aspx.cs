using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Doxess.Data;
using System.Data;
using System.Text.RegularExpressions;

namespace SFLite
{
    public partial class SearchResults : System.Web.UI.Page
    {
        private int userID
        {
            get { return (int)ViewState["userID"]; }
            set { ViewState["userID"] = value; }
        }

        private string searchTerm
        {
            get { return (string)ViewState["searchTerm"]; }
            set { ViewState["searchTerm"] = value; }
        }

        private string qry
        {
            get { return (string)ViewState["qry"]; }
            set { ViewState["qry"] = value; }
        }

        DataAccess dba = new DataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                } 
                this.searchTerm = Request.QueryString["tm"];
                int n; 
                bool isNumeric = int.TryParse(this.searchTerm, out n);
                if (isNumeric)
                {
                    qry = "oGetJobIndex 'CV.ProjManagerId=" + userID.ToString() + " and C.ClaimId=" + this.searchTerm.Trim() + "', 1";
                }
                else
                {
                    qry = "oGetJobIndex 'CV.ProjManagerId=" + userID.ToString() + " and SC.LastName like ''%" + this.searchTerm.Trim() + "%''', 2";   
                }
                this.userID = 0;
                ClaimList.Attributes.Add("style", "display: block");
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }
            }
        }

        public void getResults()
        {
            string indexCards = "";
            DataTable index = new DataTable();
            index = dba.dxGetTable(this.qry);
            if (index.Rows.Count == 0)
            {
                indexCards += "<div style='margin: .8rem .4rem 0 .4rem;'>";
                indexCards += "  <div class='ui card'>";
                indexCards += "    <div class='content'>No results found for \"" + this.searchTerm.Trim() + "\".</div>";
                indexCards += "  </div>";
                indexCards += "</div>";
            }
            else
            {
                foreach (DataRow row in index.Rows)
                {
                    string fil = row["FileNo"].ToString().Replace("-E", "E").Replace("-R", "R");
                    string cli = Server.UrlEncode(row["ClientName"].ToString());
                    string Params = "&wfid=" + row["WfId"].ToString() + "&fi=" + fil + "&cl=" + cli + "&uid=" + this.userID.ToString();
                    indexCards += "<div style='margin: .8rem .4rem 0 .4rem;'>";
                    indexCards += "  <div class='ui card'>";
                    indexCards += "    <div class='content'>";
                    indexCards += "      <div class='header'>";
                    if ((row["MastStatusId"].ToString() == "1") && (Convert.ToInt32(row["FileStatusId"].ToString()) <= 8))
                    {
                        indexCards += "        <div class='ui floated hlabel'>" + fil + "<br>" + row["ClientName"] + "</div>";
                        indexCards += "        <img class='right floated mini ui image' src='Images/upPics.png' onclick='goRooms(\"" + Params + "\");'/>";
                        indexCards += "        <img class='right floated mini ui image' src='Images/upDocs.png' onclick='goDocs(\"" + Params + "\");' />";
                    }
                    else
                    {
                        indexCards += "        <div class='ui floated d_hlabel'>" + fil + "<br>" + row["ClientName"] + "</div>";
                        indexCards += "        <img class='right floated mini ui image' src='Images/d_Inactive.png'/>";
                    }
                    indexCards += "      </div>";
                    indexCards += "    </div>";
                    indexCards += "    <div class='extra content'>";
                    indexCards += "      <div class='ui floated hlabel'>";
                    string adr = row["ClAddress"].ToString();
                    string adr1 = adr.Substring(0, adr.IndexOf(",") - 1).Trim();
                    string adr2 = adr.Substring(adr.IndexOf(",") + 1).Trim(); if (adr2.Substring(0, 1) == ",") adr2 = adr2.Substring(1);
                    indexCards += row["ClName"].ToString() + "<br>" + adr1 + "<br>" + adr2 + "<br>" + row["ClPhone"].ToString() + "</div>";
                    indexCards += "      <img class='right floated mini ui image' src='Images/Phone.png' onclick='callThis(\"" + row["ClPhone"].ToString() + "\");'/>";
                    indexCards += "      <img class='right floated mini ui image' src='Images/Directions.png' onclick='getDirections(\"" + row["ClaimValueId"].ToString() + "|" + row["ClAddress"].ToString() + "\");'/>";
                    indexCards += "    </div>";
                    indexCards += "    <div class='extra content'>";
                    indexCards += "      <div class='ui floated hlabel'>";
                    indexCards += row["InsName"].ToString() + " - " + row["InsAdjuster"].ToString() + "<br>" + row["AdjTel"].ToString() + "</div>";
                    indexCards += "      <img class='right floated mini ui image' src='Images/Phone.png' onclick='callThis(\"" + row["AdjTel"].ToString() + "\");'/>";
                    indexCards += "    </div>";
                    indexCards += "  </div>";
                    indexCards += "</div>";
                }
            }
            Response.Write(indexCards);
        }
    }
}
