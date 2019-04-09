<%@ Page Title="" Language="C#" MasterPageFile="~/Image.Master" AutoEventWireup="True" CodeBehind="Uploader.aspx.cs" Inherits="SFLite.Uploader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link href="Styles/paper-collapse.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/paper-collapse.js" type="text/javascript"></script>
    <script src="Scripts/telephoneFormat.js" type="text/javascript"></script>

    <!-- Content, Style overrides, JS HERE !-->
<style>
        .last.container {
        margin-bottom: 300px !important;
        }
        h4.ui.center.header {
        margin-top: 1em;
        }
        .content { border: 1px; }
        .ui.cards > .card,
        .ui.card {
          width: 99%;
        }
        .popped {
          z-index: 500;
        }
        .ui.floated.hlabel {
          float: left;
        }

        h5.collapse-card__title  {
          color:#2874CE;
        }
        .group  {
            text-align: center;
        }   
</style>

<script type="text/javascript">

    function setFocusG() {
        var objCtrl = document.getElementById('<%=txtHST.ClientID %>');
        if (objCtrl != null)
            objCtrl.select();
    }

    function SetGst(txt) {
        var amount = parseFloat(txt.value);
        var txtGst = document.getElementById('<%=txtHST.ClientID %>');
        txtGst.value = (amount * 13) / 100;
        round_decimals(txtGst, 2);
    }

    function setSubTotal(txt) {
        var amount = parseFloat(txt.value);
        
        var subTotal = document.getElementById('<%=txtAmount.ClientID %>');
        if (!IsNumeric(subTotal.value)) {
            alert('Please enter numeric values only.');
            subTotal.select();
            subTotal.focus();
            return;
        }
        
        if (subTotal.value == '')
            subTotal.value = 0.00;


        round_decimals(subTotal, 2);

        var GSTTotal;
        GSTTotal = (amount * 13) / 100;

        
            var GrandTotal = document.getElementById('<%=txtTotalAmount.ClientID %>');
            
            GrandTotal.value = (parseFloat(amount) + parseFloat(GSTTotal));

            round_decimals(GrandTotal, 2);
        
    }
    function setTotalAmt() {

        var GSTTotal = document.getElementById('<%=txtHST.ClientID %>');

        var JobCostAmt = document.getElementById('<%= txtAmount.ClientID %>');
        var GrandTotal = document.getElementById('<%=txtTotalAmount.ClientID %>');

        GrandTotal.value = (parseFloat(JobCostAmt.value) + parseFloat(GSTTotal.value));

        round_decimals(GrandTotal, 2);
    }

    function MpageBack() {
        window.location = 'Claims.aspx';
    }
    function Spinner() {
        var Panel = document.getElementById("<%= pnlMismatch.ClientID %>");
        Panel.style.display = "";
        __doPostBack('btnSubmit', '');
    }
    function chkUpload() {
        var btn = document.getElementById("<%=btnSubmit.ClientID %>");
        var TypeSelect = document.getElementById("<%= dllFileType.ClientID %>");
        var descRow = document.getElementById("<%= descRow.ClientID %>");
        var trInvDet = document.getElementById("<%= tblInvDet.ClientID %>");
        var txtDesc = document.getElementById("<%= txtDesc.ClientID %>");
        var FileSelect = document.getElementById("<%= FileUpload.ClientID %>");
        btn.disabled = false;
        var selectedText = TypeSelect.options[TypeSelect.selectedIndex].text;
        if (FileSelect.value.trim() == "") btn.disabled = true;
        
        if (selectedText == "Other") {
            descRow.style.display = "";
            trInvDet.style.display = "none";
            event.preventDefault();
        } else if (selectedText == "Invoice") {
            trInvDet.style.display = "";
            descRow.style.display = "none";
            event.preventDefault();
        }
        else {
            descRow.value = "";
            descRow.style.display = "none";
            trInvDet.style.display = "none";
            event.preventDefault();
        } 
    }

</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(document).ready(function () {
        document.getElementById("btnSubmit").disabled = true;
    });
</script>

<script type="text/javascript">

</script>

    <asp:LinkButton ID="LinkButton1" runat="server" /> 

    <div id="ClaimList" runat="server">
        <h4 class="ui center aligned header">Upload Document</h4>
        <div class="col-sm-4">
            <div style='margin: .8rem .4rem 0 .4rem;'>
                <div class='ui card'>
                    <div class='content'>
                        <div class='header'>
                            <div class='ui floated hlabel'><% getJobName(); %></div>
                        </div>
                    </div>
                    <div class='extra content'>
                        <table align="center">
                            <tbody>
                                <tr>
                                    <td style="padding-top:8px;"><asp:dropdownlist width="100%" id="dllFileType" runat="server" cssclass="DEInput"></asp:dropdownlist></td>
                                </tr>
                                 <tr id="descRow" runat="server">
                                    <td style="padding-top:8px;"><span style="font-size:smaller">Document Title</span><asp:textbox  width="100%" ID="txtDesc" runat="server" columns="32"></asp:textbox></td>
                                </tr>
                                <tr>
                                    <td style="padding-top:8px;"><asp:FileUpload ID="FileUpload" AllowMultiple="true" runat="server"></asp:FileUpload></td>
                                </tr>

                            </tbody>
                        </table>
                                <table id="tblInvDet" runat='server' align='center'>
                                <tr>
                                    <td style="padding-top:8px;"><span style="font-size:smaller">Invoice Date</span><asp:TextBox ID="txtInvoiceDate" runat="server" Width="100%" type="Date" style='float:left;' columns="32"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td style="padding-top:8px;"><span style="font-size:smaller">Invoice Number</span><asp:textbox  width="100%" ID="txtInvoiceNumber" runat="server" columns="32"></asp:textbox></td>
                                </tr>
                                <tr>
                                    <td style="padding-top:8px;"><span style="font-size:smaller">SubTotal Amount</span><asp:textbox  width="100%" ID="txtAmount" runat="server" columns="32"></asp:textbox></td>
                                </tr>
                                <tr>    
                                    <td style="padding-top:8px;"><span style="font-size:smaller">HST Amount</span><asp:textbox  width="100%" ID="txtHST" runat="server" columns="32"></asp:textbox></td>
                                </tr>
                                <tr>
                                    <td style="padding-top:8px;"><span style="font-size:smaller">Total Amount</span><asp:textbox  width="100%" ID="txtTotalAmount" runat="server" columns="32"></asp:textbox></td>
                                </tr>
                                </table>                        

                        
                        
                    </div>
                </div>
                <div class="group">
                   <asp:Label ID="lblmsg" runat="server" ForeColor="#CC0000"></asp:Label><br /><br />
                   <button class="ui button" id="btnSubmit" runat='server' >
                        Upload Files
                    </button>
                </div>
            </div>
        </div>
    </div>
<div style="position:relative; top:-100px; left 3%; z-index:110; display:; width: 95%;">
    <table width="100%">

        <tr>
            <td align="center">
                <asp:panel id="pnlMismatch" runat="server"  backcolor="white"   horizontalalign="Center">
                    <table>
                        <tr>
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/source.gif" Height="50px" Width="50px" />
                        </tr>
                    </table>
                </asp:panel>
            </td>
        </tr>
    </table>
</div>
</asp:Content>


