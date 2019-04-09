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
    public partial class PickRooms : System.Web.UI.Page
    {
        public DataTable index = new DataTable();
        public DataTable rms = new DataTable();

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
        private string schid
        {
            get { return (string)ViewState["schid"]; }
            set { ViewState["schid"] = value; }
        }
        private string adds
        {
            get { return (string)ViewState["adds"]; }
            set { ViewState["adds"] = value; }
        }

        DataAccess dba = new DataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.adds = "0";
                if (Request.Cookies["userID"] != null)
                {
                    this.userID = Convert.ToInt32(this.Request.Cookies["userID"].Value);
                }
                if (Request.Cookies["MYLITEROLE"] != null)
                {
                    this.LiteROLE = this.Request.Cookies["MYLITEROLE"].Value.ToString();
                }
            }

            this.WfId = Convert.ToInt32((string)Request.QueryString["wfid"]);
            this.userID = Convert.ToInt32((string)Request.QueryString["uid"]);
            this.schid = Request.QueryString["sc"];
            this.file = Request.QueryString["fi"];
            string tgt = Request["__EVENTTARGET"];
            string arg = hf1.Value.ToString();

            if (tgt == "allRooms") 
            {
                if (this.adds == "0") this.adds = arg; else this.adds += ":" + arg;
            }
        }

        public void getFile()  // get rooms used or added this session
        {
            Response.Write(this.file);
        }

        public void getRooms()  // get rooms used or added this session
        {
            Response.Write(buildRooms());
        }

        public string buildRooms()
        {
            string cul=dba.dxGetTextData("Select Culture from wfuserpref where userid=" + userID);
            
            string indexCards = "";
            if (cul=="en")
            {
            index = dba.dxGetTable("SELECT id, item from wfOptions where opType = 'rooms' and id in "
                + "(Select distinct location from StrImagesDetExtra where keyid in (Select keyId from StrImagesDet where wfid = " + this.WfId.ToString() + ")) order by item");
            }
            else
            {
            index = dba.dxGetTable("SELECT id, item_fr as item from wfOptions where opType = 'rooms' and id in "
                + "(Select distinct location from StrImagesDetExtra where keyid in (Select keyId from StrImagesDet where wfid = " + this.WfId.ToString() + ")) order by item");
            }
            foreach (DataRow row in index.Rows)
            {
                int picCount = dba.dxGetIntData("Select count(*) from StrImagesDetExtra where keyid in (Select keyId from StrImagesDet where wfid = " + this.WfId.ToString() + ") and location= " + row["id"].ToString());
                indexCards += addIndexCard(row["id"].ToString(), row["item"].ToString(), picCount);
            }
            if (this.adds != "0")
            {
                string[] items = this.adds.Split(':');
                for (int ix = 0; ix < items.Length; ix++)
                {
                    string[] room = items[ix].Split('~');
                    indexCards += addIndexCard(room[0], room[1], 0);
                }
            }
            return indexCards;
        }

        public string addIndexCard(string id, string item, int picCount)
        {
            string Card = "";
            string loc = item.Trim().Replace(" ", "<br>");
            string Params = "&wfid=" + this.WfId.ToString() + "&fi=" + this.file + "&uid=" + this.userID.ToString() + "&sc=" + schid + "&rm=" + id;// +"&loc=" + item;
            Card += "<div class='card'>";
            Card += "     <div class='image' onclick='goViewer(\"" + Params + "&loc=" + item + "\");'>";
            Card += "         <img class='center floated mini ui image' src='Images/Folder.png' />";
            Card += "     </div>";
            if (picCount > 0) Card += "<div class='cdlbl' onclick='goViewer(\"" + Params + "&loc=" + item + "\");'><b>" + loc + "</b><br><br>" + picCount.ToString() + " photos</div>";
            else Card += "<div class='cdlbl' onclick='goViewer(\"" + Params + "&loc=" + item + "\");'><b>" + loc + "</b><br><br>(0 photos)</div>";
            Card += "  <div class='extra'>";
            Card += "     <div class='ui bottom attached button' onclick='goUploader(\"" + Params + "\");'>Add photo</div>";
            Card += "  </div>";
            Card += "</div>";

            return Card;
        }
        
        public void addRooms()  // populate the drop down with the (as yet unused portion) room list
        {
            string cul = dba.dxGetTextData("Select Culture from wfuserpref where userid=" + userID);
            string indexCards = "";
            if (cul == "en")
            {
                rms = dba.dxGetTable("SELECT id, item from wfOptions where opType = 'rooms' and id > 0 and id not in"
                    + "(Select distinct location from StrImagesDetExtra where keyid in (Select keyId from StrImagesDet where wfid = " + this.WfId.ToString() + ")) order by item");
            }
            else
            {
                rms = dba.dxGetTable("SELECT id, item_fr as item from wfOptions where opType = 'rooms' and id > 0 and id not in" + "(Select distinct location from StrImagesDetExtra where keyid in (Select keyId from StrImagesDet where wfid = " + this.WfId.ToString() + ")) order by item");
            }
            
            indexCards += "<div class='card'>";
            indexCards += "  <div class='image'>";
            indexCards += "    <img class='center floated mini ui image' src='Images/AddFolder.png' />";
            indexCards += "  </div>";
            indexCards += "  <div class='extra group'>";
            indexCards += "    <div class='ui scrolling dropdown'>";
            indexCards += "      <div class='text'>Select</div>";
            indexCards += "      <i class='dropdown icon'></i>";
            indexCards += "      <div id='roomOpts' class='menu'>";
            foreach (DataRow row in rms.Rows)
            {
                indexCards += "        <div class='item' onclick='RoomsCallBack(\"" + row["id"].ToString() + "~" + row["item"].ToString() + "\");'>" + row["item"].ToString() + "</div>";
            }
            indexCards += "      </div>";
            Response.Write(indexCards);
        }


    }
}
