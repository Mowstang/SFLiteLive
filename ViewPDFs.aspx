<%@ Page Title="" Language="C#" MasterPageFile="~/Image.Master" AutoEventWireup="true" CodeBehind="ViewPDFs.aspx.cs" Inherits="SFLite.ViewPDFs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0"/>
    <script src="Scripts/unslider-min.js" type="text/javascript"></script>
    <link href="Styles/unslider.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/velocity.min.js" type="text/javascript"></script>
    <script src="Scripts/sb_eventmove.js" type="text/javascript"></script>
    <script src="Scripts/sb_eventswipe.js" type="text/javascript"></script>

    <style>
      /* required styles */

    .unslider-nav ol {
      list-style: none;
      text-align: center;
    }
    .unslider-nav ol li {
      display: inline-block;
      width: 6px;
      height: 6px;
      margin: 0 4px;
      background: transparent;
      border-radius: 5px;
      overflow: hidden;
      text-indent: -999em;
      border: 2px solid #fff;
      cursor: pointer;
    }
    .unslider-nav ol li.unslider-active {
      background: #fff;
      cursor: default;
    }

      /* END required styles */

    </style>

    <script type="text/javascript">

        function MpageBack() {  // called by Image Master back button
            hfprm = document.getElementById('<%= hfParams.ClientID %>');
            window.location = 'ViewDocs.aspx?' + hfprm.value;
        }

        function PicsCallBack(img) {
            $('.uSlider').unslider('animate:'+ img);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:HiddenField ID="hfStage" runat="server" />
<asp:HiddenField ID="hfParams" runat="server" />

<asp:LinkButton ID="LinkButton1" runat="server" /> <%-- included to force __doPostBack javascript function to be rendered --%>




</asp:Content>
