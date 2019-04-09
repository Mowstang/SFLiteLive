<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SFLite._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/paper-collapse.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/paper-collapse.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="col-sm-4">
         <% getNews(); %>
    </div>

<script type="text/javascript">
    $('.collapse-card').paperCollapse();
</script>

</asp:Content>
