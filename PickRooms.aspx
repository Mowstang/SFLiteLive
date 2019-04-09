<%@ Page Title="" Language="C#" MasterPageFile="~/Image.Master" AutoEventWireup="True" CodeBehind="PickRooms.aspx.cs" Inherits="SFLite.PickRooms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

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
        
        .ui.bottom.attached.button   
        {
            border:0 0 0 0;
            margin:0 0 0 0;
        }
 
        .cdlbl  
        {
            font-size: 1em;
            line-height: 90%;
            position: absolute;
            top: 15%;
            margin-left: 15%;
            color: #fff; 
        }
        .subhead  
        { 
            margin-top: .5em;
            color:Blue; 
            font-weight:bolder;
            text-align: center;
        }
        
        .group  {
            text-align: center;
        }
            
</style>

<script type="text/javascript">
    $(document).ready(function () {

        $('.ui.dropdown')
          .dropdown()
        ;

        var 
        $headers = $('body > h3'),
        $header = $headers.first(),
        ignoreScroll = false,
        timer;

        // Preserve example in viewport when resizing browser
        $(window)
        .on('resize', function () {
            // ignore callbacks from scroll change
            clearTimeout(timer);
            $headers.visibility('disable callbacks');

            // preserve position
            $(document).scrollTop($header.offset().top);

            // allow callbacks in 500ms
            timer = setTimeout(function () {
                $headers.visibility('enable callbacks');
            }, 500);
        });

        $headers
        .visibility({
            // fire once each time passed
            once: false,

            // don't refresh position on resize
            checkOnRefresh: true,

            // lock to this element on resize
            onTopPassed: function () {
                $header = $(this);
            },
            onTopPassedReverse: function () {
                $header = $(this);
            }
        });

    });

    function goUploader(params) {
        window.location = 'PhotoSrv/Wrapper.aspx?sa=0' + params;
    }

    function goViewer(params) {
        window.location = 'ViewImages.aspx?' + params;
    }

    function RoomsCallBack(val) {
        //add room folder
        //PickRooms.ServerSideCheck(val, ServerSideCheck_RoomsCallBack);
        var hf1 = document.getElementById('<%= hf1.ClientID %>');
        hf1.value = val;
        __doPostBack('allRooms', '')
    }

    function ServerSideCheck_RoomsCallBack(pass) {
        //handle the callback response    
        var mssg = objToString(pass);
        var rooms = document.getElementById('allRooms');
        rooms.innerHTML = mssg;
    }

    function objToString(obj) {
        var str = '';
        for (var p in obj) {
            if (obj.hasOwnProperty(p)) {
                if (p == 'value') {
                    str = obj[p];
                    return str;
                }
            }
        }
        return str;
    }

    function MpageBack() {  // called by Image Master back button
        window.location = 'Claims.aspx';
    }



</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:HiddenField ID="hf1" runat="server" />
<div id="RoomList" runat="server">

    <h4 class="ui center aligned header">Current Locations - <% getFile(); %></h4>
    <div class="col-sm-4">
        <div id='allRooms' class='ui three cards'>
            <% getRooms(); %>
        </div>
        <div class='ui three cards'>
            <% addRooms(); %>
            </div>
        </div>
    </div>
</div>


</asp:Content>

