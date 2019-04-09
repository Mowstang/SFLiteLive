<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditHours.aspx.cs" Inherits="SFLite.EditHours" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
        <link href="Styles/paper-collapse.css" rel="stylesheet" type="text/css" />

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
        .ui.floated.hlabel {
          float: left;
        }
        .group  {
            text-align: center;
        }
            
</style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script language="javascript" type="text/javascript">
    $(document).ready(function () {
    });

</script>

    <asp:LinkButton ID="LinkButton1" runat="server" /> <%-- included to force __doPostBack javascript function to be rendered --%>

    <div id="ClaimList" runat="server">
        <h4 class="ui center aligned header">Edit Hours</h4>
        <div class="col-sm-4">
            <div style='margin: .8rem .4rem 0 .4rem;'>
                <div id="uiCard" class='ui card' runat="server">
                    <div class='content'>
                        <div class='header'>
                            <div class='ui floated hlabel'><%= JobName %></div>
                        </div>
                    </div>
                    <div class='extra content'>
                        <div class='header'><span style='font-size:medium; color:#000000;'><%= sday %></span></div>
                        <div class='header'><span style='font-size:medium; color:#000000;'>Actual Hours:</span></div>
                        <table align="center">
                            <tbody>
                                <tr>
                                    <td id="tdStart" align="right" style="padding-top:8px">START TIME&nbsp;&nbsp;&nbsp;<asp:dropdownlist id="ddlStartTime" runat="server" cssclass="DEInput"></asp:dropdownlist></td>
                                </tr>
                                <tr>
                                    <td id="tdEnd" align="right" style="padding-top:8px">END TIME&nbsp;&nbsp;&nbsp;<asp:dropdownlist id="ddlEndTime" runat="server" cssclass="DEInput"></asp:dropdownlist></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="group">
                        <asp:Label ID="lblmsg" runat="server" ForeColor="#CC0000"></asp:Label><br /><br />
                    </div>
                    <div class='extra content'>
                        <div class='header'><span style='font-size:medium; color:#000000;'>ADDITIONAL INFORMATION:</span></div>
                        <div style="margin-top:8px"><textarea rows="10" id="txtAddInfo" style="width:98%;" runat="server"></textarea></div>
                        <div class="ui checkbox"><input type="checkbox" id="chkEmailPMOA" name="chkEmailPMOA" runat="server"><label>Send notes to PM/OA</label></div>
                    </div>
                </div>
                <div class="group">
                    <asp:button class="ui button" id="abtnSubmit" onclick="btnSubmit_Click" runat="server" Text="Submit"></asp:button>&nbsp;&nbsp;&nbsp;&nbsp;
                    <button class="ui button" id="abtnCancel" onclick="javascript:history.go(-1); return false;"> Cancel</button>
                </div>

            </div>
        </div>
        
    </div>
    <div id="img-out"></div>

</asp:Content>

