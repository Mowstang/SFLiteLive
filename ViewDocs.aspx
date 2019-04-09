﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Image.Master" AutoEventWireup="true" CodeBehind="ViewDocs.aspx.cs" Inherits="SFLite.ViewDocs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1">
    <link rel="stylesheet" href="Styles/swiper.min.css">
    <script src="Scripts/swiper.min.js"></script>

    <!-- Content, Style overrides, JS HERE !-->
    <style>
  
    [class*='col-'] {
      float: left;
    }
    .col-1-3 {
      width: 31%;
    }
    .group  {
        padding-top:8px;
        text-align: center;
    }
    
    div {
    display:block;
    }
    .climg {
        float:left;
        padding:4px 2px 4px 2px;
    }
    
    .swiper-container {
        width: 100%;
        height: auto;
        margin-left: auto;
        margin-right: auto;
    }
    .swiper-slide {
        text-align: center;
        font-size: 10px;
        background: #ffe;
        
        /* Center slide text vertically */
        display: -webkit-box;
        display: -ms-flexbox;
        display: -webkit-flex;
        display: flex;
        -webkit-box-pack: center;
        -ms-flex-pack: center;
        -webkit-justify-content: center;
        justify-content: center;
        -webkit-box-align: center;
        -ms-flex-align: center;
        -webkit-align-items: center;
        align-items: center;
    }
    </style>

    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            igrd = document.getElementById('<%= imageContent.ClientID %>');
            igrd.style.display = "";
        });

        function MpageBack() {  // called by Image Master back button
            hfprm = document.getElementById('<%= hfParams.ClientID %>');
            window.location = 'Claims.aspx?' + hfprm.value;
        }

        function ShowPDF(img) {
            hfprm = document.getElementById('<%= hfParams.ClientID %>');
            window.location = 'ViewPDFs.aspx?' + hfprm.value + '&DocID=' + img.toString();
        }
        function goDocs() {
            params = document.getElementById('<%= Params.ClientID %>');
            window.location = 'Uploader.aspx?' + params.value;
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:HiddenField ID="hfParams" runat="server" />
<asp:HiddenField ID="Params" runat="server" />

<asp:LinkButton ID="LinkButton1" runat="server" /> <%-- included to force __doPostBack javascript function to be rendered --%>

<div id="RoomList" runat="server">
    <h4 class="ui center aligned header"><% getRoom(); %></h4>
    <!-- Image Grid -->
    <div id="imageContent" style="padding-top:5px;padding-right:5px; padding-left:5px;" runat="server">
        <div id="Div1" class="ui grid" runat="server">
            <div class='col-1-3' onclick='goDocs()' >
                <img style='padding-right:5px;' src='Images/upDocs.png'  alt = '' width='115px' height='115px' />
            </div>
            <% getSlides(); %></div>
    </div>
</div>
</asp:Content>
