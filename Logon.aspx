<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="SFLite.Logon" %>

<html>
	<head>
		<title>Work Flow -- Logon</title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
        <link href="jqwidgets/styles/jqx.base.css" rel="stylesheet" type="text/css" />
        <link href="Styles/material.css" rel="stylesheet" type="text/css" />
        <script type="text/javascript" src="Scripts/jquery-2.2.3.min.js"></script>
        <script type="text/javascript" src="jqwidgets/jqxcore.js" ></script>
        <script type="text/javascript" src="jqwidgets/jqxinput.js" ></script>
        <script type="text/javascript" src="jqwidgets/jqxbuttons.js" ></script>
        <script type="text/javascript">
            function togglePass(state) {
                if (state == 0) {
                    document.getElementById('Pass').style.display = 'none';
                    document.getElementById('txtPass').style.display = '';
                    document.getElementById('txtPass').focus();
                } else {
                    document.getElementById('txtPass').style.display = 'none';
                    document.getElementById('Pass').style.display = '';
                }
            }
        </script>
	</head>

	<body>
		<form id="Form1" method="post" runat="server" autocomplete="off">

        <div class="container">
             
            <script type="text/javascript">
                $(document).ready(function () {
                    $("#txtUserId").jqxInput({ placeHolder: "User Id", height: 25, width: 150, minLength: 1 });
                    $("#txtPass").jqxInput({ height: 25, width: 150, minLength: 1 });
                    $('#Pass').jqxInput({ placeHolder: "Enter password:", height: 25, width: 150 });
                    $("#jqxSubmitButton").jqxButton({ theme: 'energyblue', width: '100', height: '25' });
                    togglePass(1);
                });
            </script>    
                       
            <div class="stack">
                <img alt="Strone" style=" max-width:98%; width:100%" src="Images/smartflowlogo.jpg" /><br /><br />
                <input type="text" required="true" runat="server" id="txtUserId" />
            </div>
            <br />
            <div class="group">
                <input type="password" required="true" runat="server" id="txtPass" />
                <input type="text" id="Pass" onfocus="togglePass(0)" />
            </div>
            <div class="group">
                <input style="margin-top: 20px;" runat="server" type="submit" value="Submit" id='jqxSubmitButton' causesvalidation="False" />
                <br />
                <br /><asp:Label id="lblMessage" runat="server" ForeColor="Red" style="text-align: center"></asp:Label>
                <br /><br /><br /><a href="https://smartflow.strone.ca/strwebflow/logon.aspx" target="_blank">Desktop Application</a> 
                
            </div>

        </div>
		</form>

     </body>
</html>