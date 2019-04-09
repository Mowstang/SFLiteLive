<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchResults.aspx.cs" Inherits="SFLite.SearchResults" %>

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
        .popped {
          z-index: 500;
        }
        .ui.floated.hlabel {
          float: left;
        }


        h5.collapse-card__title  {
          color:#2874CE;
        }

            
</style>

<script type="text/javascript">
    $(document).ready(function () {

        var 
    $headers = $('body > h3'),
    $header = $headers.first(),
    ignoreScroll = false,
    timer
  ;

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
    })
  ;
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
    })
  ;
    });

    function callThis(number) {
        var parts = number.split(':');
        parts[1] = parts[1].replace('(', ' ').replace(')', ' ').replace('-', ' ');
        parts[1] = parts[1].replace(new RegExp('  ', 'g'), ' ');
        var items = parts[1].trim().split(' ');
        if (items[2].length > 4) {
            items[2] = items[2].substr(0, 4);
        }
        var tel = '1' + items[0] + items[1] + items[2];
        if (!isNaN(parseFloat(items[3])) && isFinite(items[3])) tel += ',' + items[3];
        window.open("tel:+" + tel);
    }

    function getDirections(inf) {
        var parts = inf.split('|');
        var cvid = parts[0];
        var addr = parts[1];
        var map = document.getElementById('div' + cvid);
        var frm = document.getElementById('<%= embeddedMap.ClientID %>');
        frm.src = 'https://www.google.com/maps/embed/v1/place?q=' + addr + '&key=AIzaSyDObxwvPlDl3HQKKup6BCO5b_O_Y_P3lrM';
        $('.ui.modal')
      .modal('show')
    ;
    }

    function goDocs(params) {
        window.location = 'Uploader.aspx?' + params;
    }

    function goRooms(params) {
        window.location = 'PickRooms.aspx?' + params;
    }

</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="ClaimList" runat="server">
    <h4 class="ui center aligned header">Search Results</h4>
    <div class="col-sm-4">
        <% getResults(); %>
    </div>
</div>

<div class="ui modal map-container embed">
    <iframe id="embeddedMap" runat="server" frameborder="0" style="border:0" src="" allowfullscreen>
    </iframe>
</div>



</asp:Content>

