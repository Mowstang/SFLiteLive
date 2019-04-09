<%@ Page Language="C#"  MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Wrapper.aspx.cs" Inherits="SFLite.PhotoSrv.Wrapper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script language="javascript">
        function pickUpParam(param) {
            var ele = document.getElementById('<%= hidUpup.ClientID %>');
            ele.value = param;
            __doPostBack(param);
        }
    </script> 
    <style>
        h4.ui.center.header {
        margin-top: 1em;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h4 class="ui center aligned header">Upload Photos - <% getFile(); %></h4>
    <table width="100%">
		<tr>
			<td align="center">
                <div runat="server" id="site"><iframe id="uploader" frameborder="0" width="100%" height="360" runat="server" scrolling="no" src="about:blank" ></iframe></div>
                <asp:hiddenfield id="hidUpup" runat="server" value=""></asp:hiddenfield>
			</td>
		</tr>
	</table>
   <asp:LinkButton ID="LinkButton1" runat="server">&nbsp</asp:LinkButton>

</asp:Content>

