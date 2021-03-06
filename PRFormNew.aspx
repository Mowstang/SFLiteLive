﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PRFormNew.aspx.cs" Inherits="SFLite.PRFormNew" %>
<%@ Register assembly="ComponentArt.Web.UI" namespace="ComponentArt.Web.UI" tagprefix="ComponentArt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">    
  
<link href="Styles/paper-collapse.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        .style1
        {
            width: 30%;
            height: 29px;
        }
        .style2
        {
            width: 70%;
            height: 29px;
        }
        .divtable
        {
            width: 100%; 
            color: Black; 
            background-color: #F0F8FF; 
            border: 4px #808080; 
        }
        .divtoptable
        {
            width: 100%; 
            color: Black; 
            background-color: #F0F8FF; 
            border: 4px #808080;
        }
        .style3
        {
            height: 40px;
        }
        .style4
        {
            width: 1546px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type="text/javascript">
    function myFunction(ctrlId) {
        var ctrl = document.getElementById("MainContent_" + ctrlId);
        ctrl.style.backgroundColor = "white";
    }
    function CheckLength(obj, l) {
        var len = obj.value.length;
        if (len > l) {
            alert('You have exceeded the maximum for this field.')
            obj.focus();
        }
    }
    function formatAmount(num) {
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num))
            num = "0";
        sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10)
            cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + num + '.' + cents);

    }

    function formatCurrency(e) {
        var eventSource = event.srcElement;
        var eventSourceId = event.srcElement.id;
        var tgt = document.getElementById(eventSource.id);
        var esText = tgt.value;
        if (esText != "")
            esText = formatAmount(esText);
        else
            esText = "";
        tgt.value = esText;
        
        if (eventSourceId == "MainContent_txtEmergencyReserveAmount") {

            if (esText != "0.00") {
                var x = document.getElementById("<%=divEmergInfo.ClientID %>");
                x.style.display="block";
            
               
            }
        }
    }

    function NoDecimal(e) {
        var keynum;
        var keychar;
        var eventSource = event.srcElement;
        var eventSourceId = event.srcElement.id;
        if (window.event) {
            keynum = e.keyCode;
            if (((keynum > 95) && (keynum < 106)) || ((keynum > 47) && (keynum < 58)) || (keynum < 13)) {

            } else {
                if (e.preventDefault) {
                    e.preventDefault()
                } else {
                    e.stop()
                }
                e.returnValue = false;
                e.cancelBubble = true;
                e.stopPropagation();
            }
        }
        return false;
    }
</script>

 <div id='PrContent' style='padding-left: 5px;'>
  <div> 
      <div style="width: 98%; color: white; background-color:#012560; border: 4px solid gray; padding: 7px; font-family: Arial, sans-serif; font-size: x-large; font-weight: normal; font-style: italic; font-variant: normal; text-transform: capitalize;" 
          align="center" >
          <asp:Label runat='server' ID='lblTitle1' Text='Preliminary Report'></asp:Label></br></br>
          <asp:Label runat='server' ID='lblTitle' Text=''></asp:Label></br></br>
          <asp:Label runat='server' ID='lblJobName' ></asp:Label>
          
    </div>
<div class='ui card' style='width:98%; top: 0px; left: 0px;'>
    <table id="Table6"  width="100%">
        <tr>
            <td align="center"><asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label></td>
        </tr>
    </table> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px; " align="center" >
          <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>General Information</a>
    </div>
    <div style="width: 100%; color: Black; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
        <div id='divEmergInfo' runat="server">
        <table style='width:98%; margin-left:5px;'>
            <tr>
                <td style='width:50%'>
                    <asp:Label ID="Labemergstard" runat="server" Text="Emerg Comp Date:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtEmergCompletionDate" runat="server" Width="95%" type="Date" style='float:left;' onfocus="myFunction('txtEmergCompletionDate')"></asp:TextBox>
                    <asp:TextBox ID="txtecompdate" runat="server" Width="95%" ReadOnly="true" style='float:left;'></asp:TextBox>
                </td>
            </tr>
        </table>
        </div>
        <table style='width:98%; margin-left:5px;'>
            <tr >
                <td style='width:50%'>
                    <asp:Label ID="LabEmergBud" runat="server" Text="Emergency Reserve" style='float:left;'></asp:Label>  
                    <asp:TextBox runat="server" ID="txtEmergencyReserveAmount" Width="95%" style='float:left;'></asp:TextBox>
                </td>

                <td style='width:50%'><asp:Label ID="Labconbu" runat="server" Text="Content Reserve" style='float:left;'></asp:Label>   
                  <asp:TextBox ID="txtContentBudgetReserve" width="95%" runat="server" style='float:left;'></asp:TextBox>
                </td>
                <td style='width:50%'>
                    <asp:Label ID="Labrb" runat="server" Text="Rebuild Budget:" style='float:left;'></asp:Label>   
                    <asp:TextBox ID="txtRestorationReserveAmount" Width="95%" runat="server" style='float:left;'></asp:TextBox>
                </td>

            </tr>
        </table>
    </div>
</div>
</div>

<div class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px; " align="center" >
        <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Description of Property:</a>
    </div>

    <div style="width: 100%; color: Black; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
        <table style='width:98%; margin-left:5px;'>
            <tr>
                <td style='width: 50%'><asp:Label ID="Labaop" runat="server" Text="Age of Property:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtAgeOfProperty" runat="server" Width="96%" style='float:left;' onfocus="myFunction('txtAgeOfProperty')"></asp:TextBox>
                </td>
                <td style='width:50%'>
                    <asp:Label ID="lblSFofBuilding" runat="server" Text="SF of Builing (Excl Basement):" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtSFOfBuilding" runat="server" Width="96%" style='float:left;' onfocus="myFunction('txtSFOfBuilding')"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style='width:50%'><asp:Label ID="Labcond" runat="server" Text="Condition of Building:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtConditionOfBuilding" runat="server" Width="96%" style='float:left;' onfocus="myFunction('txtConditionOfBuilding')"></asp:TextBox>
                </td>
                <td style='width: 50%'>
                    <asp:Label ID="lblQualiltyOfFinishes" runat="server" Text="Quality Of Finishes:" style='float:left;'></asp:Label>
                        <asp:DropDownList ID="ddlQualityOfFinish" runat="server" Width="96%" style='float:left;' onfocus="myFunction('ddlQualityOfFinish')">
                            <asp:ListItem Value=""></asp:ListItem>
                            <asp:ListItem Value="Standard Grade">Standard Grade</asp:ListItem>
                            <asp:ListItem Value="Average Grade">Average Grade</asp:ListItem>
                            <asp:ListItem Value="High Grade">High Grade</asp:ListItem>
                            <asp:ListItem Value="Premium Grade">Premium Grade</asp:ListItem>
                        </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style='width:50%'>
                <asp:Label ID="Labtypbu" runat="server" Text="Type of Building:" style='float:left;'></asp:Label>    
                    <asp:DropDownList ID="ddlTypeOfBuilding" runat="server" Width="96%" style='float:left;'  onfocus="myFunction('ddlTypeOfBuilding')">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="Residential">Residential</asp:ListItem>
                        <asp:ListItem Value="Commercial">Comercial</asp:ListItem>
                        <asp:ListItem Value="Industrial">Industrial</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style='width: 100%' colspan='2'>
                    <asp:Label ID="lblPropertyStructure" runat="server" Text="Property/Structure: " style='float:left;'  ></asp:Label>
                    <asp:TextBox ID="txtProperty" runat="server" Width="98%" Height="75px" TextMode="MultiLine" style='float:left;' onfocus="myFunction('txtProperty')"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
</div>

<div class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px; " align="center" >
        <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Description of Damages and Cause:</a>
    </div>
    <div style="width: 100%; color: Black; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
        <table style='width:98%; margin-left:5px;'>
            <tr>
                <td style='width:50%'><asp:Label ID="lblDamageLocation" runat="server" Text="Damage Location:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtDamageLocation" runat="server" Width="95%" style='float:left;'  onfocus="myFunction('txtDamageLocation')"></asp:TextBox>
                </td>
                <td style='width:50%'><asp:Label ID="lblSFAA" runat="server" Text="SF of Affected Area:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtSquareFootAffectedAreas" runat="server" Width="95%" style='float:left;'   onfocus="myFunction('txtSquareFootAffectedAreas')"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style='width:50%'><asp:Label ID="lblRoomsAffected" runat="server" Text="# of Rooms Affected:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtNumRoomsAffected" runat="server" Width="95%" style='float:left;'  onfocus="myFunction('txtNumRoomsAffected')"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style='width:50%'><asp:Label ID="lblWaterType" runat="server" Text="Water Type:" style='float:left;'></asp:Label>   
                    <asp:DropDownList ID="ddlWaterType" runat="server" Width="95%" style='float:left;'  onfocus="myFunction('ddlWaterType')">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="Category 1">Category 1: Clean Water</asp:ListItem>
                        <asp:ListItem Value="Category 2">Category 2: Grey water</asp:ListItem>
                        <asp:ListItem Value="Category 3">Category 3: Black Water</asp:ListItem>
                        <asp:ListItem Value="N/A">N/A</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style='width:30%'><asp:Label ID="lblKitchenAffected" runat="server" Text="Kitchen Affected:" style='float:left;'></asp:Label>   
                    <asp:RadioButtonList runat="server" ID="rbtnKitchenAffected" RepeatDirection="Horizontal"  Width="95%" >
                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style='width:50%'><asp:Label ID="lblFireType" runat="server" Text="Fire Type:" style='float:left;'></asp:Label>   
                    <asp:DropDownList ID="ddlFireType" runat="server" Width="95%" style='overflow: visible; float:left;'  onfocus="myFunction('ddlFireType')">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="Type 1">Type 1: Smoke Damage Only</asp:ListItem>
                        <asp:ListItem Value="Type 2">Type 2: Flames Affect Interior Finish Only</asp:ListItem>
                        <asp:ListItem Value="Type 3">Type 3: Flames Affect Structure Without Perferations</asp:ListItem>
                        <asp:ListItem Value="Type 4">Type 4: Flames Perferate Roof</asp:ListItem>
                        <asp:ListItem Value="Type 5">Type 5: Flames Perferate Roof and Walls</asp:ListItem>
                        <asp:ListItem Value="Type 6">Type 6: Building Is A Total Loss</asp:ListItem>
                        <asp:ListItem Value="N/A">N/A</asp:ListItem>  
                    </asp:DropDownList>
                </td>
                <td style='width:50%'><asp:Label ID="lblBathroomsAffected" runat="server" Text="Bathrooms Affected:" style='float:left;'></asp:Label> 
                    <asp:RadioButtonList runat="server" ID="rbtnBathRoomAffected" RepeatDirection="Horizontal"  Width="95%">
                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style='width:50%' colspan='2'>
                    <asp:Label ID="lblItemsOfHighValue" runat="server" Text="Items Of High Value:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtItemsOfHighValue" runat="server" Width="98%" Height="75px" TextMode="MultiLine" style='float:left;'  onfocus="myFunction('txtItemsOfHighValue')"></asp:TextBox>
                </td>
            </tr>
            <tr runat='server' id='trRestorationComments'>
                <td style='width:50%' colspan='2'>
                    <asp:Label ID="lblRestorationComments" runat="server" Text="Restoration Comments:" style='float:left;'></asp:Label>
                    <asp:TextBox ID="txtRestoration" runat="server" Width="98%" Height="75px" TextMode="MultiLine" style='float:left;' onfocus="myFunction('txtRestoration')"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
</div>
<div id="Equip" runat='server' class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px;" align="center" >
        <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Moisture Readings</a>
    </div>
    <div class="divtable" runat="server" >
        <div class="divtable" style="width:98%; padding: 5px;"> 
            <table style='width:100%;'>
                <tr>
                    <td>
                        <asp:Label ID="lblEquipment" runat='server' Text='Equipment' Font-Bold="True"></asp:Label>
                    </td>
                    <td class="DEField" style="width:44%"><asp:Label ID="Label3" runat="server" Text="Air Movers: " meta:txta="Air Movers:"></asp:Label>
                        <asp:TextBox ID="txtFans" CssClass="DEInput" runat="server" Width="55px" Text="0 " meta:txtd="0" onfocus="myFunction('txtFans')"></asp:TextBox>
                    </td>
                    <td class="DEField" style="width:30%"><asp:Label ID="Label4" runat="server" Text="Dehus: " meta:txta="Dehus:"></asp:Label>
                        <asp:TextBox ID="txtDehus" CssClass="DEInput" runat="server" Width="55px" Text="0 " meta:txtd="0" onfocus="myFunction('txtDehus')"></asp:TextBox>
                    </td>
                </tr>
                <tr><td>&nbsp</td></tr>
            </table>
            <table style='width:100%;'>
                <tr>
                    <td style="width:30%; vertical-align:top"><asp:Label ID="lblMoistureReadings" runat='server' Text='Initial Moisture Readings' Font-Bold="True"></asp:Label></td>
                    <td colspan="5" width="100%">
                        <table cellspacing="2" cellpadding="2" width="100%" border="0">
                            <tr>
                                <td style="vertical-align:bottom"><asp:Literal ID="Literal11" runat='server' Text='Affected Areas: ' /></td>
                                <td><asp:Label ID="Label5" runat="server" Text="Temp:" meta:txta="Temp:"></asp:Label>
                                    <asp:TextBox ID="txtAffectedTemp" CssClass="DEInput" runat="server" Width="55px" Text="0 " meta:txtd="0.0" onfocus="myFunction('txtAffectedTemp')"></asp:TextBox>
                                </td>
                                <td><asp:Label ID="Label6" runat="server" Text="RH:" meta:txta="RH:"></asp:Label>
                                    <asp:TextBox ID="txtAffectedRH" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtAffectedRH')"></asp:TextBox>
                                </td>
                                <td><asp:Label ID="Label7" runat="server" Text="GPP:" meta:txta="GPP: "></asp:Label>
                                    <asp:TextBox ID="txtAffectedGPP" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtAffectedGPP')"></asp:TextBox>
                                </td>

                            </tr>
                            <tr>
                                <td style="vertical-align:bottom" meta:txtd="Non Affected Areas:"><asp:Literal ID="Literal12" runat='server' Text='UnAffected Areas:' /></td>
                                <td class="DEField" valign="middle"><asp:Label ID="Label8" runat="server" Text="Temp:" meta:txta="Temp:"></asp:Label>
                                    <asp:TextBox ID="txtNonAffectedTemp" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtNonAffectedTemp')"></asp:TextBox>
                                </td>
                                <td valign="middle"><asp:Label ID="Label9" runat="server" Text="RH:" meta:txta="RH:"></asp:Label>
                                    <asp:TextBox ID="txtNonAffectedRH" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtNonAffectedRH')"></asp:TextBox>
                                </td>
                                <td valign="middle"><asp:Label ID="Label10" runat="server" Text="GPP:" meta:txta="GPP:"></asp:Label>
                                    <asp:TextBox ID="txtNonAffectedGPP" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtNonAffectedGPP')"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align:bottom"><asp:Literal ID="Literal13" runat='server' Text='External Areas:' /></td>
                                <td valign="middle"><asp:Label ID="Label11" runat="server" Text="Temp:" meta:txta="Temp:"></asp:Label>
                                    <asp:TextBox ID="txtExternalTemp" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtExternalTemp')"></asp:TextBox>
                                </td>
                                <td valign="middle">
                                    <asp:Label ID="Label12" runat="server" Text="RH:" meta:txta="RH:"></asp:Label>
                                    <asp:TextBox ID="txtExternalRH" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtExternalRH')"></asp:TextBox>
                                </td>
                                <td valign="middle">
                                    <asp:Label ID="Label13" runat="server" Text="GPP:" meta:txta="GPP:"></asp:Label>
                                    <asp:TextBox ID="txtExternalGPP" CssClass="DEInput" runat="server" Width="55px" Text="0" meta:txtd="0.0" onfocus="myFunction('txtExternalGPP')"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>


<div class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px;" align="center" >
        <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Description of Pre-Existing Conditions:</a>
    </div>
    <div class="divtable">
      <div style="width: 100%; color: Black; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
          <div style='width:98%; margin-left:5px;'>
              <asp:Literal ID="Literal14" runat='server' Text='Description of Pre-Exising Damages:'/>
              <asp:TextBox ID="txtPreExistingDamage" runat="server" Height="75px" TextMode="MultiLine" Width="98%" onfocus="myFunction('txtPreExistingDamage')"></asp:TextBox><br />
          </div>
      </div>
    </div>
</div>
    
<div class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px; " 
        align="center" >
        <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Description of Emergency Services Required:</a>
    </div>
    
    <div class="divtable">
      <div style="width: 100%; color: Black; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
      <table style='width:98%; margin-left:5px;'>
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="lblEmergServices" runat='server' Text="Emergency Service:" style='float:left;'></asp:Label> 
            </td>
            <td>
                <asp:RadioButtonList ID="rbtnEmergencyService" runat="server" Width="96%" RepeatDirection="Horizontal">
                    <asp:ListItem Value="1" Text="Yes " meta:txtd="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No" meta:txtd="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="Label1" runat="server" Text="Adjuster Needed On Site?" style='float:left;'></asp:Label> 
            </td>
            <td>
                <asp:RadioButtonList ID="rbtnAdjusterNeeded" runat="server" Width="96%" RepeatDirection="Horizontal">
                    <asp:ListItem Value="1" Text="Yes" meta:txtd="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No" meta:txtd="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>    
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="Label28" runat="server" Text="Vacate Premises?" style='float:left;'></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList runat="server" ID="rbtnVacatePremises" RepeatDirection="Horizontal"  Width="96%">
                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        
        </tr>
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="lblPackoutRequired" runat='server'  Text="Content Packout?:" style='float:left;'></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList ID="rbtnContentDamage" runat="server" RepeatDirection="Horizontal"  Width="96%">
                    <asp:ListItem Value="1" Text="Yes" meta:txtd="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No" meta:txtd="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="lblBuildingDamage" runat='server' Text="Structural Damages:" style='float:left;'></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList ID="rbtnBuildingDamage" runat="server" RepeatDirection="Horizontal" Width="96%">
                    <asp:ListItem Value="1" Text="Yes" meta:txtd="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No" meta:txtd="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>

        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="Label31" runat="server" Text="ACM/Mould/Hazardous Materials?" style='float:left;'></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList runat="server" ID="rbtnHazardousMaterials" RepeatDirection="Horizontal"  Width="96%">
                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                </asp:RadioButtonList>
            </td>        
        </tr>
        <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="Label27" runat="server" Text="Drying Strategy:" style='float:left;'></asp:Label>   
            </td>
            <td>
                <asp:DropDownList ID="ddlDryingStrategy" runat="server" Width="96%" style='float:left;' onfocus="myFunction('ddlDryingStrategy')">
                    <asp:ListItem Value=""></asp:ListItem>
                    <asp:ListItem Value="Structural">Structural</asp:ListItem>
                    <asp:ListItem Value="Restorative">Restorative</asp:ListItem>
                    <asp:ListItem Value="Preventative">Preventative</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
       <tr>
            <td style='width:50%;height:25px'>
                <asp:Label ID="Label29" runat="server" Text="Tearout Required?" style='float:left;'></asp:Label> 
            </td>
            <td>
                <asp:DropDownList ID="ddlTearOutRequired" runat="server" Width="96%" style='float:left;' onfocus="myFunction('ddlTearOutRequired')">
                    <asp:ListItem Value=""></asp:ListItem>
                    <asp:ListItem Value="No">No</asp:ListItem>
                    <asp:ListItem Value="Partial">Partial</asp:ListItem>
                    <asp:ListItem Value="Full">Full</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan='2'>
                <asp:Literal ID="Literal7" runat='server' Text='We have responded with emergency service to:'/>
                <asp:TextBox ID="txtResponded" runat="server" Height="75px" TextMode="MultiLine" Width="98%" onfocus="myFunction('txtResponded')"></asp:TextBox>
            </td>
        </tr>
    </table>

 </div>


      
      </div>
      </div>
<div class='ui card' style='width:98%; top: 0px; left: 0px;'> 
    <div style="width: 100%; color: #000000; background-color: #C0C0C0; border: 4px #808080; padding: 15px;" align="center" >
          <a class="c" style='font-size:x-large; color: #000000; font-weight: bold;'>Description of Required Repairs:</a>
    </div>
    <div style="width: 100%; border: 4px #808080; padding: 5px; background-color: #F0F8FF;" >
        <table style='width:98%; margin-left:5px;'>
            <tr>
                <td colspan='2'>
                    <asp:Literal ID="Literal1" runat='server' Text='Required Repairs:'/>
                    <asp:TextBox ID="txtRequiredRepairs" runat="server" Height="75px" TextMode="MultiLine" Width="98%" onfocus="myFunction('txtRequiredRepairs')"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td style='width:50%;height:25px'>
                    <asp:Label ID="Label2" runat="server" Text="Inspection Completed By:" style='float:left;'></asp:Label> 
                </td>
                <td style='width:50%;height:25px'>
                    <asp:DropDownList runat="server" ID="ddlPM" Width="100%"></asp:DropDownList>
                </td>
                
            </tr>
        </table>
    </div>
</div>


<div class='ui card' style='width:98%; top: 10px; left: 0px;'> 
    <table style='width:100%; margin-left:10px; margin-top:10px;margin-bottom:10px'>
        <tr style="">
            <td style='width:45%; padding-left:10px;' >
                <asp:Button id="btnsub" runat="server" Text="Submit" Height="45px" Width="95%" mxta:text="Submit" onclientclick="PopUpDialogs();" OnClick="btnsubmt_Click"/> 
            </td>
            <td style='width: 45%; padding-right: 8px;''>
                &nbsp 
                <asp:Button id="btnret" runat="server" Text="Return" Height="45px" Width="96%" mxta:text="Return" />
            </td>
        </tr>
    </table>
</div>
</div>
<div>&nbsp

                        <asp:Literal runat='server' ID='litMessage'></asp:Literal>

            <input type='hidden' runat='server' id='hidBlnCloseFile'/>
            <input type='hidden' runat='server' id='hidBlnCreateRFile'/>
            <input type='hidden' runat='server' id='hidBlnReturn' value='false'/>
            <input type='hidden' runat='server' id='hidBlnFullService' value='0'/>
            <input type='hidden' runat='server' id='HidBlnIsError' value='0'/>
</div>
</asp:Content>

 
 
 
 
      

 
 
 
 
 
 
    
 
 
 
 
      

 
 
 
 
 
 
    