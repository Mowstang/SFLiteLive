<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewImages.aspx.cs" Inherits="SFLite.ViewImages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link rel="stylesheet" href="Styles/swiper.min.css">
    <script src="Scripts/swiper.min.js"></script>

    <!-- Content, Style overrides, JS HERE !-->
    <style>

    .swiper-container {
        width: 98%;
        height: auto;
        margin-left: auto;
        margin-right: auto;
    }
    .swiper-slide {
        text-align: center;
        font-size: 10px;
        background: #ffe;
        height: 120px;
        
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
    .group  {
        padding-top:8px;
        text-align: center;
    
    ] 
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:HiddenField ID="hf1" runat="server" />
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        var swiper = new Swiper('.swiper-container', {
            pagination: '.swiper-pagination',
            slidesPerView: 3,
            slidesPerColumn: 4,
            paginationClickable: true,
            spaceBetween: 6
        });
    });
</script>

<div id="RoomList" runat="server">
    <h4 class="ui center aligned header"><% getRoom(); %></h4>
    <!-- Swiper -->
    <div class="swiper-container">
        <div class="swiper-wrapper"><% getSlides(); %></div>
        <!-- Add Pagination -->
        <div class="swiper-pagination"></div>
    </div>
    <div class="group">
        <button class="ui button" id="btnCancel" onclick="javascript:history.go(-1); return false;">
            Cancel
        </button>
    </div>
</div>

</asp:Content>
